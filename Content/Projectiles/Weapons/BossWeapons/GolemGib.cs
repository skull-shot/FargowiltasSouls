﻿using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons
{
    public class GolemGib : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/BossWeapons", "GolemGib1");
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreAI()
        {
            if (Projectile.ai[1] == 2)
            {
                Projectile.width = 34;
                Projectile.height = 36;
            }
            else if (Projectile.ai[1] == 3)
            {
                Projectile.width = 24;
                Projectile.height = 36;
            }
            else if (Projectile.ai[1] == 4)
            {
                Projectile.width = 32;
                Projectile.height = 28;
            }
            else if (Projectile.ai[1] == 5)
            {
                Projectile.width = 36;
                Projectile.height = 38;
            }
            else if (Projectile.ai[1] == 6)
            {
                Projectile.width = 52;
                Projectile.height = 54;
            }
            else if (Projectile.ai[1] == 7)
            {
                Projectile.width = 40;
                Projectile.height = 26;
            }
            else if (Projectile.ai[1] == 8)
            {
                Projectile.width = 62;
                Projectile.height = 42;
            }
            else if (Projectile.ai[1] == 9)
            {
                Projectile.width = 14;
                Projectile.height = 16;
            }
            else if (Projectile.ai[1] == 10)
            {
                Projectile.width = 34;
                Projectile.height = 32;
            }
            else if (Projectile.ai[1] == 11)
            {
                Projectile.width = 18;
                Projectile.height = 12;
            }
            else
            {
                Projectile.width = 30;
                Projectile.height = 42;
            }
            return true;
        }


        public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.03f * Projectile.direction;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 20f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;
            }
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        /*public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 6;
        }*/

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 16;
            height = 16;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GibExplosion>(),
                    Projectile.damage, Projectile.knockBack * 2f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex;
            if (Projectile.ai[1] != 0)
            {
                tex = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Textures/Content/Projectiles/Weapons/BossWeapons/" + GetType().Name + Projectile.ai[1]).Value;
            }
            else
            {
                tex = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Textures/Content/Projectiles/Weapons/BossWeapons/" + GetType().Name + 1).Value;
            }
            FargoSoulsUtil.DrawTexture(Main.spriteBatch, tex, 0, Projectile, lightColor, true);

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 1;
            Projectile.hide = true;
            Projectile.ai[0] = 0f;
            Projectile.velocity *= 0;
            Projectile.knockBack *= 2f;
            Projectile.Resize(80, 80);;
        }
    }
}