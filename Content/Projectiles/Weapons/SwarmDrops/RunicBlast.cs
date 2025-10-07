using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops
{
    public class RunicBlast : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_675";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.97f;
                if (Projectile.ai[1] > 30)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = -1;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.velocity = 25 * Vector2.UnitX.RotatedBy((Main.MouseWorld - Projectile.Center).ToRotation());
                SoundEngine.PlaySound(SoundID.Item158 with { Pitch = -0.8f }, Projectile.Center);
                FargoSoulsUtil.DustRing(Projectile.Center, 20, DustID.PinkTorch, 2f, scale: 1.5f);
                Projectile.ai[0] = 2;
            }
            Projectile.rotation = MathHelper.Pi + Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch);
            SoundEngine.PlaySound(SoundID.Item110, Projectile.Center);
            base.OnKill(timeLeft);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FargoSoulsUtil.DustRing(target.Center, 20, DustID.PinkTorch, 3, scale: 1.5f);
            SoundEngine.PlaySound(SoundID.Item110, Projectile.Center);
            base.OnHitNPC(target, hit, damageDone);
        }

        #region Drawing
        public float transitToDark;

        private static VertexStrip _vertexStrip = new VertexStrip();

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 2)
            {
                MiscShaderData miscShaderData = GameShaders.Misc["MagicMissile"];
                miscShaderData.UseSaturation(-2.8f);
                miscShaderData.UseOpacity(2f);
                miscShaderData.Apply();
                _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + Projectile.Size / 2f);
                _vertexStrip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Type].Value;

                Rectangle rectangle = new(0, 0, texture2D13.Width, texture2D13.Height);
                Vector2 origin2 = texture2D13.Size() / 2f;
                SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                for (int k = 5; k >= 0; k--)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin2;
                    Color color = Projectile.GetAlpha(lightColor) * ((5 - k) / ((float)5 * 2));
                    Main.EntitySpriteDraw(texture2D13, drawPos, rectangle, color, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
                }

                FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            }
            return false;
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.Purple, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A /= 2;
            return result;
        }

        private float StripWidth(float progressOnStrip)
        {
            return Projectile.scale * 3 * MathHelper.Lerp(26f, 32f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
        }
        #endregion
    }
}
