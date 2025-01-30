using FargowiltasSouls.Content.Tiles;
using FargowiltasSouls.Content.WorldGeneration;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace FargowiltasSouls.Core.Systems
{
    public class PyramidGenSystem : ModSystem
    {
        public static bool ShouldGenerateArena => !ModLoader.HasMod("Remnants");
        public override void Load()
        {
            On_WorldGen.Pyramid += OnPyramidGen;
            On_WorldGen.PlaceTile += OnPlaceTile;
            On_WorldGen.CanKillTile_int_int_refBoolean += OnCanKillTile;
            On_Player.PickWall += OnPickWall;
            On_WorldGen.KillWall += OnKillWall;
        }
        public override void Unload()
        {
            On_WorldGen.Pyramid -= OnPyramidGen;
            On_WorldGen.PlaceTile -= OnPlaceTile;
            On_WorldGen.CanKillTile_int_int_refBoolean -= OnCanKillTile;
        }
        public static Point PyramidLocation = new(); // Doesn't save. Only used within worldgen.
        public override void PreWorldGen()
        {
            PyramidLocation = new();
        }
        public static bool OnPyramidGen(On_WorldGen.orig_Pyramid orig, int i, int j)
        {
            bool ret = orig(i, j);
            if (ret && ShouldGenerateArena)
            {
                if (PyramidLocation == Point.Zero)
                {
                    PyramidLocation = new(i, j);
                }
            }
            return ret;
        }
        public static bool OnPlaceTile(On_WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute = false, bool forced = false, int plr = -1, int style = 0)
        {
            if (!TileID.Sets.Torch[Type] && CoffinProtectionActive(i, j))
                return false;
            return orig(i, j, Type, mute, forced, plr, style);
        }
        public static bool OnCanKillTile(On_WorldGen.orig_CanKillTile_int_int_refBoolean orig, int x, int y, out bool blockDamaged)
        {
            bool ret = orig(x, y, out blockDamaged);
            if (FargoGlobalTile.SolidTile(x, y) && CoffinProtectionActive(x, y))
                ret = false;
            return ret;
        }
        public static void OnPickWall(On_Player.orig_PickWall orig, Player player, int x, int y, int damage)
        {
            if (CoffinProtectionActive(x, y))
                return;
            orig(player, x, y, damage);
        }
        public static void OnKillWall(On_WorldGen.orig_KillWall orig, int i, int j, bool fail = false)
        {
            if (CoffinProtectionActive(i, j))
                return;
            orig(i, j, fail);
        }
        // Tile protection with this is done in FargoGlobalTile.
        public static bool CoffinProtectionActive(int x, int y) => !WorldGen.generatingWorld && !WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.CursedCoffin] && CoffinArena.PaddedRectangle.Contains(x, y);
        public static bool ArenaItemPrevention(Item item, Player player)
        {
            bool illegalItem = false;
            bool buildItem = false;

            // no placing
            if (!ItemID.Sets.Torches[item.type] && (item.createTile != -1 || item.createWall != -1))
            {
                illegalItem = true;
                buildItem = true;
            }

            // no mining
            if (item.pick != 0 || item.hammer != 0 || item.axe != 0)
                illegalItem = true;

            if (item.type == ItemID.WetBomb || item.type == ItemID.LavaBomb)
            {
                illegalItem = true;
                buildItem = true;
            }

            if (!illegalItem)
                return false;

            Point tilePos = Main.MouseWorld.ToTileCoordinates();
            if ((FargoGlobalTile.SolidTile(tilePos.X, tilePos.Y) || buildItem) && CoffinProtectionActive(tilePos.X, tilePos.Y))
                return true;

            return false;
        }
        public override void PreUpdateWorld()
        {
            //Main.NewText("pyramid pos: " + PyramidLocation.X + " " + PyramidLocation.Y + " your pos: " + Main.LocalPlayer.Bottom.ToTileCoordinates().X + " " + Main.LocalPlayer.Bottom.ToTileCoordinates().Y);
        }
        // Makes a Dunes biome and designates a Pyramid spot in it
        public static void GenerateDunesWithPyramid()
        {
            double worldSizeXMod = (double)Main.maxTilesX / 4200.0;
            DunesBiome dunesBiome = GenVars.configuration.CreateBiome<DunesBiome>();
            bool success = false;
            for (int i = 0; i < 1000; i++)
            {
                Point dunePoint = Point.Zero;
                int huh = 0; // what's up with this variable. it's very strange.
                for (int j = 0; j < 1000; j++)
                {
                    dunePoint = WorldGen.RandomWorldPoint(0, 500, 0, 500);
                    bool nearJungle = Math.Abs(dunePoint.X - GenVars.jungleOriginX) < (int)(600.0 * worldSizeXMod);
                    bool nearEdge = Math.Abs(dunePoint.X - Main.maxTilesX / 2) < 300;
                    bool nearSnow = dunePoint.X > GenVars.snowOriginLeft - 300 && dunePoint.X < GenVars.snowOriginRight + 300;
                    huh++;
                    if (huh >= Main.maxTilesX)
                    {
                        nearJungle = false;
                    }
                    if (huh >= Main.maxTilesX * 2)
                    {
                        nearSnow = false;
                    }
                    bool validated = !(nearJungle || nearEdge || nearSnow);
                    if (validated)
                    {
                        break;
                    }
                }
                dunesBiome.Place(dunePoint, GenVars.structures);
                int x = WorldGen.genRand.Next(dunePoint.X - 200, dunePoint.X + 200);
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (Main.tile[x, y].HasTile && ValidatePosition(x, y, GenVars.numPyr))
                    {
                        GenVars.PyrX[GenVars.numPyr] = x;
                        GenVars.PyrY[GenVars.numPyr] = y + 20;
                        GenVars.numPyr++;
                        success = true;
                        break;
                    }
                }
                if (GenVars.numPyr > 0)
                {
                    success = true;
                    break;
                }
                   
            }
            //if (!success)
            //    throw new Exception("Fargo's Souls: Pyramid position could not be correctly designated. Post a bug report in Fargo's Mods Discord and provide this world's seed and your modlist.");

        }
        // Rummages through the world until it finds a valid spot for a Pyramid
        // Runs the "generate dune + designate pyramid" code except without generating a dune, until a pyramid spot is found in Sand tiles
        public static void DesignatePyramidSpot()
        {
            double worldSizeXMod = (double)Main.maxTilesX / 4200.0;
            //DunesBiome dunesBiome = GenVars.configuration.CreateBiome<DunesBiome>();
            int safety = 0;
            while (++safety < 10000)
            {
                Point dunePoint = Point.Zero;
                bool validated = false;
                int huh = 0; // what's up with this variable. it's very strange.
                while (!validated)
                {
                    dunePoint = WorldGen.RandomWorldPoint(0, 500, 0, 500);
                    bool nearJungle = Math.Abs(dunePoint.X - GenVars.jungleOriginX) < (int)(600.0 * worldSizeXMod);
                    bool nearEdge = Math.Abs(dunePoint.X - Main.maxTilesX / 2) < 300;
                    bool nearSnow = dunePoint.X > GenVars.snowOriginLeft - 300 && dunePoint.X < GenVars.snowOriginRight + 300;
                    huh++;
                    if (huh >= Main.maxTilesX)
                    {
                        nearJungle = false;
                    }
                    if (huh >= Main.maxTilesX * 2)
                    {
                        nearSnow = false;
                    }
                    validated = !(nearJungle || nearEdge || nearSnow);
                }
                //dunesBiome.Place(dunePoint, GenVars.structures);
                int x = WorldGen.genRand.Next(dunePoint.X - 200, dunePoint.X + 200);
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == TileID.Sand)
                    {
                        GenVars.PyrX[GenVars.numPyr] = x;
                        GenVars.PyrY[GenVars.numPyr] = y + 20;
                        GenVars.numPyr++;
                        return;
                    }
                }
            }
        }
        // Validates a pyramid position. Index is used to be consistent with vanilla code for doing this.
        public static bool ValidatePosition(int x, int y, int index)
        {
            int num690 = x;
            int num691 = y;
            if (num690 > 300 && num690 < Main.maxTilesX - 300 && (GenVars.dungeonSide >= 0 || !((double)num690 < (double)GenVars.dungeonX + (double)Main.maxTilesX * 0.15)) && (GenVars.dungeonSide <= 0 || !((double)num690 > (double)GenVars.dungeonX - (double)Main.maxTilesX * 0.15)))
            {
                if (!((double)num691 >= Main.worldSurface) && Main.tile[num690, num691].TileType == 53)
                {

                    int num692 = Main.maxTilesX;
                    for (int num693 = 0; num693 < index; num693++)
                    {
                        int num694 = Math.Abs(num690 - GenVars.PyrX[num693]);
                        if (num694 < num692)
                        {
                            num692 = num694;
                        }
                    }
                    int num695 = 220;
                    if (WorldGen.drunkWorldGen)
                    {
                        num695 /= 2;
                    }
                    num691--;
                    int i = num690;
                    int j = num691;
                    if (!(Main.tile[i, j].TileType == 151 || Main.tile[i, j].WallType == 151))
                        return true;
                    //Pyramid(num690, num691);
                }
            }
            return false;
        }
        public bool SaveDrunkWorldGen;
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            if (!ShouldGenerateArena)
                return;
            // Find index of Dunes pass
            int dunesIndex = tasks.FindIndex(g => g.Name == "Dunes");
            // After Dunes pass, generate a Dunes biome and designate a Pyramid spot if no pyramid spot was designated
            // Note that this pyramid spot may still be invalidated by later worldgen; which is why later code exists
            tasks.Insert(dunesIndex + 1, new PassLegacy("GuaranteePyramid", delegate
            {
                if (GenVars.numPyr <= 0)
                    GenerateDunesWithPyramid();
            }));
            // Find index of Pyramid pass
            int pyramidIndex = tasks.FindIndex(g => g.Name == "Pyramids");
            // Before Pyramid pass, if there's STILL no valid pyramid spot, keep designating Pyramid spots until one works.
            tasks.Insert(pyramidIndex, new PassLegacy("GuaranteePyramidAgain", delegate
            {
                bool ValidPyramid = false;
                int safety = 0;
                while (!ValidPyramid && ++safety < 1000)
                {
                    for (int index = 0; index < GenVars.numPyr; index++)
                    {
                        ValidPyramid |= ValidatePosition(GenVars.PyrX[index], GenVars.PyrY[index], index);
                    }
                    if (!ValidPyramid)
                    {
                        // Just keep doing it until it works
                        GenVars.numPyr--; // Decrement to never hit the array index limit. Overwriting older pyramid spots is fine since this only runs if none are valid anyway.
                        DesignatePyramidSpot();
                    }
                }
            }));

            //The Pyramids pass index has incremented, so increment index tracker
            pyramidIndex++;
            // Generate Cursed Coffin arena right after Pyramid pass
            tasks.Insert(pyramidIndex + 1, new PassLegacy("CursedCoffinArena", (progress, config) =>
            {
                progress.Message = "Burying a sarcophagus";
                if (PyramidLocation == Point.Zero)
                {
                    Rectangle undergroundDesertLocation = GenVars.UndergroundDesertLocation;
                    int x = undergroundDesertLocation.Center.X;
                    int y = undergroundDesertLocation.Top - 10;
                    WorldGen.Pyramid(x, y);
                }
                if (PyramidLocation != Point.Zero)
                {
                    CoffinArena.Generate(PyramidLocation);
                }
            }));
        }
    }
}
