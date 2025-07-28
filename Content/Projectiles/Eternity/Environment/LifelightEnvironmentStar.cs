using FargowiltasSouls.Common.Graphics.Particles;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using FargowiltasSouls.Content.Bosses.Lifelight;

namespace FargowiltasSouls.Content.Projectiles.Masomode.Environment
{
    public class LifelightEnvironmentStar : LightningArc
    {
        // Kills the projectile above 0, so set it to a negative value.
        public ref float Timer => ref Projectile.ai[0];

        // The .whoAmI of the parent npc.
        public ref float ParentIndex => ref Projectile.ai[1];

        public override string Texture => "FargowiltasSouls/Assets/Effects/LifeStar";

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.alpha = 0;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.Read();
            base.ReceiveExtraAI(reader);
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
        private Color GetColor()
        {
            return Main.rand.NextFromCollection([Color.Cyan, Color.Goldenrod, Color.DeepPink]);
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Math.Abs(Timer);
                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 1.5f }, Projectile.Center);
                Projectile.netUpdate = true;
            }
            if (Timer == -5)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = (int)(150 * Projectile.scale);
                Projectile.Center = Projectile.position;

                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                SoundEngine.PlaySound(LifeChallenger.RuneSound1 with { PitchRange = (-0.6f, -0.4f), Volume = 0.5f }, Projectile.Center);

                for (int j = 0; j < 32; j++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                    Particle p = new SmallSparkle(
                        worldPosition: Projectile.Center + offset,
                        velocity: Vector2.Zero,
                        drawColor: GetColor(),
                        scale: 1f,
                        lifetime: Main.rand.Next(20, 80),
                        rotation: 0,
                        rotationSpeed: Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)
                        );
                    p.Spawn();
                }
            }
            if (Timer > 0f)
                Projectile.Kill();
            else if (Timer > -5)
            {
                Projectile.Opacity = 0f;
            }

            // Ramp up the scale and rotation over time
            float ratio = 1f - Math.Abs(Timer) / Projectile.localAI[0];
            float rampupVfx = (float)Math.Sin(MathHelper.PiOver2 * ratio);
            Projectile.scale = 0.1f + 1.4f * rampupVfx;
            Projectile.scale *= Main.rand.NextFloat(0.8f, 1.2f);
            Projectile.rotation = 2f * MathHelper.TwoPi * rampupVfx;

            Timer++;
        }

        // Telegraphs should not deal damage.
        public override bool? CanDamage() => Timer > -5 && Timer < 0 ? null : false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.townNPC)
                return;
            modifiers.SourceDamage *= 10;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.HotPink;
            color.A = 50;
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = texture.Size() / 2f;

            for (int i = 0; i < 3; i++)
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}