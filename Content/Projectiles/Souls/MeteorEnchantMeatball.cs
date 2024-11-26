using FargowiltasSouls.Assets.ExtraTextures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class MeteorEnchantMeatball : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 60;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            //Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * 4;
            Projectile.tileCollide = false;
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.penetrate = 1;
            //Projectile.usesIDStaticNPCImmunity = true;
            //Projectile.idStaticNPCHitCooldown = 120;
            //Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.extraUpdates = 2;

            Projectile.scale = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Force != 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = 120;
                Projectile.height = 120;
                Projectile.Center = Projectile.position;
            }
        }
        public ref float Force => ref Projectile.ai[0];
        public override void AI()
        {
            if (Projectile.timeLeft < 60 * 2.5f)
                Projectile.tileCollide = true;

            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = Main.rand.NextBool() ? 1 : -1;

            Projectile.rotation += Projectile.localAI[1] * MathHelper.TwoPi / 24f;

            /*
            if (++Projectile.frameCounter > 4)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            */

            if (++Projectile.localAI[0] % 2 == 1)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y);
                Main.dust[d].noGravity = true;

                int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y);
                Main.dust[d2].noGravity = true;
            }

            if (Main.rand.NextBool(2))
            {
                float scaleFactor9 = 0.4f;
                int i = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + Main.rand.NextVector2FromRectangle(new(0, 0, Projectile.width, Projectile.height)) * 0.75f, default, Main.rand.Next(61, 64));
                Gore gore = Main.gore[i];
                gore.position -= Projectile.velocity * 4;
                gore.velocity *= scaleFactor9;
                gore.velocity = -Projectile.velocity * Main.rand.NextFloat(0.5f, 0.9f);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;
        public override void OnKill(int timeLeft)
        {
            ScreenShakeSystem.StartShake(2f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (FargoSoulsUtil.HostCheck)
            {
                int i = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<MeteorExplosion>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                float scale = Projectile.scale / 2f;
                if (i.IsWithinBounds(Main.maxProjectiles))
                {
                    Projectile p = Main.projectile[i];
                    p.position = p.Center;
                    p.scale *= scale;
                    p.width = (int)(p.width * scale);
                    p.height = (int)(p.height * scale);
                    p.Center = p.position;
                }
            }

            if (!Main.dedServ)
            {
                int gores = Force != 0 ? 10 : 5;
                for (int j = 0; j < gores; j++)
                {
                    int i = j % 5;
                    Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                    Vector2 vel = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(4, 8);
                    int type = i + 1;
                    if (!Main.dedServ)
                        Gore.NewGore(Projectile.GetSource_FromThis(), pos, vel, ModContent.Find<ModGore>(Mod.Name, $"MeteorGore{type}").Type, Projectile.scale);
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 60 * 5);
            target.AddBuff(BuffID.OnFire3, 60 * 5);
        }
        public static readonly Color OrangeColor = Color.Lerp(Color.OrangeRed, Color.Orange, 0.4f);
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.1f;
            float ratio = MathF.Pow(completionRatio, 1.5f);
            return MathHelper.SmoothStep(baseWidth, 25f * Projectile.scale, ratio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(OrangeColor, Color.SkyBlue, completionRatio);
            float opacity = 0.7f;
            return color * opacity;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawMeteor(Projectile, Texture, Force != 0, ref lightColor);
            return false;
        }
        public static void DrawMeteor(Projectile Projectile, string textureString, bool large, ref Color lightColor)
        {
            Vector2 normalizedVel = Projectile.velocity.SafeNormalize(Vector2.Zero);
            //draw projectile
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            if (large)
                texture = ModContent.Request<Texture2D>(textureString + "Wizard").Value;

            int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            //Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Height - Projectile.height) / 2;

            Color drawColor = Projectile.GetAlpha(lightColor);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D circle = FargosTextureRegistry.BloomTexture.Value;
            float circleScale = 0.35f * Projectile.scale;
            Vector2 circleOffset = normalizedVel * 4f * Projectile.scale;
            Main.EntitySpriteDraw(circle, Projectile.Center + circleOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, OrangeColor * 0.1f * Projectile.Opacity, Projectile.rotation, circle.Size() / 2f, circleScale, effects, 0);
            //glow
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            float count = 12f;
            Color glowColor = Color.Lerp(Color.White, OrangeColor, 1f) * (1 / (count * 0.1f)) * Projectile.Opacity;
            for (int j = 0; j < count; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / count).ToRotationVector2() * 4.5f;
                afterimageOffset += normalizedVel * 3f;

                Main.EntitySpriteDraw(texture, Projectile.Center + afterimageOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), glowColor, Projectile.rotation, origin2, Projectile.scale, effects, 0f);
            }
            Vector2 glowOffset = normalizedVel * 3.5f;
            Main.EntitySpriteDraw(texture, Projectile.Center + glowOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), OrangeColor * 1f * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, effects, 0f);

            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), drawColor, Projectile.rotation, origin2, Projectile.scale, effects, 0);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 3; i++)
            {
                Texture2D glowTexture = ModContent.Request<Texture2D>(textureString + "Glow").Value;
                Vector2 offset = normalizedVel * (i - 1) * 4;
                float glowScale = 1.12f * Projectile.scale * (large ? 1.5f : 1f);
                Rectangle glowRect = new(0, 0, glowTexture.Width, glowTexture.Height);
                Main.EntitySpriteDraw(glowTexture, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), glowRect, OrangeColor with { A = 160 } * Projectile.Opacity * 0.8f, normalizedVel.ToRotation() + MathHelper.PiOver2, glowTexture.Size() / 2, glowScale, effects, 0f);
            }
            Main.spriteBatch.ResetToDefault();
        }
    }
}
