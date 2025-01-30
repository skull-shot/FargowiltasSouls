using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Assets.ExtraTextures;
using Luminance.Core.Graphics;
using Terraria.GameContent;
using Fargowiltas.Common.Configs;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;
using System.Text.RegularExpressions;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using Terraria.DataStructures;

namespace FargowiltasSouls.Content.Projectiles
{
    public class RockeaterAuraProj : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public ref float DrawTime => ref Projectile.ai[2];

        public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity; // Color.White * Projectile.Opacity;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.Alive() || !CheckActive(player))
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = player.Center;
            Projectile.timeLeft = 60;
        }
        public bool CheckActive(Player player)
        {
            if (DrawTime > 0)
                DrawTime--;
            if (player == null || !player.Alive() || player.whoAmI != Main.myPlayer)
                return false;
            if (player.HeldItem == null || (player.HeldItem.type != ModContent.ItemType<EaterLauncher>() && player.HeldItem.type != ModContent.ItemType<EaterLauncherJr>()))
                return false;
            if (player.itemTime <= 0)
                return true;
            if (DrawTime < 15)
                DrawTime += 2;
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return false;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.Alive())
            {
                Projectile.Kill();
                return false;
            }
            if (player.whoAmI != Main.myPlayer)
                return false;

            Vector2 auraPos = Projectile.Center;

            Color darkColor = Color.DarkMagenta;
            Color mediumColor = Color.Magenta;

            float innerRadius = 300;
            float outerRadius = player.FargoSouls().RockeaterDistance;

            Vector2 nearbyPosition = auraPos.DirectionTo(Main.MouseWorld); // * (innerRadius + outerRadius) / 2;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargosTextureRegistry.WavyNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;
            var maxOpacity = 0.7f * ModContent.GetInstance<FargoClientConfig>().TransparentFriendlyProjectiles;
            maxOpacity *= (float)DrawTime / 15f;

            ManagedShader innerShader = ShaderManager.GetShader("FargowiltasSouls.RockeaterOuterAura");
            innerShader.TrySetParameter("colorMult", 7.35f);
            innerShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            innerShader.TrySetParameter("radius", innerRadius);
            innerShader.TrySetParameter("anchorPoint", auraPos);
            innerShader.TrySetParameter("screenPosition", Main.screenPosition);
            innerShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            innerShader.TrySetParameter("nearbyPosition", nearbyPosition);
            innerShader.TrySetParameter("maxOpacity", maxOpacity);
            innerShader.TrySetParameter("darkColor", darkColor.ToVector4());
            innerShader.TrySetParameter("midColor", mediumColor.ToVector4());

            ManagedShader outerShader = ShaderManager.GetShader("FargowiltasSouls.RockeaterInnerAura");
            outerShader.TrySetParameter("colorMult", 7.35f);
            outerShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            outerShader.TrySetParameter("radius", outerRadius);
            outerShader.TrySetParameter("anchorPoint", auraPos);
            outerShader.TrySetParameter("screenPosition", Main.screenPosition);
            outerShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            outerShader.TrySetParameter("nearbyPosition", nearbyPosition);
            outerShader.TrySetParameter("maxOpacity", maxOpacity);
            outerShader.TrySetParameter("darkColor", darkColor.ToVector4());
            outerShader.TrySetParameter("midColor", mediumColor.ToVector4());

            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, innerShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, outerShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
