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
        public const int TrailLength = 10;
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.DD2BetsyFireball);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DD2BetsyFireball);
            Projectile.width = Projectile.height = 62;
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
            /*
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
            */
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float radius = Projectile.width * Projectile.scale / 2;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.WavyNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;

            Vector2 auraPos = Projectile.Center;
            var maxOpacity = Projectile.Opacity * 0.7f;

            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.HellFireballShader");
            borderShader.TrySetParameter("colorMult", 7.35f);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            borderShader.TrySetParameter("radius", radius);
            borderShader.TrySetParameter("screenPosition", Main.screenPosition);
            borderShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());


            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);

            Vector2[] anchorPoints = new Vector2[ProjectileID.Sets.TrailCacheLength[Type]];
            float[] opacities = new float[ProjectileID.Sets.TrailCacheLength[Type]];

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                var oldOpacity = maxOpacity * (ProjectileID.Sets.TrailCacheLength[Type] - (float)i) / (float)ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldCenter = Projectile.oldPos[i] + Projectile.Size / 2;
                anchorPoints[i] = oldCenter;
                opacities[i] = oldOpacity;
            }

            borderShader.TrySetParameter("anchorPoints", anchorPoints);
            borderShader.TrySetParameter("opacities", opacities);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, borderShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}