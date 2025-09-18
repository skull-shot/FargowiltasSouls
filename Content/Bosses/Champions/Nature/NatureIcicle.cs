using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Bosses.Champions.Nature
{
    public class NatureIcicle : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Souls/FrostIcicle";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nature Icicle");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;

            Projectile.scale = 1.5f;
            Projectile.hide = true;
            CooldownSlot = 1;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
                Projectile.rotation = Main.rand.NextFloat(0, (float)Math.PI * 2);
                Projectile.hide = false;
            }
            if (Projectile.ai[2] == 0) // register telegraph time and rotation
            {
                Projectile.ai[2] = Projectile.ai[0];
                float baseRot = WorldSavingSystem.MasochistModeReal ? 26 : 32;
                float rotation = MathHelper.ToRadians(baseRot) + Main.rand.NextFloat(MathHelper.ToRadians(30));
                if (Main.rand.NextBool())
                    rotation *= -1;
                Projectile.ai[1] = rotation;
            }

            if (--Projectile.ai[0] > 0)
            {
                Projectile.tileCollide = false;
                Projectile.rotation += Projectile.velocity.Length() * .1f * Projectile.localAI[0];
            }
            else if (Projectile.ai[0] == 0)
            {
                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(Main.player[p].Center) * 30;
                    Projectile.netUpdate = true;

                    Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1]);

                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                }
            }
            else
            {
                if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    Projectile.tileCollide = true;

                if (Projectile.velocity != Vector2.Zero)
                    Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0.0f, 0.0f, 0, new Color(), 1f);
                if (!Main.rand.NextBool(3))
                {
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity *= 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust2 = Main.dust[index2];
                    dust2.scale *= 1.75f;
                }
                else
                {
                    Dust dust = Main.dust[index2];
                    dust.scale *= 0.5f;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Chilled, 300);
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*
            float start = Projectile.ai[2];
            float end = 0;
            if (Projectile.ai[0] < start && Projectile.ai[0] > end)
            {
                int p = Player.FindClosest(Projectile.Center, 0, 0);
                
                if (p != -1)
                {
                    Vector2 dir = Projectile.SafeDirectionTo(Main.player[p].Center).RotatedBy(Projectile.ai[1]);

                    Asset<Texture2D> line = TextureAssets.Extra[178];
                    float opacity = LumUtils.InverseLerp(start, end, Projectile.ai[0]);
                    Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Color.Lerp(Color.LightBlue, Color.Blue, 0.25f) * opacity, dir.ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(2f, Projectile.scale * 4), SpriteEffects.None);
                }
            }
            */

            Texture2D texture = Projectile.GetTexture();
            Vector2 drawPos = Projectile.GetDrawPosition();
            Rectangle frame = Projectile.GetDefaultFrame();
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color trailColor = Projectile.GetAlpha(Color.LightSkyBlue) * 0.75f * 0.5f;
                trailColor *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 oldPos = Projectile.oldPos[i];
                float oldRot = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture, oldPos + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), frame, trailColor, oldRot, frame.Size() / 2, Projectile.scale, spriteEffects, 0);
            }

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12).ToRotationVector2() * 2f * Projectile.scale;
                Color glowColor = Color.LightSkyBlue;

                Main.EntitySpriteDraw(texture, drawPos + afterimageOffset, frame, Projectile.GetAlpha(glowColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects);
            }
            Main.spriteBatch.ResetToDefault();
            Main.EntitySpriteDraw(texture, drawPos, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects);
            return false;
        }
    }
}