﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Eternity;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantSpearMagical : MutantSpearThrown
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantSpear_April" :
            FargoAssets.GetAssetString("Content/Projectiles/Weapons/FinalUpgrades", "Penetrator");

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = attackTime + 600 / flySpeed;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        const int attackTime = 120;
        const int flySpeed = 25;
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.localAI[1] == 0) //cosmetic rotation
                {
                    Projectile.rotation = MathHelper.TwoPi + Main.rand.NextFloat(MathHelper.TwoPi);
                    if (Main.rand.NextBool())
                        Projectile.rotation *= -1;
                }

                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.ai[1], 0.05f);

                if (++Projectile.localAI[1] > attackTime)
                {
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                    Projectile.ai[0] = 1f;
                    Projectile.velocity = flySpeed * Projectile.ai[1].ToRotationVector2();
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
                if (--Projectile.localAI[0] < 0)
                {
                    Projectile.localAI[0] = 4;
                    if (Projectile.ai[1] == 0 && FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MutantSphereSmall>(), Projectile.damage, 0f, Projectile.owner, Projectile.ai[0]);
                }
            }

            scaletimer++;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordMoonBlast>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.velocity.ToRotation(), 12);
            }
        }


        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.White * Projectile.Opacity;
            color.A = (byte)(255f * Math.Min(Projectile.localAI[1] / attackTime, 1f));
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0) //block the fancy trail draw
                return true;

            return base.PreDraw(ref lightColor);
        }
    }
}