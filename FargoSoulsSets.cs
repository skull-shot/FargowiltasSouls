using Fargowiltas;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
        public class Tiles
        {
            public static List<int> CommonTiles;
        }

        public class Buffs
        {
            public static bool[] Debuffs;
        }

        public override void PostSetupContent()
        {
            #region Items
            SetFactory itemFactory = ItemID.Sets.Factory;

            List<int> sacrificePreHM = [
                // squrl
                ItemType<TreeSword>(), 1,
                ItemType<MountedAcornGun>(), 1,
                ItemType<SnowballStaff>(), 1,
                ItemType<KamikazeSquirrelStaff>(), 1,

                // coffin
                ItemType<SisypheanFist>(), 1,
                ItemType<SpiritLongbow>(), 1,
                ItemType<GildedSceptre>(), 1,
                ItemType<EgyptianFlail>(), 1,
                ];
            List<int> sacrificeHM = [
                // brn
                ItemType<TheBaronsTusk>(), 1,
                ItemType<NavalRustrifle>(), 1,
                ItemType<RoseTintedVisor>(), 1,
                ItemType<DecrepitAirstrikeRemote>(), 1,

                // lifelight
                ItemType<EnchantedLifeblade>(), 1,
                ItemType<Lightslinger>(), 1,
                ItemType<CrystallineCongregation>(), 1,
                ItemType<KamikazePixieStaff>(), 1,
                ];
            var sacrifice = sacrificePreHM.Concat(sacrificeHM).ToList();

            for (int i = 0; i < sacrificeHM.Count; i += 2)
                FargoSets.Items.HardmodeSacrifice[sacrificeHM[i]] = true;
            for (int i = 0; i < sacrifice.Count; i += 2)
                FargoSets.Items.SacrificeCountDefault[sacrifice[i]] = sacrifice[i + 1];

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
            #region Tiles
            //SetFactory tileFactory = TileID.Sets.Factory;

            var Shovel = TileID.Sets.CanBeDugByShovel.GetTrueIndexes();
            var Stone = TileID.Sets.Conversion.Stone.GetTrueIndexes();
            var Sand = TileID.Sets.Conversion.Sand.GetTrueIndexes();
            var Sandstone = TileID.Sets.Conversion.HardenedSand.GetTrueIndexes();
            var Ice = TileID.Sets.Conversion.Ice.GetTrueIndexes();
            var Mud = TileID.Sets.Mud.GetTrueIndexes();
            Tiles.CommonTiles =
            [
                TileID.Marble,
                TileID.Granite
            ];
            for (int i = 0; i < Shovel.Count; i++)
            {
                if (!Tiles.CommonTiles.Contains(Shovel[i]))
                    Tiles.CommonTiles.Add(Shovel[i]);
            }
            for (int i = 0; i < Stone.Count; i++)
            {
                if (!Tiles.CommonTiles.Contains(Stone[i]))
                    Tiles.CommonTiles.Add(Stone[i]);
            }
            for (int i = 0; i < Sand.Count; i++)
            {
                if (!Tiles.CommonTiles.Contains(Sand[i]))
                    Tiles.CommonTiles.Add(Sand[i]);
            }
            for (int i = 0; i < Sandstone.Count; i++)
            {
                if (!Tiles.CommonTiles.Contains(Sandstone[i]))
                    Tiles.CommonTiles.Add(Sandstone[i]);
            }
            for (int i = 0; i < Ice.Count; i++)
            {
                if (!Tiles.CommonTiles.Contains(Ice[i]))
                    Tiles.CommonTiles.Add(Ice[i]);
            }
            for (int i = 0; i < Mud.Count; i++)
            {
                if (!Tiles.CommonTiles.Contains(Mud[i]))
                    Tiles.CommonTiles.Add(Mud[i]);
            }
            #endregion
            #region Buffs
            SetFactory buffFactory = BuffID.Sets.Factory;

            Buffs.Debuffs = buffFactory.CreateBoolSet(false, [.. FargowiltasSouls.DebuffIDs]);
            #endregion
        }
    }
}
