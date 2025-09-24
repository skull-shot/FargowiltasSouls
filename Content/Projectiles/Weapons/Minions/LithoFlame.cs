using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class LithoFlame : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.SpiritFlame);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.SpiritFlame];
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.velocity *= 0.97f;
            if (Projectile.ai[0] >= 60)
            {
                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1200, true, true));
                if (n != null && n.CanBeChasedBy())
                {
                    if (Projectile.ai[2] < 50)
                        Projectile.ai[2]++;
                    Vector2 vel = Projectile.SafeDirectionTo(n.Center) * MathHelper.Lerp(10, Projectile.ai[2], 0.5f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, vel, 1f / 10f);
                }
                else Projectile.ai[2] = 0;
            }

            for (float d = 0f; d < 3f; d++) // tweaked vanilla spirit flame dust behavior
            {
                if (Main.rand.NextBool(3))
                {
                    Dust s = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, DustID.Shadowflame, 0f, -2f)];
                    s.position = Projectile.Center + Vector2.UnitY.RotatedBy(d * ((float)Math.PI * 2f) / 3f + Projectile.ai[0]) * 10f;
                    s.noGravity = true;
                    s.velocity = Projectile.DirectionFrom(s.position);
                    s.scale = Projectile.timeLeft < 60 ? 1.6f : Projectile.timeLeft < 120 ? 1.1f : 0.5f;
                    s.fadeIn = 0.5f;
                    s.alpha = 200;
                }
            }
            float lightFade = (255f - Projectile.alpha) / 255f;
            Lighting.AddLight(Projectile.Center, lightFade * 0.7f * Color.Plum.R / 255f, lightFade * 0.7f * Color.Plum.G / 255f, lightFade * 0.7f * Color.Plum.B / 255f);
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 120);
            SoundEngine.PlaySound(SoundID.Item14 with {MaxInstances = 2, Volume = 0.66f}, Projectile.Center);

            // more vanilla dust but like way toned down
            for (int i = 0; i < 14; i++)
            {
                float fadein = 1.5f;
                Dust s = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width * 10, Projectile.height * 10, DustID.Shadowflame, Scale: 2f + Main.rand.NextFloat() * 0.5f)];
                if (i > 7)
                {
                    s.velocity = Vector2.UnitY.RotatedBy(i + 1 / (7 * MathHelper.TwoPi));
                    s.position = Projectile.Center;
                }
                else
                {
                    fadein = 0.5f;
                    s.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * 80 / 3f;
                    s.velocity *= 2f;
                }
                s.noGravity = true;
                s.fadeIn = fadein + Main.rand.NextFloat() * 0.5f;
            }
            for (int i = 0; i < 5; i++)
            {
                Dust smoke = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, Scale: 1.5f)];
                smoke.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI).RotatedBy(Projectile.velocity.ToRotation()) * 80 / 3f;
                smoke.fadeIn = 0.5f + Main.rand.NextFloat() * 0.5f;
                smoke.noGravity = true;
                smoke.velocity *= 1.5f;
            }

            /*for (int i = 0; i < Main.rand.Next(4, 7); i++)
            {
                Vector2 velocity = Main.rand.NextFloat(1f, 4f) * Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Shadowflame);
                Main.dust[d].velocity = velocity;
            }*/
            Projectile.Kill();
        }
        public override bool? CanDamage() => Projectile.ai[0] >= 60;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X / 2;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y / 2;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft < 30)
                Projectile.alpha = (int)(255f - 255f * Projectile.timeLeft / 30f);
            FargoSoulsUtil.GenericProjectileDraw(Projectile, Color.White);
            return false;
        }
    }
}