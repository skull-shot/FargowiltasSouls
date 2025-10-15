﻿using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomFlamePillar : BaseDeathray, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Deathrays", "AbomDeathray");
        public AbomFlamePillar() : base(240, drawDistance: 6000) { }
        private Vector2 spawnPos;
        public bool fadeStart = false;

        public override void AI()
        {
            if (!Main.dedServ && Main.LocalPlayer.active)
                FargoSoulsUtil.ScreenshakeRumble(5);

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            int npcID = (int)Projectile.ai[1];
            if (npcID.IsWithinBounds(Main.maxNPCs) && Main.npc[npcID] is NPC npc && npc.TypeAlive<AbomBoss>())
            {
                Projectile closestDeathray = Main.projectile.Where(p => p.TypeAlive<AbomDeathray>()).OrderBy(p => p.Distance(npc.Center)).FirstOrDefault();
                if (closestDeathray != null)
                {
                    float spd = MathF.Min(18, MathF.Abs(closestDeathray.Center.X - Projectile.Center.X));
                    Vector2 vel = spd * Vector2.UnitX * Projectile.HorizontalDirectionTo(closestDeathray.Center);
                    Projectile.Center += vel;
                    spawnPos += vel;
                }
            }
            /*if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == ModContent.\1Type<\2>\(\))
            {
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center;
            }
            else
            {
                Projectile.Kill();
                return;
            }*/
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(FargosSoundRegistry.GenericDeathray, Projectile.Center);
                spawnPos = Projectile.Center;
            }
            else //vibrate beam
            {
                Projectile.Center = spawnPos + Main.rand.NextVector2Circular(5, 5);
            }

            float num801 = 5f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * num801 * 6f;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }

            if (Projectile.localAI[0] > maxTime / 2 && Projectile.scale < num801)
            {
                if (Projectile.ai[0] > 0)
                {
                    /*
                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = Main.rand.Next(150); i < 3000; i += 300)
                        {
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Projectile.velocity * i, Vector2.Zero,
                                ModContent.ProjectileType<AbomScytheSplit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], -1f);
                        }
                    }
                    */
                    Projectile.ai[0] = 0;
                }
            }

            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f;
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //Projectile.rotation = num804;
            //num804 += 1.57079637f;
            //Projectile.velocity = num804.ToRotationVector2();
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
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.CopperCoin, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            Projectile.position -= Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<AbomFangBuff>(), 240);
        }

        public float WidthFunction(float _) => Projectile.width * Projectile.scale;

        public static Color ColorFunction(float _) => new(253, 254, 32, 100);

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
            //DrawFlamePillar(Projectile, drawDistance, WidthFunction, fadeStart);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            List<Vector2> laserPositions = Projectile.GetLaserControlPoints(12, drawDistance);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.FlamePillarShader");
            shader.TrySetParameter("laserDirection", Projectile.velocity);
            shader.TrySetParameter("screenPosition", Main.screenPosition);
            shader.SetTexture(FargoAssets.WavyNoise.Value, 1, SamplerState.LinearWrap);

            PrimitiveSettings laserSettings = new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader);
            PrimitiveRenderer.RenderTrail(laserPositions, laserSettings, 60);
            Main.spriteBatch.ResetToDefault();
        }

        public static void DrawFlamePillar(Projectile projectile, float drawDistance, PrimitiveSettings.VertexWidthFunction widthFunction, bool fadeStart = false)
        {
            // This should never happen, but just in case.
            if (projectile.velocity == Vector2.Zero)
                return;

            Main.spriteBatch.PrepareForShaders();

            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.FlamePillarShader");

            Vector2 direction = projectile.velocity.SafeNormalize(Vector2.UnitY);
            Vector2 offset = direction;

            // Get the laser positions.
            Vector2 laserStartOffset = direction * -176 * projectile.scale;
            Vector2 laserStart = projectile.Center + offset * 2 + laserStartOffset;
            Vector2 laserEnd = laserStart + direction * drawDistance;

            // Create 8 points that span across the draw distance from the projectile center.

            // This allows the drawing to be pushed back, which is needed due to the shader fading in at the start to avoid
            // sharp lines.
            Vector2 initialDrawPoint = laserStart;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Set shader parameters. This one takes a fademap and a color.

            // The laser should fade to this in the middle.
            Color brightColor = AbomSword.lightColor;
            shader.TrySetParameter("mainColor", brightColor);
            shader.TrySetParameter("fadeStart", fadeStart);

            // GameShaders.Misc["FargoswiltasSouls:MutantDeathray"].UseImage1(); cannot be used due to only accepting vanilla paths.
            Texture2D fademap = FargoAssets.DeviBackStreak.Value;
            FargoSoulsUtil.SetTexture1(fademap);
            for (int j = 0; j < 2; j++)
            {
                PrimitiveSettings primSettings = new(widthFunction, ColorFunction, Shader: shader);
                PrimitiveRenderer.RenderTrail(baseDrawPoints, primSettings, 30);
                /*
                for (int i = 0; i < baseDrawPoints.Length / 2; i++)
                {
                    Vector2 temp = baseDrawPoints[i];
                    int swap = baseDrawPoints.Length - 1 - i;
                    baseDrawPoints[i] = baseDrawPoints[swap];
                    baseDrawPoints[swap] = temp;
                }
                PrimitiveRenderer.RenderTrail(baseDrawPoints, primSettings, 30);
                */
            }

            Main.spriteBatch.ResetToDefault();
        }
    }
}