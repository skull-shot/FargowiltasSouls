using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Fargowiltas.Common.Systems.InstaVisual;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class TomeShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_712";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 1.2f;
            Projectile.extraUpdates = 1;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 240;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.Opacity = 0.85f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            FargoExtensionMethods.Animate(Projectile, 5);
        }
    }
}
