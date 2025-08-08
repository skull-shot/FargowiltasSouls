using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Hell
{
    public class DemonFireball : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.DD2BetsyFireball);

        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DD2BetsyFireball);
            Projectile.width = Projectile.height = 70;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 5;
            Projectile.Opacity = 1f;
            Projectile.alpha = 0;
        }
        public ref float StartingY => ref Projectile.ai[2];
        public override void AI()
        {
            if (StartingY == 0)
                StartingY = Projectile.Center.Y;
            Projectile.velocity.Y += 0.6f;

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
            if (Projectile.velocity.Y > 0 && Projectile.Center.Y > StartingY)
                Projectile.tileCollide = true;
            // dust
            if (Main.rand.NextBool(3))
            {
                int num686 = Utils.SelectRandom(Main.rand, 226, 229);
                Vector2 center10 = Projectile.Center;
                Vector2 vector57 = new Vector2(-16f, 16f);
                vector57 = vector57;
                float num687 = 0.6f;
                vector57 += new Vector2(-16f, 16f);
                vector57 = vector57.RotatedBy(Projectile.rotation);
                int num688 = 4;
                int num689 = Dust.NewDust(center10 + vector57 + Vector2.One * -num688, num688 * 2, num688 * 2, num686, 0f, 0f, 100, default, num687);
                Dust dust2 = Main.dust[num689];
                dust2.velocity *= 0.1f;
                Main.dust[num689].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
            /*
            Vector2 auraPos = Projectile.Center;
            float radius = Projectile.width * Projectile.scale / 2;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.WavyNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;
            var maxOpacity = Projectile.Opacity;

            ManagedShader fireballShader = ShaderManager.GetShader("FargowiltasSouls.HellFireballShader");
            fireballShader.TrySetParameter("colorMult", 7.35f);
            fireballShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            fireballShader.TrySetParameter("radius", radius);
            fireballShader.TrySetParameter("anchorPoint", auraPos);
            fireballShader.TrySetParameter("screenPosition", Main.screenPosition);
            fireballShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            fireballShader.TrySetParameter("maxOpacity", maxOpacity);
            fireballShader.TrySetParameter("velocity", Projectile.velocity.SafeNormalize(Vector2.Zero));

            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, fireballShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
            */
        }
    }
}