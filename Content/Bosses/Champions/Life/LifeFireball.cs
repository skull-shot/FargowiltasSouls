using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Life
{
    public class LifeFireball : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.CultistBossFireBall);

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Main.projFrames[Type] = Main.projFrames[ProjectileID.CultistBossFireBall];
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 100;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.scale = 1f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            }
            
            if (Main.rand.NextBool(3))
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch,
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity.X *= 0.5f;
                Main.dust[index2].velocity.Y *= 0.5f;
            }
            
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }

            if (--Projectile.ai[0] > 0)
            {
                float speed = Projectile.velocity.Length();
                speed += Projectile.ai[1];
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * speed;
            }
            else if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = Player.FindClosest(Projectile.Center, 0, 0);

                if (Projectile.ai[1] != -1 && Main.player[(int)Projectile.ai[1]].active && !Main.player[(int)Projectile.ai[1]].dead)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(Main.player[(int)Projectile.ai[1]].Center);
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.tileCollide = true;

                if (++Projectile.localAI[1] < 90) //accelerate
                {
                    Projectile.velocity *= 1.04f;
                }

                if (Projectile.localAI[1] < 120)
                {
                    float rotation = Projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[(int)Projectile.ai[1]].Center - Projectile.Center;
                    float targetAngle = vel.ToRotation();
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.025f));
                }

                /*if (Projectile.velocity.Y <= 0) //don't home upwards ever
                {
                    Projectile.velocity = Projectile.oldVelocity;
                    if (Projectile.localAI[1] < 90)
                        Projectile.velocity *= 1.04f;
                }*/
            }

            Projectile.rotation += 0.2f;
        }

        public override void OnKill(int timeLeft)
        {
            /*
            if (timeLeft > 0)
            {
                for (int i = 0; i < 5; i++) //drop greek fire
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-10, 0),
                              Main.rand.Next(326, 329), Projectile.damage / 4, 0f, Main.myPlayer);
                    }
                }
            }
            */

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 2; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default,
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(BuffID.CursedInferno, 120);
                target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 120);
            }
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 25);
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

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.Red * Projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12).ToRotationVector2() * 2f * Projectile.scale;
                Color glowColor = Color.Red;

                Main.EntitySpriteDraw(texture2D13, Projectile.Center + afterimageOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, glowColor, Projectile.rotation, origin2, Projectile.scale, effects);
            }
            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}