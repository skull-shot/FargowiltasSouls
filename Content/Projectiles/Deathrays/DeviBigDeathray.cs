﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Buffs.Eternity;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;

namespace FargowiltasSouls.Content.Projectiles.Deathrays
{
	public class DeviBigDeathray : BaseDeathray, IPixelatedPrimitiveRenderer
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Deathrays", "DeviDeathray");

        public static List<Asset<Texture2D>> RingTextures =>
        [
            FargoAssets.DeviRingTexture,
            FargoAssets.DeviRing2Texture,
            FargoAssets.DeviRing3Texture,
            FargoAssets.DeviRing4Texture,
        ];

        public DeviBigDeathray() : base(180) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Love Ray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        float offsetFromDevi = 300;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //the laser is offset really far out in front of devi
            //add another thinner hitbox to fix that
            float num6 = 0f;
            Vector2 startPoint = Projectile.Center - Projectile.velocity * offsetFromDevi;
            float closeRangeHitboxWidthModifier = 0.5f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, startPoint + Projectile.velocity * Projectile.localAI[1], closeRangeHitboxWidthModifier * 22f * Projectile.scale * hitboxModifier, ref num6))
            {
                return true;
            }

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            if (!Main.dedServ && Main.LocalPlayer.active)
                FargoSoulsUtil.ScreenshakeRumble(6);
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<DeviBoss>());
            if (npc != null)
            {
                Projectile.Center = npc.Center + Projectile.velocity * offsetFromDevi + Main.rand.NextVector2Circular(20, 20);
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(FargosSoundRegistry.DeviDeathray, Projectile.Center);
            }
            float num801 = 17f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 5f * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();
            num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, DustID.CopperCoin, vector80.X, vector80.Y, 0, default, 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.CopperCoin, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            const int increment = 100;
            for (int i = 0; i < array3[0]; i += increment)
            {
                if (!Main.rand.NextBool(3))
                    continue;
                float offset = i + Main.rand.NextFloat(-increment, increment);
                if (offset < 0)
                    offset = 0;
                if (offset > array3[0])
                    offset = array3[0];
                int d = Dust.NewDust(Projectile.position + Projectile.velocity * offset,
                    Projectile.width, Projectile.height, DustID.GemAmethyst, 0f, 0f, 0, default, Main.rand.NextFloat(4f, 8f));
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity += Projectile.velocity * 0.5f;
                Main.dust[d].velocity *= Main.rand.NextFloat(12f, 24f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 2); //lots of defense down stack to make damage calc consistent
            target.AddBuff(BuffID.WitheredArmor, 2);
            target.AddBuff(BuffID.BrokenArmor, 2);
            target.AddBuff(ModContent.BuffType<HexedBuff>(), 120);
            target.FargoSouls().HexedInflictor = Projectile.GetSourceNPC().whoAmI;
            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 60 * 10);

            target.velocity.X = 0;
            target.velocity.Y = -0.4f;
        }

        public float WidthFunction(float trailInterpolant)
        {
            float baseWidth = Projectile.scale * Projectile.width;

            return baseWidth * 0.7f;
        }

        public static Color[] DeviColors => [new(216, 108, 224, 100), new(232, 140, 240, 100), new(224, 16, 216, 100), new(240, 220, 240, 100)];
        public static Color ColorFunction(float trailInterpolant)
        {
            float time = (float)(0.5 * (1 + Math.Sin(1.5f * Main.GlobalTimeWrappedHourly % 1)));
            float localInterpolant = (time + (1 - trailInterpolant)) / 2;
            Color color = Color.Lerp(Color.MediumVioletRed, Color.Purple, localInterpolant) * 2;
            color.A = 100;
            return color;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            // This should never happen, but just in case.
            if (Projectile.velocity == Vector2.Zero)
                return;

            if (Projectile.hide)
                return;

            ManagedShader laser = ShaderManager.GetShader("FargowiltasSouls.DeviTouhouDeathray");
            ManagedShader ring = ShaderManager.GetShader("FargowiltasSouls.DeviRing");

            // Get the laser end position.
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance;

            // Create 8 points that span across the draw distance from the projectile center.
            // This allows the drawing to be pushed back, which is needed due to the shader fading in at the start to avoid
            // sharp lines.
            Vector2 initialDrawPoint = Projectile.Center - Projectile.velocity * 300f;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Draw the background rings.
            DrawRings(baseDrawPoints, true, ring);

            #region MainLaser

            // Set shader parameters. This one takes two lots of fademaps and colors for two different overlayed textures.
            laser.TrySetParameter("mainColor", new Color(255, 180, 243, 100) * 2);
            // GameShaders.Misc["FargoswiltasSouls:MutantDeathray"].UseImage1(); cannot be used due to only accepting vanilla paths.
            FargoSoulsUtil.SetTexture1(FargoAssets.DeviBackStreak.Value);

            // Secondary fademap
            FargoSoulsUtil.SetTexture2(FargoAssets.DeviInnerStreak.Value);
            PrimitiveRenderer.RenderTrail(baseDrawPoints, new(WidthFunction, ColorFunction, Pixelate: true, Shader: laser), 50);
            #endregion

            // Draw the foreground rings.
            DrawRings(baseDrawPoints, false, ring);
        }

        public float RingWidthFunction(float trailInterpolant) => Projectile.scale * 4;

        public static Color RingColorFunction(float trailInterpolant)
        {
            Color color = Color.Lerp(Color.Blue, Color.Red, trailInterpolant) * 2;
            color.A = 100;
            return color;
        }

        private void DrawRings(Vector2[] baseDrawPoints, bool inBackground, ManagedShader ring)
        {
            Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
            velocity = velocity.RotatedBy(MathHelper.PiOver2) * 1250;

            Vector2 currentLaserPosition;
            int iterator = 0;
            // Get the first position

            // We want to create a ring on every base point on the trail.
            for (int i = 1; i <= baseDrawPoints.Length / 2; i++)
            {
                // The middle of the laser
                currentLaserPosition = baseDrawPoints[i];

                // This is to make the length of them shorter as they go along.
                float velocityScaler = MathHelper.Lerp(1.05f, 0.85f, (float)i / baseDrawPoints.Length);
                velocity *= velocityScaler;

                // Move the current position back by half the velocity, so we start drawing at the edge.
                currentLaserPosition -= velocity * 0.5f;

                Vector2[] ringDrawPoints = new Vector2[10];
                for (int j = 0; j < ringDrawPoints.Length; j++)
                {
                    ringDrawPoints[j] = Vector2.Lerp(currentLaserPosition, currentLaserPosition + velocity, (float)j / ringDrawPoints.Length);

                    // This basically simulates 3D. It isnt actually, but looks close enough.

                    // Get the mid point.
                    Vector2 middlePoint = currentLaserPosition + velocity / 2;
                    // Get the vector towards the mid point from the current position.
                    Vector2 currentDistanceToMiddle = middlePoint - ringDrawPoints[j];
                    // Get the vector towards the mid point from the original position.
                    Vector2 originalDistanceToMiddle = middlePoint - currentLaserPosition;

                    // Compare the distances. This gives a 0-1 based on how far it is from the middle.
                    float distanceInterpolant = currentDistanceToMiddle.Length() / originalDistanceToMiddle.Length();
                    float offsetStrength = MathHelper.SmoothStep(1f, 0f, distanceInterpolant);

                    // Offset the ring draw point.
                    if (inBackground)
                        ringDrawPoints[j] += Projectile.velocity * offsetStrength * 75f;
                    else
                        ringDrawPoints[j] -= Projectile.velocity * offsetStrength * 75f;
                }

                ring.TrySetParameter("mainColor", new Color(216, 108, 224, 100));
                FargoSoulsUtil.SetTexture1(RingTextures[iterator].Value);
                ring.TrySetParameter("stretchAmount", 0.2f);

                float scrollSpeed = MathHelper.Lerp(1f, 1.3f, 1 - i / (baseDrawPoints.Length / 2 - 1));
                ring.TrySetParameter("scrollSpeed", scrollSpeed);
                ring.TrySetParameter("reverseDirection", inBackground);
                float opacity = 1f;
                if (inBackground)
                    opacity = 0.5f;
                ring.TrySetParameter("opacity", opacity);
                PrimitiveRenderer.RenderTrail(ringDrawPoints, new(RingWidthFunction, RingColorFunction, Pixelate: true, Shader: ring), 30);
                iterator++;
            }
        }
    }
}