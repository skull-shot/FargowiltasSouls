﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class PalmTreeSentry : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/Minions", Name);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 82;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 7200;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();


            if (!(player.Alive() && player.HasEffect<PalmwoodEffect>() && !player.HasEffect<TimberEffect>()))
            {
                Projectile.Kill();
                return;
            }//this is to work properly with sentry despawning
            bool forceEffect = modPlayer.ForceEffect<PalmWoodEnchant>();

            //BIG palm sentry!
            Projectile.scale = forceEffect ? 2 : 1;
            Projectile.height = 82 * (int)Projectile.scale;
            Projectile.width = 80 * (int)Projectile.scale;
            //Projectile.height = forcePalm ? 110 : 82; //stupid fucking idiot dumbass hatred way of making palm not clip into death

            Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            Projectile.ai[1] += 1f;

            int attackRate = forceEffect ? 30 : 45;

            if (Projectile.ai[1] >= attackRate)
            {
                float num = 2000f;
                int npcIndex = -1;
                for (int i = 0; i < 200; i++)
                {
                    float dist = Vector2.Distance(Projectile.Center, Main.npc[i].Center);

                    if (dist < num && dist < 420 && Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        npcIndex = i;
                        num = dist;
                    }
                }

                if (npcIndex != -1)
                {
                    NPC target = Main.npc[npcIndex];

                    if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
                    {
                        Vector2 offset = -Vector2.UnitY * 55 * MathHelper.Clamp(Projectile.Distance(target.Center) / 420, 0, 1);
                        Vector2 velocity = Vector2.Normalize(target.Center + offset - Projectile.Center) * 16;

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PalmwoodShot>(), Projectile.damage, 2, Projectile.owner);
                    }
                }
                Projectile.ai[1] = 0f;

                //kill if too far away
                if (Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center) > 2000)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                TextureAssets.Projectile[Type].Size() / 2, // this is the only reason why
                Projectile.scale,
                SpriteEffects.None);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}
