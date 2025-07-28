using FargowiltasSouls.Assets.Textures;

namespace FargowiltasSouls.Content.Projectiles.Weapons.FinalUpgrades
{
    public class PenetratorDive : Penetrator
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/FinalUpgrades", "Penetrator");

        public override void AI()
        {
            base.AI();
            Projectile.localAI[0]++;
        }

        public override bool? CanDamage()
        {
            if (Projectile.localAI[0] > 2)
                return true;
            return null;
        }
    }
}