﻿using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviEnergyHeart : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            CooldownSlot = ImmunityCooldownID.Bosses;

            Projectile.alpha = 150;
            Projectile.timeLeft = 80;

            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Siblings/Deviantt/DeviHeartThrow" + Main.rand.Next(1,3)), Projectile.Center);
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 5)
                    Projectile.frame = 1;
            }

            // Fade into 50 alpha from 150
            if (Projectile.alpha >= 60)
                Projectile.alpha -= 10;

            Projectile.rotation = Projectile.ai[0];
            Projectile.scale += 0.01f;

            float speed = Projectile.velocity.Length();
            speed += Projectile.ai[1];
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * speed;
        }

        public override void OnKill(int timeLeft)
        {
            FargoSoulsUtil.HeartDust(Projectile.Center, Projectile.rotation + MathHelper.PiOver2);
            SoundEngine.PlaySound(FargosSoundRegistry.DeviHeartExplosion with { MaxInstances = 0, Volume = 0.33f }, Projectile.Center);
            /*for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 0, default(Color), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 8f;
            }*/
            for (int i = 0; i < 5; i++)
            {
                // THESE DO NOT CURRENTLY WORK, DO NOT USE.

                //Vector2 velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 6f);
                //Particle heart = new HeartParticle(Projectile.Center, velocity, Color.HotPink, 60, 1f, 1f);
                //ParticleManager.SpawnParticle(heart);
            }
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<DeviBoss>()))
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.UnitX.RotatedBy(Projectile.rotation + (float)Math.PI / 2 * i),
                            ModContent.ProjectileType<DeviDeathray>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<HexedBuff>(), 120);
            target.FargoSouls().HexedInflictor = Projectile.GetSourceNPC().whoAmI;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}