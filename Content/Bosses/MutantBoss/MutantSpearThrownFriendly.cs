using FargowiltasSouls.Content.Projectiles.Weapons.FinalUpgrades;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantSpearThrownFriendly : PenetratorThrown
    {
        public override string Texture => "FargowiltasSouls/Assets/Textures/Content/Projectiles/Weapons/FinalUpgrades/Penetrator";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = Terraria.ModLoader.DamageClass.Default;
        }
    }
}