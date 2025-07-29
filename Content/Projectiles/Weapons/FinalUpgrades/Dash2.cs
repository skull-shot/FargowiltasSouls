using FargowiltasSouls.Assets.Textures;

namespace FargowiltasSouls.Content.Projectiles.Weapons.FinalUpgrades
{
    public class Dash2 : Dash
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/FinalUpgrades", "Dash");

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 15 * 60 * (Projectile.extraUpdates + 1);
        }
    }
}