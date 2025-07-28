using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace FargowiltasSouls.Content.Projectiles.Masomode.Bosses.MechanicalBosses
{
    public class MechElectricOrbTelegraphed : MechElectricOrb
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/Bosses/MechanicalBosses/MechElectricOrb";


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int sizeY = texture.Height / Main.projFrames[Type]; //ypos of lower right corner of sprite to draw
            int sizeX = texture.Width / 4;
            int frameY = Projectile.frame * sizeY;
            int frameX = (int)ColorType * sizeX;
            Rectangle rectangle = new(frameX, frameY, sizeX, sizeY);
            Vector2 origin = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float telegraphTime = 35;
            if (++Projectile.localAI[2] < telegraphTime)
            {
                Projectile.position -= Projectile.velocity * (1 - (Projectile.localAI[2] / telegraphTime));
            }

            Color color = ColorType switch
            {
                Blue => Color.Teal,
                Green => Color.Green,
                Yellow => Color.Yellow,
                _ => Color.Red
            };
            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.33f)
            {
                Color oldColor = color;
                oldColor.A = 50;
                float modifier = (float)(ProjectileID.Sets.TrailCacheLength[Type] - i) / ProjectileID.Sets.TrailCacheLength[Type];
                oldColor *= modifier;
                float scale = (Projectile.scale / 2) + (Projectile.scale * modifier / 2);
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 oldPos = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1) + (origin / 2);
                float oldRot = Projectile.oldRot[max0];
                Main.EntitySpriteDraw(texture, oldPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, oldColor,
                    oldRot, origin, scale, spriteEffects, 0);
            }

            Asset<Texture2D> line = TextureAssets.Extra[178];
            float opacity = 0.55f;
            Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, color * opacity, Projectile.velocity.ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(0.33f, Projectile.scale * 5), SpriteEffects.None);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Color.White,
                    Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}