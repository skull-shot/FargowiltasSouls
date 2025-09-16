using FargowiltasSouls.Assets.Textures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class ExplosionVisual : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;


        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            AIType = ProjectileID.Bullet;
        }
        public ref float Radius => ref Projectile.ai[0];
        public ref float Duration => ref Projectile.ai[1];

        public ref float Timer => ref Projectile.localAI[0];
        public override bool PreAI()
        {
            if (++Timer >= Duration)
            {
                Projectile.Kill();
            }
            return base.PreAI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float progress = Timer / Duration;
            float radius = Radius * MathF.Pow(progress, 0.3f);

            Vector2 auraPos = Projectile.Center + new Vector2(0f, Projectile.gfxOffY);
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.HarshNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;
            var maxOpacity = MathHelper.Lerp(1, 0, MathF.Pow(progress, 2));

            Vector4 darkColor = Color.Lerp(Color.Red, Color.Black, 0.5f).ToVector4();
            Vector4 midColor = Color.OrangeRed.ToVector4();
            Vector4 brightColor = Color.Yellow.ToVector4();
            darkColor.W = 1;
            midColor.W = 1;
            brightColor.W = 1;
            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.ExplosionVisualShader");
            borderShader.TrySetParameter("radius", radius);
            borderShader.TrySetParameter("darkColor", darkColor);
            borderShader.TrySetParameter("midColor", midColor);
            borderShader.TrySetParameter("brightColor", brightColor);
            borderShader.TrySetParameter("anchorPoint", auraPos);
            borderShader.TrySetParameter("screenPosition", Main.screenPosition);
            borderShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            borderShader.TrySetParameter("maxOpacity", maxOpacity);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);

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