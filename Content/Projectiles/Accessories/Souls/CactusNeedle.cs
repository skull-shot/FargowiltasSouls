using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class CactusNeedle : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RollingCactusSpike;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PineNeedleFriendly);
            Projectile.aiStyle = 336;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = true;

            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
            Projectile.width = Projectile.height = 5;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 1) // from cactus staff
            {
                Projectile.ai[1] = 0;
                for (int i = 0; i < 5; i++)
                {
                    Dust c = Dust.NewDustDirect(Projectile.Center - Projectile.velocity, Projectile.width, Projectile.height, DustID.JunglePlants, Scale: 1.1f);
                    c.noGravity = true;
                    c.velocity *= 3f;
                }
            }

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 1)
            {
                target.FargoSouls().Needled = true;
            }
            else if (Projectile.ai[1] == 1)
            {
                if (Main.rand.NextBool(3))
                    target.AddBuff(BuffID.Poisoned, Main.rand.Next(60, 120));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust c = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.JunglePlants, Scale: Main.rand.NextBool(2) ? 0.8f : 1f)];
                c.noGravity = true;
            }
        }
    }
}
