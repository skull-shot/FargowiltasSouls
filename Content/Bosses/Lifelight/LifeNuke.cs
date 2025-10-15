using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{
    public class LifeNuke : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Bosses/Lifelight", Name);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 80;

            Projectile.scale = 1f;
            Projectile.Opacity = 0.5f;
        }

        public override bool? CanDamage() => false; // WorldSavingSystem.MasochistModeReal;

        public override void AI()
        {

            if (Projectile.timeLeft < 20)
            {
                float interpolant = 1f - (Projectile.timeLeft / 20f);
                Projectile.position -= Projectile.velocity * interpolant;
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 3f, 0.1f);
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.1f);
            }
                
            Projectile.rotation += Projectile.velocity.Length() * 0.075f * Math.Sign(Projectile.velocity.X);
            Projectile.alpha = (int)(150 * Math.Sin(++Projectile.localAI[0] / 3));

            /*
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, Scale: 3f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }
            */
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<SmiteBuff>(), 60 * 4);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            int max = (int)Projectile.ai[0];
            for (int i = 0; i < max; i++)
            {
                float rad = MathHelper.TwoPi / max * i;
                int damage = Projectile.damage;
                int knockBack = 3;
                float speed = 0.8f;
                if (Projectile.ai[2] != 0)
                    speed *= Projectile.ai[2];
                Vector2 vector = Projectile.velocity.RotatedBy(rad) * speed;
                if (FargoSoulsUtil.HostCheck)
                {
                    bool useSplitProj = Projectile.ai[1] != 0;
                    int type = useSplitProj ? ModContent.ProjectileType<LifeSplittingProjSmall>() : ModContent.ProjectileType<LifeProjSmall>();
                    float ai0 = useSplitProj ? -180 : 0;
                    float ai1 = useSplitProj ? 2 : 0;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vector, type, damage, knockBack, Main.myPlayer, ai0, ai1);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].hostile = Projectile.hostile;
                        Main.projectile[p].friendly = Projectile.friendly;
                    }
                }
            }


            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemDiamond, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
                Main.dust[dust].noGravity = true;
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.GemDiamond, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.GemDiamond, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default,
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity += new Vector2(1f, 1f).RotatedBy(MathHelper.TwoPi / 4 * j);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);


            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.HotPink * Projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }
            Vector2 offset = Vector2.Zero;
            Color drawColor = Projectile.GetAlpha(lightColor);
            if (Projectile.timeLeft < 20)
            {
                float interpolant = 1f - (Projectile.timeLeft / 20f);
                offset = Main.rand.NextVector2Circular(10, 10) * interpolant;
                //drawColor = Color.Lerp(drawColor, Color.White, interpolant);
            }
                
            Main.EntitySpriteDraw(texture2D13, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), drawColor, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            Main.spriteBatch.ResetToDefault();
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity;
    }
}
