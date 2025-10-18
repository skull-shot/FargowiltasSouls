using System;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Dusts;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.BloodMoon
{
    public class BloodTrail : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetDefaults()
        {
            Projectile.width = 18*2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;

            Player target = null;
            int p = Player.FindClosest(Projectile.Center, 16, 16);
            if (p >= 0) target = Main.player[p];

            if (target != null && Projectile.Hitbox.Distance(target.Bottom) <= 16 && target.velocity.Y == 0)
            {
                target.velocity.X *= .8f;
                if (!WorldSavingSystem.MasochistModeReal)
                    FargoSoulsUtil.AddDebuffFixedDuration(target, BuffID.Bleeding, 600);
                else target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 600);
            }

            /*for (int b = 0; b < Main.maxProjectiles; b++)
            {
                Projectile proj = Main.projectile[b];
                if (proj.type == Projectile.type && proj.ai[0] == Projectile.ai[0])
                {
                    if (proj.ai[1] == 0) proj.ai[1] = 1;
                }
            }*/

            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VampireHeal, Scale: 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Vector2.Zero;
                Main.dust[d].velocity.Y -= 3;

                for (int i = 0; i < 2; i++)
                {
                    int b = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<BloodDust>());
                    Main.dust[b].scale = 1f;
                    Main.dust[b].noGravity = true;
                    Main.dust[b].velocity = Vector2.Zero;
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}