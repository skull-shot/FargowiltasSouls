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
            public static bool[] AiStylePierceResistImmune;
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

            Projectiles.AiStylePierceResistImmune = projectileFactory.CreateBoolSet(false,
                ProjAIStyleID.Yoyo,
                ProjAIStyleID.Spear,
                ProjAIStyleID.ShortSword,
                ProjAIStyleID.Drill,
                ProjAIStyleID.HeldProjectile,
                ProjAIStyleID.NightsEdge, // all fancy sword swings
                ProjAIStyleID.CursedFlameWall, // clinger staff
                ProjAIStyleID.Rainbow, // rainbow gun
                ProjAIStyleID.MechanicalPiranha,
                ProjAIStyleID.SleepyOctopod
                );

            #endregion
            #region NPCs
            SetFactory npcFactory = NPCID.Sets.Factory;
            #endregion
        }
    }
}
