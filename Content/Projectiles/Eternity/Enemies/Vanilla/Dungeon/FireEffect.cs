using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Dungeon
{
    public class FireEffect : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flamelash;
        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.width = Projectile.height = 20;
            Projectile.Opacity = 0;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = true;
            Main.projFrames[Type] = 6;
            
            Projectile.scale = 0.5f;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * t.Height() / 6, t.Width(), t.Height() / 6), Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(t.Width(), t.Height() / 6) / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0) Projectile.ai[0] = Main.rand.NextFloat(1f, 2f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.timeLeft > 50 && Projectile.Opacity < 0.5f)
            {
                Projectile.Opacity += 0.05f;
            }else if (Projectile.timeLeft < 10)
            {
                Projectile.Opacity -= 0.05f;
            }
            Projectile.scale = MathHelper.Lerp(Projectile.scale, Projectile.ai[0], 0.03f);
            if (Projectile.velocity.Y > -3)
            {
                Projectile.velocity.Y -= 0.2f;
            }
                base.AI();
        }
    }
}
