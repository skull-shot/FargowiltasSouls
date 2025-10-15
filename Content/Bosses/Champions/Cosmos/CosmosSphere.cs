﻿using FargowiltasSouls.Assets.Particles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosSphere : ModProjectile, IPixelatedPrimitiveRenderer
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 240 * 2;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] <= 0;
        }
        public ref float Direction => ref Projectile.ai[2];
        public override void AI()
        {
            /*for (int i = 0; i < 2; i++)
            {
                float num = Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 vector2 = new Vector2(-Projectile.width * 0.65f * Projectile.scale, 0.0f).RotatedBy(num * 6.28318548202515, new Vector2()).RotatedBy(Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Vortex, -Projectile.velocity.X / 3f, -Projectile.velocity.Y / 3f, 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = Projectile.Center + vector2;
                Main.dust[index2].noGravity = true;
            }*/

            Projectile.rotation += 0.12f * (Projectile.velocity.Y / 6);

            if (Projectile.timeLeft % Projectile.MaxUpdates == 0) //once per tick
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 11;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }
                Projectile.scale = 1f - Projectile.alpha / 255f;

                if (--Projectile.ai[0] == 0)
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                }
                float telegraphTime = 26f;
                if (Projectile.ai[1] < telegraphTime && Projectile.ai[1] > 0)
                {
                    float lerper = 1 - Projectile.ai[1] / telegraphTime;
                    Projectile.velocity.Y = 30f / Projectile.MaxUpdates * MathF.Pow(lerper, 2) * Direction;
                }
                if (--Projectile.ai[1] == 0)
                {
                    Projectile.velocity.Y = 30f / Projectile.MaxUpdates * Direction;
                    Projectile.netUpdate = true;
                }

                NPC eridanus = FargoSoulsUtil.NPCExists(EModeGlobalNPC.championBoss, ModContent.NPCType<CosmosChampion>());
                if (/*Projectile.ai[0] < 0 &&*/ Projectile.ai[1] > 0 && eridanus != null && eridanus.HasValidTarget)
                {
                    float modifier = Projectile.ai[1] / 60f;
                    if (modifier < 0)
                        modifier = 0;
                    if (modifier > 1)
                        modifier = 1;
                    Projectile.position.Y += (Main.player[eridanus.target].position.Y - Main.player[eridanus.target].oldPosition.Y) * 0.6f * modifier;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 360);
        }

        public override void OnKill(int timeleft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 208;
            Projectile.Center = Projectile.position;
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 1f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index3].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 1f;
                Main.dust[index3].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = (Projectile.scale - 0.6f) * Projectile.width * 5f;
            float ratio = MathF.Pow(completionRatio, 1.5f);
            return MathHelper.SmoothStep(baseWidth, 7.7f, ratio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Blue, Color.SkyBlue, completionRatio);
            float opacity = 0.7f;
            return color * opacity;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargoAssets.FadedStreak.Value);
            if (!(Projectile.velocity.X > 0 || Projectile.velocity.X < 0) && Projectile.velocity.Y != 0)
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            /*for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }*/



            Vector2 normalizedVel = Projectile.velocity.SafeNormalize(Vector2.Zero);
            //draw projectile
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            Color drawColor = Projectile.GetAlpha(lightColor);

            Texture2D circle = FargoAssets.BloomTexture.Value;
            float circleScale = (0.35f * Projectile.scale) * 0.8f;
            Vector2 circleOffset = normalizedVel * 4f * Projectile.scale;
            Main.EntitySpriteDraw(circle, Projectile.Center + circleOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, Color.CornflowerBlue * 0.1f * Projectile.Opacity, Projectile.rotation, circle.Size() / 2f, circleScale, effects, 0);
            //glow
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            float count = 12f;
            Color glowColor = Color.Lerp(Color.White, Color.CornflowerBlue, 1f) * (1 / (count * 0.1f)) * Projectile.Opacity;
            for (int j = 0; j < count; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / count).ToRotationVector2() * 4.5f;
                afterimageOffset += normalizedVel * 3f;

                //if(!(Projectile.velocity.X > 0 || Projectile.velocity.X < 0) && Projectile.velocity.Y != 0)
                    Main.EntitySpriteDraw(texture, Projectile.Center + afterimageOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), glowColor, Projectile.rotation, origin2, Projectile.scale, effects, 0f);
            }
            Vector2 glowOffset = normalizedVel * 3.5f;
            Main.EntitySpriteDraw(texture, Projectile.Center + glowOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Color.CornflowerBlue * 1f * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, effects, 0f);

            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), drawColor, Projectile.rotation, origin2, Projectile.scale, effects, 0);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 3; i++)
            {
                Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "Glow").Value;
                Vector2 offset = normalizedVel * (i - 1) * 4;
                float glowScale = 1.12f * Projectile.scale;
                Rectangle glowRect = new(0, 0, glowTexture.Width, glowTexture.Height);
                if (!(Projectile.velocity.X > 0 || Projectile.velocity.X < 0) && Projectile.velocity.Y != 0)
                    Main.EntitySpriteDraw(glowTexture, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), glowRect, Color.CornflowerBlue with { A = 160 } * Projectile.Opacity * 0.8f, normalizedVel.ToRotation() + MathHelper.PiOver2, glowTexture.Size() / 2, glowScale, effects, 0f);
            }
            Main.spriteBatch.ResetToDefault();
                   
            //else
                //Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            
            return false;
        }

    }
}