﻿
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Deathrays
{
    public class DeviLightBeam : BaseDeathray, IPixelatedPrimitiveRenderer
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Deathrays", "AbomDeathray");

        public DeviLightBeam() : base(30 * 3) { }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.extraUpdates = 2;
        }

        public override bool? CanDamage()
        {
            return Projectile.localAI[0] > 6;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            /*int ai1 = (int)Projectile.ai[1];
            if (Main.npc[ai1].active && Main.npc[ai1].type == ModContent.NPCType<DeviBoss>())
            {
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + Projectile.velocity * 250 + Main.rand.NextVector2Circular(5, 5);
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
                SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                //SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
            }
            float num801 = 0.3f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 1f * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();
            num804 += Projectile.ai[0] / maxTime;
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
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            Projectile.position -= Projectile.velocity;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }

        public float WidthFunction(float _) => Projectile.width * Projectile.scale * 1.2f;

        public static Color ColorFunction(float _) => new(232, 216, 77, 100);

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            //LaserDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["FargowiltasSouls:GenericDeathray"]);

            //// Get the laser end position.
            //Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance;

            //// Create 8 points that span across the draw distance from the projectile center.
            //Vector2 initialDrawPoint = Projectile.Center;
            //Vector2[] baseDrawPoints = new Vector2[8];
            //for (int i = 0; i < baseDrawPoints.Length; i++)
            //    baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            //// Set shader parameters.
            //GameShaders.Misc["FargowiltasSouls:GenericDeathray"].UseColor(new Color(255, 255, 210));
            //GameShaders.Misc["FargowiltasSouls:GenericDeathray"].SetShaderTexture(FargosTextureRegistry.MutantStreak);
            //GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["stretchAmount"].SetValue(3);
            //GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["scrollSpeed"].SetValue(1f);
            //GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["uColorFadeScaler"].SetValue(0.8f);
            //GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["useFadeIn"].SetValue(true);


            //LaserDrawer.DrawPrims(baseDrawPoints.ToList(), -Main.screenPosition, 10);
        }
    }
}