using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class FallenStarDay : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_12";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FallingStar);
        }

        //fallen star ai
        public override void AI()
        {
            if (Projectile.ai[1] == 0f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[1] = 1f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[1] != 0f)
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
            }
            Projectile.alpha += (int)(25f * Projectile.localAI[0]);
            if (Projectile.alpha > 200)
            {
                Projectile.alpha = 200;
                Projectile.localAI[0] = -1f;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
                Projectile.localAI[0] = 1f;
            }


            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Vector2 vector10 = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector10 / 2f, vector10 + new Vector2(400f))) && Main.rand.Next(6) == 0)
            {
                int num87 = Utils.SelectRandom<int>(Main.rand, 16, 17, 17, 17);
                if (Main.tenthAnniversaryWorld)
                {
                    num87 = Utils.SelectRandom<int>(Main.rand, 16, 16, 16, 17);
                }
                Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity * 0.2f, num87);
            }
            Projectile.light = 0.9f;
            if (Main.rand.Next(20) == 0 || (Main.tenthAnniversaryWorld && Main.rand.Next(15) == 0))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
            }


        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Color newColor8 = Color.CornflowerBlue;
            if (Main.tenthAnniversaryWorld)
            {
                newColor8 = Color.HotPink;
                newColor8.A /= 2;
            }
            for (int num645 = 0; num645 < 7; num645++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float num646 = 0f; num646 < 1f; num646 += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num646 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, newColor8).noGravity = true;
            }
            for (float num647 = 0f; num647 < 1f; num647 += 0.25f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num647 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
            Vector2 vector58 = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector58 / 2f, vector58 + new Vector2(400f))))
            {
                for (int num648 = 0; num648 < 7; num648++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom<int>(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }


            int itemDrop = Item.NewItem(Projectile.GetSource_DropAsItem(), (int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height,ModContent.ItemType<FallenStarDayItem>());

            if (Main.netMode == 1 && itemDrop >= 0)
            {
                NetMessage.SendData(21, -1, -1, null, itemDrop, 1f);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D value20 = TextureAssets.Projectile[Projectile.type].Value;
            Microsoft.Xna.Framework.Rectangle rectangle6 = new Rectangle(0, 0, value20.Width, value20.Height);
            Vector2 origin9 = rectangle6.Size() / 2f;
            Microsoft.Xna.Framework.Color alpha3 = Projectile.GetAlpha(lightColor);
            Texture2D value21 = TextureAssets.Extra[91].Value;
            Microsoft.Xna.Framework.Rectangle value22 = value21.Frame();
            Vector2 origin10 = new Vector2((float)value22.Width / 2f, 10f);
            Vector2 vector34 = new Vector2(0f, Projectile.gfxOffY);
            Vector2 spinningpoint = new Vector2(0f, -10f);
            float num189 = (float)Main.timeForVisualEffects / 60f;
            Vector2 vector35 = Projectile.Center + Projectile.velocity;
            Microsoft.Xna.Framework.Color color44 = Microsoft.Xna.Framework.Color.Blue * 0.2f;
            Microsoft.Xna.Framework.Color color45 = Microsoft.Xna.Framework.Color.White * 0.5f;
            color45.A = 0;
            float num190 = 0f;
            if (Main.tenthAnniversaryWorld)
            {
                color44 = Microsoft.Xna.Framework.Color.HotPink * 0.3f;
                color45 = Microsoft.Xna.Framework.Color.White * 0.75f;
                color45.A = 0;
                num190 = -0.1f;
            }

            Microsoft.Xna.Framework.Color color46 = color44;
            color46.A = 0;
            Microsoft.Xna.Framework.Color color47 = color44;
            color47.A = 0;
            Microsoft.Xna.Framework.Color color48 = color44;
            color48.A = 0;
            Main.EntitySpriteDraw(value21, vector35 - Main.screenPosition + vector34 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189), value22, color46, Projectile.velocity.ToRotation() + (float)Math.PI / 2f, origin10, 1.5f + num190, SpriteEffects.None);
            Main.EntitySpriteDraw(value21, vector35 - Main.screenPosition + vector34 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189 + (float)Math.PI * 2f / 3f), value22, color47, Projectile.velocity.ToRotation() + (float)Math.PI / 2f, origin10, 1.1f + num190, SpriteEffects.None);
            Main.EntitySpriteDraw(value21, vector35 - Main.screenPosition + vector34 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189 + 4.18879032f), value22, color48, Projectile.velocity.ToRotation() + (float)Math.PI / 2f, origin10, 1.3f + num190, SpriteEffects.None);
            Vector2 vector36 = Projectile.Center - Projectile.velocity * 0.5f;
            for (float num191 = 0f; num191 < 1f; num191 += 0.5f)
            {
                float num192 = num189 % 0.5f / 0.5f;
                num192 = (num192 + num191) % 1f;
                float num193 = num192 * 2f;
                if (num193 > 1f)
                {
                    num193 = 2f - num193;
                }
                Main.EntitySpriteDraw(value21, vector36 - Main.screenPosition + vector34, value22, color45 * num193, Projectile.velocity.ToRotation() + (float)Math.PI / 2f, origin10, 0.3f + num192 * 0.5f, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(value20, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle6, alpha3, Projectile.rotation, origin9, Projectile.scale + 0.1f, SpriteEffects.None);
            return;
        }
    }
}
