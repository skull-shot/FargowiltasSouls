﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Phupperbat
{
    public class ChibiiRemii : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chibii Remii");
            Main.projFrames[Projectile.type] = 11;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BlackCat);
            AIType = ProjectileID.BlackCat;
            Projectile.width = 34;
            Projectile.height = 44;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].blackCat = false; // Relic from AIType
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();

            if (!player.active || player.dead || player.ghost)
            {
                modPlayer.ChibiiRemii = false;
            }
            
            if (modPlayer.ChibiiRemii)
            {
                Projectile.timeLeft = 2;
            }

            if (Projectile.tileCollide) //walking
            {
                if (Projectile.velocity.X == 0)
                    Projectile.direction = System.Math.Sign(player.Center.X - Projectile.Center.X);

                if (player.velocity.X == 0 && System.Math.Abs(player.Bottom.Y - Projectile.Bottom.Y) < 16 * 2 && System.Math.Abs(player.Center.X - Projectile.Center.X) < 16 * 3)
                    Projectile.velocity.X += 0.1f * System.Math.Sign(Projectile.Center.X - player.Center.X);

                if (!Collision.SolidCollision(Projectile.position + Projectile.velocity.X * Vector2.UnitX, Projectile.width, Projectile.height))
                    Projectile.position.X += Projectile.velocity.X;
                if (Projectile.velocity.Y < 0)
                    Projectile.position.Y += Projectile.velocity.Y;

                if (Projectile.velocity.X == 0)
                {
                    Projectile.frame = 0;
                }
                else if (Projectile.frameCounter == 0)
                {
                    Projectile.frame++;
                }

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            else //flying
            {
                Projectile.Center += Projectile.velocity;

                if (++Projectile.frameCounter > 5)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }

                const int firstFlyFrame = 7;
                if (Projectile.frame < firstFlyFrame || Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = firstFlyFrame;
            }

            //Projectile.spriteDirection = Math.Sign(Projectile.velocity.X == 0 ? player.Center.X - Projectile.Center.X : Projectile.velocity.X);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Projectile.owner].Center.Y > Projectile.Bottom.Y;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.vampireHeal(1, Projectile.Center, target);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = Color.Red * Projectile.Opacity;
            color26.A = 20;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.25f)
            {
                Color color27 = color26 * 0.3f;
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            //float scale = Projectile.scale * (Main.mouseTextColor / 200f - 0.35f) * 0.3f + 1f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale * 1.2f, effects, 0);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}