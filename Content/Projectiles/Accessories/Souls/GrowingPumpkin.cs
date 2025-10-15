﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class GrowingPumpkin : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override bool? CanDamage() => Projectile.frame == 4; //only hit enemies when fully grown
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);

            Projectile.ai[0]++;

            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            //falls to the ground
            if (!modPlayer.ForceEffect<PumpkinEnchant>())
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
                if (Projectile.velocity.Y > 16f)
                {
                    Projectile.velocity.Y = 16f;
                }
            }

            Projectile.damage = PumpkinEffect.BaseDamage(player);

            if (Projectile.frame != 4)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 60)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = (Projectile.frame + 1) % 5;

                    //dust
                    if (Projectile.frame == 4)
                    {
                        const int max = 16;
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 vector6 = Vector2.UnitY * 5f;
                            vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Projectile.Center;
                            Vector2 vector7 = vector6 - Projectile.Center;
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Grass, 0f, 0f, 0, default, 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = vector7;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player hitPlayer = Main.player[i];
                    if (hitPlayer.Alive() && hitPlayer.Hitbox.Intersects(Projectile.Hitbox))
                    {
                        if (Main.myPlayer == hitPlayer.whoAmI)
                        {
                            int heal = 18;
                            if (modPlayer.ForceEffect<PumpkinEnchant>())
                                heal *= 2;
                            FargoGlobalItem.OnRetrievePickup(hitPlayer);
                            hitPlayer.FargoSouls().HealPlayer(heal);
                        }
                        SoundEngine.PlaySound(SoundID.Item2, hitPlayer.Center);
                        Projectile.Kill();
                    }
                }
            }

            Projectile.width = (int)(32 * Projectile.scale);
            Projectile.height = (int)(32 * Projectile.scale); //make it not float when shrinking

            if (Projectile.ai[0] > 60 * 20) //make Projectile shrink and disappear after this many seconds instead of lasting forever
                Projectile.scale -= 0.01f;

            if (Projectile.scale <= 0)
                Projectile.Kill();
        }

        private void SpawnFire(FargoSoulsPlayer modPlayer)
        {
            //leave some fire behind
            Projectile[] fires = FargoSoulsUtil.XWay(5, Projectile.GetSource_FromThis(), Projectile.Center, ModContent.ProjectileType<PumpkinFlame>(), 3, Projectile.damage, 0);
            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            //dont do fire explosion on death if it dies from scale thing or isnt full grown
            if (Projectile.scale <= 0 || Projectile.frame != 4)
                return;

            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            SpawnFire(modPlayer);
            const int max = 16;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Grass, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //flash when fully grown
            if (Projectile.frame == 4)
            {
                Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
                int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
                int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;

                Color color26 = lightColor;
                color26 = Projectile.GetAlpha(color26);

                if (Projectile.ai[0] % 10 < 5)
                    color26.A = 0;

                SpriteEffects effects = SpriteEffects.None;

                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
                return false;
            }

            return true;
        }
    }
}