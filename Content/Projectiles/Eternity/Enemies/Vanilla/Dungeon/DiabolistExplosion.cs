using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Dungeon
{
    public class DiabolistExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_687";
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.width = Projectile.height = 60;
            
            Projectile.tileCollide = true;
            Projectile.timeLeft = 20;
            Main.projFrames[Type] = 7;
            Projectile.scale = 2;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Color color = new(lightColor.R + 120, lightColor.G + 100, lightColor.B + 80, lightColor.A + 100);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * t.Height() / 7, t.Width(), t.Height() / 7), color , Projectile.rotation, new Vector2(t.Width(), t.Height() / 7) / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 19)
            {
                Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.6f, 0.5f, 0.4f));
            Projectile.frame = (int)MathHelper.Lerp(0, 6, 1 - Projectile.timeLeft / 20f);
            base.AI();
        }
    }
}
