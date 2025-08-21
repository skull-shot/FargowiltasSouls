using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Earth
{
    public class FlowerPetal : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public override string Texture => "Terraria/Images/Projectile_221";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flower Petal");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.alpha = 0;
            Projectile.hide = true;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0 && Projectile.timeLeft > 105)
                Projectile.timeLeft = 105;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.scale = Main.rand.NextFloat(1.5f, 2f);
                Projectile.frame = Main.rand.Next(3);
                Projectile.hide = false;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }

            if (++Projectile.localAI[1] > 30 && Projectile.localAI[1] < 100)
            {
                Projectile.velocity *= 1.06f;
            }

            Projectile.rotation += Projectile.velocity.X * 0.01f;
            Projectile.rotation += Projectile.velocity.X.NonZeroSign() * 0.17f;

            /*
            int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemAmethyst);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].scale *= 2f;
            Main.dust[dust].velocity *= 0.1f;
            */
            //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, Projectile.velocity.X, Projectile.velocity.Y);
            
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 0 && FargoSoulsUtil.HostCheck)
            {
                for (int i = -1; i <= 1; i++) //split
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(5) * i) / 2f,
                        Projectile.type, Projectile.damage, 0f, Main.myPlayer, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Projectile.GetTexture();
            Vector2 drawPos = Projectile.GetDrawPosition();
            Rectangle frame = Projectile.GetDefaultFrame();
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12).ToRotationVector2() * 2f * Projectile.scale;
                Color glowColor = Color.DeepPink;

                Main.EntitySpriteDraw(texture, drawPos + afterimageOffset, frame, Projectile.GetAlpha(glowColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects);
            }
            Main.spriteBatch.ResetToDefault();
            Main.EntitySpriteDraw(texture, drawPos, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects);
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.9f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DeepPink, Color.Transparent, completionRatio) * 0.7f;
        }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoAssets.FadedStreak.Value.SetTexture1();
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }
    }
}