using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Snow
{
    public class IceShard : ModProjectile
    {
        // why is this not a vanilla projectile already >:(
        public override string Texture => "Terraria/Images/Extra_35";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 40;
            Projectile.timeLeft = 15;
            Projectile.scale = 0.3f;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.ai[0] = (Projectile.ai[0]+1)%5;
            if (Projectile.ai[0] == 0)
                Projectile.frame = (Projectile.frame + 1) % 3;
            if (Projectile.ai[0] % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.Frost, Scale: 0.2f);
            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition, rectangle, lightColor, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
