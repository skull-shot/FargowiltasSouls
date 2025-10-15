using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomCannonball : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.CannonballHostile);

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.CannonballHostile);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.timeLeft = 600;
            Projectile.scale = 2;
            Projectile.width /= 3;
            Projectile.height /= 3;
            Projectile.Opacity = 1f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        ref float Gravity => ref Projectile.ai[0];
        ref float CountdownTimer => ref Projectile.ai[1];
        ref float Behavior => ref Projectile.ai[2];
        ref float TimerStart => ref Projectile.localAI[0];

        public override bool? CanDamage()
        {
            //basically, it cant hurt you when destroyed by abom's p2 transition or death anim
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>())
                && Main.npc[EModeGlobalNPC.abomBoss].dontTakeDamage)
                return false;

            return Behavior == 1 || Projectile.timeLeft <= 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //circular hitbox
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= Projectile.width / 2;
        }

        public override void AI()
        {
            if (TimerStart == 0)
            {
                TimerStart = CountdownTimer;

                SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
            }

            if (--CountdownTimer <= 0)
            {
                Projectile.Kill();
                return;
            }

            Projectile.hide = false;

            Projectile.rotation += 0.8f;

            if (Behavior == 0)
            {
                Projectile.velocity.Y += Gravity;

                const float maxScale = 5f;
                Projectile.scale = maxScale * (1f - CountdownTimer / TimerStart);
                if (Projectile.scale < 0.1f)
                    Projectile.scale = 0.1f;
                if (Projectile.scale > maxScale)
                    Projectile.scale = maxScale;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                ScreenShakeSystem.StartShake(5, shakeStrengthDissipationIncrement: 10f / 30);

                Projectile.FargoSouls().GrazeCD = 0;

                Projectile.timeLeft = 0;
                
                const int ExplosionDiameter = 400;

                Projectile.scale = 1f;
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = ExplosionDiameter;
                Projectile.Center = Projectile.position;

                Projectile.Damage();

                float scaleFactor9 = 2;
                for (int j = 0; j < 5; j++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, (Vector2.UnitX * 5).RotatedByRandom(MathHelper.TwoPi), Main.rand.Next(61, 64), scaleFactor9);
                }

                for (int i = 0; i < 200; i++)
                {
                    Vector2 pos = Projectile.Center + new Vector2(0, Main.rand.NextFloat(ExplosionDiameter * 0.8f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)); //circle with highest density in middle
                    Vector2 vel = (pos - Projectile.Center) / 500;
                    Particle p = new ExpandingBloomParticle(pos, vel, Color.Lerp(Color.Yellow, Color.Red, pos.Distance(Projectile.Center) / (ExplosionDiameter / 2f)), startScale: Vector2.One * 3, endScale: Vector2.One * 0, lifetime: 60);
                    p.Velocity *= 2f;
                    p.Spawn();
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();

            target.AddBuff(BuffID.Bleeding, 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<AbomFangBuff>(), 240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Vector2 drawPos = Projectile.Center;
            drawPos += Projectile.rotation.ToRotationVector2() * Projectile.scale * 4f; //offset axis of rotation so spin is more obvious

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12).ToRotationVector2() * 4f * Projectile.scale;
                Color glowColor = Color.OrangeRed;
                float opacity = 1f;
                glowColor *= opacity;
                float scale = Projectile.scale;
                Main.EntitySpriteDraw(texture2D13, drawPos + afterimageOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, glowColor, Projectile.rotation, origin2, scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture2D13, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}