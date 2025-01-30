using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
    public class CoffinPlayerSoul : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/CursedCoffin/CoffinDarkSouls";
        const int TrailLength = 10;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron Scrap");
            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }
        public int[] oldFrame = new int[TrailLength];
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.timeLeft = 60 * 6;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.localAI[0] < 25)
            {
                Projectile.localAI[0]++;
                Projectile.scale = MathHelper.Lerp(0.15f, 1, Projectile.localAI[0] / 25);
            }
            for (int i = 0; i < oldFrame.Length; i++)
            {
                int j = oldFrame.Length - i - 1;
                if (j > 0)
                    oldFrame[j] = oldFrame[j - 1];
                else
                    oldFrame[j] = Projectile.frame;
            }
            Projectile.Animate(6);

            if (Projectile.ai[1] != 0)
                Projectile.velocity.Y += Projectile.ai[1]; //ai1 is Y-acceleration

            if (Projectile.timeLeft <= 20)
            {
                Projectile.Opacity -= 1 / 20f;
                Projectile.scale -= 1 / 20f;
            }

            NPC coffin = Main.npc.FirstOrDefault(n => n.TypeAlive<CursedCoffin>());
            if (coffin.Alive())
            {
                if (Projectile.Distance(coffin.Center) > coffin.width / 2)
                {
                    Movement(coffin.Center, 0.038f, 8f, 2f, 0.01f, 30);
                }
                else
                {
                    Projectile.velocity = (coffin.Center - Projectile.Center) * 0.12f;
                    Projectile.Opacity *= 0.8f;
                }
            }
            else
            {
                Projectile.Kill();
            }

            void Movement(Vector2 pos, float accel = 0.03f, float maxSpeed = 20, float lowspeed = 5, float decel = 0.03f, float slowdown = 30)
            {
                if (Projectile.Distance(pos) > slowdown)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
                }
                else
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
                }
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCs.Add(index);
        }
        public static readonly Color GlowColor = new(224, 196, 252, 0);
        public override bool PreDraw(ref Color lightColor)
        {
            //draw projectile
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int frameY = frameHeight * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, frameY, texture2D13.Width, frameHeight);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Width - Projectile.width) / 2;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = GlowColor with { A = 255 };
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                int oldFrameY = frameHeight * oldFrame[i]; //ypos of upper left corner of sprite to draw
                Rectangle oldRectangle = new(0, oldFrameY, texture2D13.Width, frameHeight);
                Main.EntitySpriteDraw(texture2D13, value4 + drawOffset + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle?(oldRectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();
            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
