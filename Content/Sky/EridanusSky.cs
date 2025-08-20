using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Core.Globals;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Sky
{
    public class EridanusSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;

        public override void Update(GameTime gameTime)
        {
            const float increment = 0.01f;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<CosmosChampion>()))
            {
                intensity += increment;
                if (intensity > 1f)
                {
                    intensity = 1f;
                }
            }
            else
            {
                intensity -= increment;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                NPC eridanus = FargoSoulsUtil.NPCExists(EModeGlobalNPC.championBoss, ModContent.NPCType<CosmosChampion>());
                if (eridanus != null)
                {
                    float radius = MathHelper.SmoothStep(0, 4500, intensity);
                    Vector2 auraPos = eridanus.Center;
                    var target = Main.LocalPlayer;

                    var blackTile = TextureAssets.MagicPixel;
                    var noise = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/deepspace");
                    if (!blackTile.IsLoaded)
                        return;
                    if (!noise.IsLoaded)
                        return;

                    ManagedShader blackShader = ShaderManager.GetShader("FargowiltasSouls.EridanusBackgroundShader");
                    blackShader.TrySetParameter("radius", radius);
                    blackShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
                    blackShader.TrySetParameter("anchorPoint", auraPos);
                    blackShader.TrySetParameter("screenPosition", Main.screenPosition);
                    blackShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
                    blackShader.TrySetParameter("maxOpacity", intensity);

                    Main.spriteBatch.GraphicsDevice.Textures[1] = noise.Value;

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, blackShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
                    spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
                    spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
                //spriteBatch.Draw(ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/AbomSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity * 0.75f);
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
            return new Color(Vector4.Lerp(new Vector4(1f, 0.9f, 0.6f, 1f), inColor.ToVector4(), 1f - intensity));
        }
    }
}