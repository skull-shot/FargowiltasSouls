using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public class FargoSoulsSets : ModSystem
    {
        public class Items
        {

        }
        public class Projectiles
        {
            public static bool[] PierceResistImmune;
        }
        public class NPCs
        {

        }

        public override void PostSetupContent()
        {
            #region Items
            SetFactory itemFactory = ItemID.Sets.Factory;


            #endregion
            #region Projectiles
            SetFactory projectileFactory = ProjectileID.Sets.Factory;

            Projectiles.PierceResistImmune = projectileFactory.CreateBoolSet(false,
                ProjectileID.FlyingKnife,
                ProjectileID.WeatherPainShot
                );

            #endregion
            #region NPCs
            SetFactory npcFactory = NPCID.Sets.Factory;
            #endregion
        }
    }
}