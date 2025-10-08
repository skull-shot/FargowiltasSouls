using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Data;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops
{
    public class TwilightStarSpawn : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_79";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.light = 0.5f;
            Projectile.scale = 0.8f;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.97f;
                if (Projectile.ai[1] > 15)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = -1;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.velocity = 45 * Vector2.UnitX.RotatedBy((Main.MouseWorld - Projectile.Center).ToRotation());
                SoundEngine.PlaySound(SoundID.Item9 with { Pitch = -0.7f }, Projectile.Center);
                FargoSoulsUtil.DustRing(Projectile.Center, 20, DustID.GoldFlame, 2f, scale: 1.5f);
                Projectile.ai[0] = 2;
            }
            Projectile.rotation = MathHelper.Pi + Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold);
            SoundEngine.PlaySound(SoundID.Item110, Projectile.Center);
            base.OnKill(timeLeft);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FargoSoulsUtil.DustRing(target.Center, 20, DustID.GoldFlame, 3, scale: 1.5f);
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
                Vector2 origin2 = rectangle.Size() / 2f;
                SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Color origColor = Color.Gold;

                for (int k = 5; k >= 0; k--)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin2;
                    Color color = Projectile.GetAlpha(origColor) * ((5 - k) / ((float)5 * 2));
                    Main.EntitySpriteDraw(texture2D13, drawPos, rectangle, 
                        color, 0, origin2 / 0.8f, Projectile.scale, spriteEffects, 0);
                }

                int count = 6;
                for (int i = 0; i < count; i++)
                {
                    float opac = (i + 1) / count;
                    Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition, rectangle, 
                        origColor * opac, 0, origin2, opac * Projectile.scale, spriteEffects, 0);
                }
            }
            return false;
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, Color.Gold, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
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
