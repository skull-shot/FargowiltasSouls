using FargowiltasSouls.Content.Projectiles.Eternity.Environment;
using Terraria;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Buffs
{
    public class LightningRodLightning : RainLightning
    {
        public override string Texture => "Terraria/Images/Projectile_466";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = false;
            Projectile.hostile = true;
        }
    }
}