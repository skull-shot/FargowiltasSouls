﻿using Fargowiltas.Content.NPCs;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosDeathray : BaseDeathray, IPixelatedPrimitiveRenderer
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Deathrays", "ShadowDeathray");
        public CosmosDeathray() : base(20, drawDistance: 3600) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.trap = true;
        }

        bool hasHit;

        public override bool? CanDamage()
        {
            return Projectile.scale >= 1f && !hasHit;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            /*if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<CosmosChampion>())
            {
                
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
                Vector2 spawnPos = Projectile.Center;
                if (Projectile.ai[0] == 0f)
                    spawnPos += Projectile.velocity * 3000;
                SoundEngine.PlaySound(SoundID.Item12, spawnPos);
                SoundEngine.PlaySound(SoundID.Item14, spawnPos);
            }
            float num801 = 1f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 1f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f + Projectile.ai[0];
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //Projectile.rotation = num804;
            //num804 += 1.57079637f;
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
                array3[i] = Projectile.ai[0] == 0f ? 6000f : 3000f;
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

            Projectile.position -= Projectile.velocity;

            if (Projectile.scale == 1f)
            {
                /*for (int i = 0; i < 150; i++)
                {
                    int d = Dust.NewDust(Projectile.position + Projectile.velocity * Main.rand.NextFloat(array3[0]),
                        Projectile.width, Projectile.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 6f;
                }*/

                Projectile.FargoSouls().GrazeCD = 0;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type == ModContent.NPCType<Deviantt>())
                modifiers.FinalDamage *= 4f;
            if (target.type == ModContent.NPCType<DeviBoss.DeviBoss>())
                modifiers.FinalDamage *= 12f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 360);
            hasHit = true;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public float WidthFunction(float trailInterpolant)
        {
            return 1.1f * Projectile.width * Projectile.scale / 2;
        }
        

        public static Color ColorFunction(float trailInterpolant)
        {
            Color color = Color.DarkMagenta;
            color.A = 100;
            return color;
        }
        public Vector2[] offsets = new Vector2[80];
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            if (Projectile.hide)
                return;

            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.EridanusDeathray");

            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * Projectile.localAI[1] * 1.1f;

            Vector2 initialDrawPoint = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitY) * 100;
            Vector2[] baseDrawPoints = new Vector2[80];
            for (int i = 0; i < baseDrawPoints.Length; i++)
            {
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));
                if (offsets[i] == Vector2.Zero)
                {
                    float dif = Projectile.localAI[1] * 1.1f / baseDrawPoints.Length;
                    offsets[i] = Main.rand.NextVector2Circular(dif * 0.5f, dif * 0.5f);
                }
                baseDrawPoints[i] += offsets[i];
            }
               

            FargoSoulsUtil.SetTexture1(FargoAssets.CracksNoise.Value);

            shader.TrySetParameter("laserDirection", Projectile.velocity.SafeNormalize(Vector2.UnitY));

            PrimitiveRenderer.RenderTrail(baseDrawPoints, new(WidthFunction, ColorFunction, Pixelate: true, Shader: shader), 80);

            Main.spriteBatch.ResetToDefault();
        }
    }
}