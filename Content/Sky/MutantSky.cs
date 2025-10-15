using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Sky
{
    public class MutantSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private float lifeIntensity = 0f;
        private float shaderIntensity = 0f;
        private float specialColorLerp = 0f;
        private Color? specialColor = null;
        private int delay = 0;
        const int amountOfStatic = 200;
        private readonly int[] xPos = new int[amountOfStatic];
        private readonly int[] yPos = new int[amountOfStatic];

        public override void Update(GameTime gameTime)
        {
            const float increment = 0.01f;

            bool useSpecialColor = false;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())
                && (Main.npc[EModeGlobalNPC.mutantBoss].ai[0] < 0 || Main.npc[EModeGlobalNPC.mutantBoss].ai[0] >= 10))
            {
                intensity += increment;
                lifeIntensity = Main.npc[EModeGlobalNPC.mutantBoss].ai[0] < 0 ? 1f : 1f - (float)Main.npc[EModeGlobalNPC.mutantBoss].life / Main.npc[EModeGlobalNPC.mutantBoss].lifeMax;

                void ChangeColorIfDefault(Color color) //waits for bg to return to default first
                {
                    if (specialColor == null)
                        specialColor = color;
                    if (specialColor != null && specialColor == color)
                        useSpecialColor = true;
                }

                bool raiseShader = true;

                switch ((int)Main.npc[EModeGlobalNPC.mutantBoss].ai[0])
                {
                    case -5:
                        if (Main.npc[EModeGlobalNPC.mutantBoss].ai[2] >= 420)
                            ChangeColorIfDefault(FargoSoulsUtil.AprilFools ? new Color(255, 180, 50) : Color.Cyan);
                        break;

                    case 10: //p2 transition, smash to black
                        useSpecialColor = true;
                        specialColor = Color.Black;
                        specialColorLerp = 1f;
                        raiseShader = false;
                        break;

                    case 27: //ray fan
                        ChangeColorIfDefault(Color.Red);
                        break;

                    case 36: //slime rain
                        if (WorldSavingSystem.MasochistModeReal && Main.npc[EModeGlobalNPC.mutantBoss].ai[2] > 180 * 3 - 60)
                            ChangeColorIfDefault(Color.Blue);
                        break;

                    case 44: //empress
                        ChangeColorIfDefault(Color.DeepPink);
                        break;

                    case 48: //queen slime
                        ChangeColorIfDefault(Color.Purple);
                        break;

                    default:
                        break;
                }

                if (raiseShader)
                {
                    shaderIntensity += increment;
                    if (shaderIntensity > 1f)
                        shaderIntensity = 1f;
                }

                if (intensity > 1f)
                    intensity = 1f;
            }
            else
            {
                lifeIntensity -= increment;
                if (lifeIntensity < 0f)
                    lifeIntensity = 0f;

                shaderIntensity -= increment;
                if (shaderIntensity < 0f)
                    shaderIntensity = 0f;

                specialColorLerp -= increment * 2;
                if (specialColorLerp < 0)
                    specialColorLerp = 0;

                intensity -= increment;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    shaderIntensity = 0f;
                    lifeIntensity = 0f;
                    specialColorLerp = 0f;
                    specialColor = null;
                    delay = 0;
                    Deactivate();
                    return;
                }
            }

            if (useSpecialColor)
            {
                specialColorLerp += increment * 2;
                if (specialColorLerp > 1)
                    specialColorLerp = 1;
            }
            else
            {
                specialColorLerp -= increment * 2;
                if (specialColorLerp < 0)
                {
                    specialColorLerp = 0;
                    specialColor = null;
                }
            }
        }

        private Color ColorToUse(ref float opacity)
        {
            Color color = FargoSoulsUtil.AprilFools ? Color.OrangeRed : new(51, 255, 191);
            opacity = intensity * 0.5f + lifeIntensity * 0.5f;

            if (specialColorLerp > 0 && specialColor != null)
            {
                color = Color.Lerp(color, (Color)specialColor, specialColorLerp);
                if (specialColor == Color.Black)
                    opacity = System.Math.Min(1f, opacity + System.Math.Min(intensity, lifeIntensity) * 0.5f);
            }

            return color;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float opacity = 0f;
                Color color = ColorToUse(ref opacity);

                spriteBatch.Draw(ModContent.Request<Texture2D>($"FargowiltasSouls/Content/Sky/MutantSky{FargoSoulsUtil.TryAprilFoolsTexture}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), color * opacity);

                if (--delay < 0)
                {
                    delay = Main.rand.Next(5 + (int)(85f * (1f - lifeIntensity)));
                    for (int i = 0; i < amountOfStatic; i++) //update positions
                    {
                        xPos[i] = Main.rand.Next(Main.screenWidth);
                        yPos[i] = Main.rand.Next(Main.screenHeight);
                    }
                }

                Texture2D staticTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/MutantStatic", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                for (int i = 0; i < amountOfStatic; i++) //static on screen
                {
                    int width = Main.rand.Next(3, 251);
                    spriteBatch.Draw(staticTexture, new Rectangle(xPos[i] - width / 2, yPos[i], width, 3),
                    color * lifeIntensity * 0.75f);
                }

                Color vignetteColor = (FargoSoulsUtil.AprilFools ? Color.Red : Color.Blue) * shaderIntensity * 0.2f;
                spriteBatch.Draw(ModContent.Request<Texture2D>($"FargowiltasSouls/Content/Sky/MutantVignette", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), vignetteColor);

                var blackTile = TextureAssets.MagicPixel;
                var risingFlame = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/MutantFlame");
                if (!blackTile.IsLoaded || !risingFlame.IsLoaded)
                    return;

                ManagedShader wavyTvShader = ShaderManager.GetShader("FargowiltasSouls.MutantBackgroundShader");
                wavyTvShader.TrySetParameter("globalTime", Main.GlobalTimeWrappedHourly);
                wavyTvShader.TrySetParameter("screenPosition", Main.screenPosition);
                wavyTvShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
                wavyTvShader.TrySetParameter("scrollSpeed", opacity);
                wavyTvShader.TrySetParameter("opacity", shaderIntensity);

                Main.spriteBatch.GraphicsDevice.Textures[1] = risingFlame.Value;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, wavyTvShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                
                Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
                
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public override float GetCloudAlpha()
        {
            return 1f - intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override Color OnTileColor(Color inColor)
        {
            float dummy = 0f;
            Color skyColor = Color.Lerp(Color.White, ColorToUse(ref dummy), 0.5f);
            return Color.Lerp(skyColor, inColor, 1f - intensity);
        }
    }
}