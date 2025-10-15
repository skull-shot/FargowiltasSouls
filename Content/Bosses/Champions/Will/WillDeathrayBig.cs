﻿

using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Will
{
    public class WillDeathrayBig : BaseDeathray
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Will/WillDeathray";

        public WillDeathrayBig() : base(20, hitboxModifier: WorldSavingSystem.MasochistModeReal ? 1f : 0.5f, drawDistance: 3600, sheeting: TextureSheeting.Horizontal) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Will Deathray");
            Main.projFrames[Projectile.type] = 5;
        }

        public override bool? CanDamage()
        {
            return Projectile.scale == 10f;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            //NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<WillChampion>());
            //if (npc == null)
            //{
            //    Projectile.Kill();
            //    return;
            //}
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(FargosSoundRegistry.Zombie104, new Vector2(Projectile.Center.X, Main.LocalPlayer.Center.Y));
            }
            float num801 = 10f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 1.5f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            float num804 = Projectile.velocity.ToRotation() - 1.57079637f; //npc.ai[3] - 1.57079637f + Projectile.ai[0];
                                                                           //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            Projectile.rotation = num804;
            num804 += 1.57079637f;
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
                array3[i] = 4000f;
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
                int num812 = Dust.NewDust(vector79, 0, 0, DustID.GoldFlame, vector80.X, vector80.Y, 0, default, 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.GoldFlame, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            Projectile.position -= Projectile.velocity;

            if (Main.LocalPlayer.active && !Main.dedServ)
            {
                FargoSoulsUtil.ScreenshakeRumble(5);

                if (Projectile.localAI[0] < maxTime / 2)
                {
                    const int increment = 100;
                    for (int i = 0; i < array3[0]; i += increment)
                    {
                        float offset = i + Main.rand.NextFloat(-increment, increment);
                        Vector2 spawnPos = Projectile.position + Projectile.velocity * offset;
                        if (Math.Abs(spawnPos.Y - Main.LocalPlayer.Center.Y) > Main.screenHeight * 0.75f)
                            continue;
                        int d = Dust.NewDust(spawnPos,
                            Projectile.width, Projectile.height, DustID.GoldFlame, 0f, 0f, 0, default, 6f);
                        Main.dust[d].noGravity = Main.rand.NextBool();
                        Main.dust[d].velocity += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-6f, 6f);
                        Main.dust[d].velocity *= Main.rand.NextFloat(1f, 3f);
                    }
                }
            }

            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
                target.AddBuff(ModContent.BuffType<MidasBuff>(), 300);
            }
            target.AddBuff(BuffID.Bleeding, 300);
        }

        public float WidthFunction(float _) => Projectile.width * Projectile.scale * (WorldSavingSystem.MasochistModeReal ? 2 : 1);

        public static Color ColorFunction(float _) => new(253, 254, 32, 100);

        public override bool PreDraw(ref Color lightColor)
        {
            // This should never happen, but just in case.
            if (Projectile.velocity == Vector2.Zero)
                return false;

            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.WillBigDeathray");

            // Get the laser end position.
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance;

            // Create 8 points that span across the draw distance from the projectile center.

            // This allows the drawing to be pushed back, which is needed due to the shader fading in at the start to avoid
            // sharp lines.
            Vector2 initialDrawPoint = Projectile.Center - Projectile.velocity * 150f;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Set shader parameters. This one takes a fademap and a color.

            // The laser should fade to this in the middle.
            Color brightColor = new(252, 252, 192, 100);
            shader.TrySetParameter("mainColor", brightColor);
            // GameShaders.Misc["FargoswiltasSouls:MutantDeathray"].UseImage1(); cannot be used due to only accepting vanilla paths.
            Texture2D fademap = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Textures/Trails/WillStreak").Value;
            FargoSoulsUtil.SetTexture1(fademap);

            PrimitiveRenderer.RenderTrail(baseDrawPoints, new(WidthFunction, ColorFunction, Shader: shader), 30);
            return false;
        }
    }
}