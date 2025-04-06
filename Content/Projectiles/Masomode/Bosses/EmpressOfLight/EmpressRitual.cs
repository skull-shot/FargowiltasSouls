using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode.Bosses.EmpressOfLight
{
    public class EmpressRitual : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_872";

        private const float realRotation = MathHelper.Pi / 180f;
        public float VisualScale = 0f;
        public int Timer = 0;

        public EmpressRitual() : base(realRotation, 600f, NPCID.HallowBoss, DustID.HallowedTorch, visualCount: 18) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hide = true;
        }
        protected override void Movement(NPC npc)
        {
            Projectile.velocity = Vector2.Zero;
            if (npc.TypeAlive(NPCID.HallowBoss))
            {
                Projectile.velocity = npc.GetGlobalNPC<EmpressofLight>().targetPos - Projectile.Center;
            }
            else
            {
                Projectile.Kill();
            }

        }
        public override void AI()
        {
            base.AI();
            float maxTime = Projectile.ai[2];
            float endTime = 30f;
            Projectile.scale = 1f;
            //NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], npcType);
            /*
            if (npc.TypeAlive(NPCID.HallowBoss))
            {
                if (npc.GetGlobalNPC<EmpressofLight>().Ritual)
                {
                    Timer = 0;
                }
                else
                {
                    if (Timer < maxTime - endTime)
                        Timer = (int)(maxTime - endTime);
                }
            }
            else
            {
                Projectile.Kill();
            }
            */
            Projectile.rotation += 1f;

            /*
            Player player = Main.LocalPlayer;
            if (player.active && !player.dead && !player.ghost)
            {
                float distance = player.Distance(Projectile.Center);
                if (distance > threshold && distance < threshold * 4f)
                {
                    
                }
            }
            */
            if (Projectile.Opacity >= 1f)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.TypeAlive(ProjectileID.FairyQueenLance) && p.Distance(Projectile.Center) > threshold + 5)
                    {
                        p.Kill();
                    }
                }
            }
            Timer++;
            if (Timer >= maxTime - endTime)
            {
                Projectile.Opacity = 1f - LumUtils.InverseLerp(maxTime - endTime, maxTime, Timer);
                if (Timer >= maxTime)
                {
                    Projectile.Kill();
                }
            }
            VisualScale = Projectile.Opacity;
        }
        public override bool CanHitPlayer(Player target) => base.CanHitPlayer(target);
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCsAndTiles.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 auraPos = Projectile.Center;

            float leeway = Projectile.width / 2 * Projectile.scale;
            leeway *= 0.75f;
            float radius = threshold - leeway;
            float scale = MathF.Sqrt(VisualScale) * Projectile.scale;

            bool enraged = NPC.ShouldEmpressBeEnraged();
            Color enragedColor = Color.White;
            if (enraged)
            {
                float lerpValue = Utils.GetLerpValue(0f, 60f, (int)Main.time, clamped: true);
                enragedColor = Color.Lerp(Color.White, Main.OurFavoriteColor, lerpValue);
            }

            // projectiles
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            float rotationTimer = -Timer * realRotation * 2;

            Vector2 starPos = Projectile.Center;
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int x = 0; x < visualCount; x++)
            {
                int framesX = 2;
                int frame = (Projectile.frame + x) % Main.projFrames[Projectile.type];
                int y3 = num156 * frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new(0, y3, texture2D13.Width / framesX, num156);
                Vector2 origin2 = rectangle.Size() / 2f;
                Color color26 = enraged ? enragedColor : Main.hslToRgb(((float)x / visualCount + 0.5f) % 1f, 1f, 0.5f); // empress color
                color26 *= Projectile.Opacity;

                Vector2 drawOffset = new Vector2(radius * scale, 0f).RotatedBy(rotationTimer);
                drawOffset = drawOffset.RotatedBy(2f * MathHelper.Pi / visualCount * x);

                float rotation = drawOffset.ToRotation();
                const int max = 4;
                for (int i = 0; i < max; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(max - i) / max;
                    Vector2 value4 = starPos + drawOffset.RotatedBy(rotationPerTick * -i);
                    float rot = rotation;
                    Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, rot, origin2, Projectile.scale, SpriteEffects.None, 0);
                }

                float finalRot = rotation;
                Main.EntitySpriteDraw(texture2D13, starPos + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, finalRot, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ResetToDefault();


            // base.PreDraw(ref lightColor);


            var target = Main.LocalPlayer;

            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargosTextureRegistry.ColorNoiseMap;

            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;

            var maxOpacity = Projectile.Opacity;
            if (enraged)
                maxOpacity *= 0.8f;

            float timerDiv = 60f;
            if (ClientConfig.Instance.PhotosensitivityMode)
            {
                timerDiv = 300f;
                diagonalNoise = FargosTextureRegistry.PerlinNoise;
            }

            Color darkColor = enraged ? enragedColor : Main.hslToRgb((Timer / timerDiv + 0.5f) % 1f, 1f, 0.5f); // empress color
            darkColor *= Projectile.Opacity;

            Color mediumColor = Color.Lerp(darkColor, Color.White, 0.5f);
            Color lightColor2 = Color.White;

            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.EmpressRitualShader");
            borderShader.TrySetParameter("colorMult", 7.35f);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            borderShader.TrySetParameter("radius", radius * scale);
            borderShader.TrySetParameter("anchorPoint", auraPos);
            borderShader.TrySetParameter("screenPosition", Main.screenPosition);
            borderShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            borderShader.TrySetParameter("playerPosition", target.Center);
            borderShader.TrySetParameter("maxOpacity", maxOpacity);
            borderShader.TrySetParameter("sensMode", enraged || ClientConfig.Instance.PhotosensitivityMode);

            borderShader.TrySetParameter("darkColor", darkColor.ToVector4());
            borderShader.TrySetParameter("midColor", mediumColor.ToVector4());
            borderShader.TrySetParameter("lightColor", lightColor2.ToVector4());

            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, borderShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}