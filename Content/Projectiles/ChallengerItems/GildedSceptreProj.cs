using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class GildedSceptreProj : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/CursedCoffin/CoffinWaveShot";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.timeLeft = 60 * 8;
            Projectile.Opacity = 1;
        }
        public ref float Triggered => ref Projectile.ai[2];

        private bool foundTarget = false;

        private Vector2 originPos = Vector2.Zero;
        public override void AI()
        {
            if (Projectile.localAI[0] < 6)
            {
                Projectile.localAI[0]++;
                Projectile.scale = MathHelper.Lerp(0, 1, Projectile.localAI[0] / 6);
            }
            if (Triggered == 0)
            {
                if (!Projectile.owner.IsWithinBounds(Main.maxPlayers) || !Main.player[Projectile.owner].Alive() || Main.player[Projectile.owner].HeldItem.type != ModContent.ItemType<GildedSceptre>())
                {
                    Projectile.Kill();
                    return;
                }
                if (originPos == Vector2.Zero)
                    originPos = Projectile.Center;
                //Vector2 desiredPos = originPos + (MathF.Tau * (Projectile.ai[1] % 20) / 20).ToRotationVector2() * 6;
                //Projectile.velocity = desiredPos - Projectile.Center;
            }
            if (Triggered != 0) // home onto closest 
            {
                #region Targeting
                if (!foundTarget && Projectile.ai[0] == -1)
                {
                    // find target
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2000, true);
                    foundTarget = Projectile.ai[0] != -1;
                    Projectile.netUpdate = true;
                }
                Projectile.friendly = foundTarget;
                #endregion
                if (foundTarget && Projectile.ai[0] > 0)
                {
                    NPC npc = Main.npc[(int)Projectile.ai[0]];
                    if (npc.active && npc.CanBeChasedBy()) //target is still valid
                    {
                        Vector2 targetCenter = npc.Center;
                        float inertia = 15f;
                        float deadzone = 25f;
                        Vector2 vectorToIdlePosition = targetCenter - Projectile.Center;
                        float num = vectorToIdlePosition.Length();
                        if (num > deadzone)
                        {
                            vectorToIdlePosition.Normalize();
                            vectorToIdlePosition *= 16;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                        }
                        else if (Projectile.velocity == Vector2.Zero)
                        {
                            Projectile.velocity.X = -0.15f;
                            Projectile.velocity.Y = -0.05f;
                        }
                    }
                    else //we lost em boys
                    {
                        Projectile.ai[0] = -1;
                        foundTarget = false;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int index1 = 0; index1 < 40; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3));
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 60 * 4);
        }
        private static readonly Color GlowColor = new(224, 196, 252, 0);
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.owner != Main.myPlayer && Triggered == 0)
                return false;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            int height = texture.Height / Main.projFrames[Type];
            int y = height * Projectile.frame;
            Rectangle rectangle = new(0, y, texture.Width, height);
            Vector2 origin = rectangle.Size() / 2f;
            if (Triggered == 0)
            {
                for (int j = 0; j < 12; j++)
                {
                    float spinOffset = (Main.GameUpdateCount * 0.001f * j) % 12;
                    float magnitude = 1f + ((j % 5) * 1f * MathF.Sin(Main.GameUpdateCount * MathHelper.TwoPi / (10 + ((j - 6f) * 28f))));
                    Vector2 afterimageOffset = (MathHelper.TwoPi * (j + spinOffset) / 12f).ToRotationVector2() * magnitude * Projectile.scale;
                    Color glowColor = GlowColor;

                    Main.spriteBatch.Draw(texture, drawPos + afterimageOffset, rectangle, glowColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                }
            }

            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            if (completionRatio < 0.08f)
                return Color.Transparent;
            Color color = Color.Lerp(Color.Lerp(Color.MediumPurple, Color.DeepPink, 0.5f), GlowColor, 0.5f);
            if (Projectile.ai[1] != 0)
                color = Color.Lerp(color, Color.Red, 0.6f);
            return Color.Lerp(color, Projectile.GetAlpha(GlowColor with { A = 100 }) * 0.5f, completionRatio);

            //return Color.Lerp(GlowColor, Color.Transparent, completionRatio) * 0.7f;
        }


        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }
    }
}
