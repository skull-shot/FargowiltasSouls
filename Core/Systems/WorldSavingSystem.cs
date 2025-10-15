﻿using FargowiltasSouls.Content.WorldGeneration;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Core.Systems
{
    public class WorldSavingSystem : ModSystem
    {
        public enum Downed //to keep them organized and synced, DO NOT rearrange
        {
            TimberChampion,
            TerraChampion,
            EarthChampion,
            NatureChampion,
            LifeChampion,
            ShadowChampion,
            SpiritChampion,
            WillChampion,
            CosmosChampion,
            TrojanSquirrel,
            Lifelight,
            CursedCoffin,
            BanishedBaron,
            Magmaw
        }

        //masomode
        public const int MaxCountPreHM = 560;
        public const int MaxCountHM = 240;

        private static bool swarmActive;
        private static bool downedBetsy;
        private static bool shouldBeEternityMode;
        private static bool masochistModeReal;
        private static bool canPlayMaso = true;
        private static bool downedFishronEX;
        internal static bool downedDevi;
        internal static bool downedAbom;
        internal static bool downedMutant;
        private static bool angryMutant;
        private static bool haveForcedAbomFromGoblins;
        private static int skipMutantP1;
        private static bool receivedTerraStorage;
        private static bool spawnedDevi;
        private static bool downedAnyBoss;
        private static bool[] downedBoss = new bool[Enum.GetValues(typeof(Downed)).Length];
        private static bool wOFDroppedDeviGift2;
        private static bool shiftingSandEvent;
        private static bool haveForcedMutantFromKS;

        public static bool EternityMode { get; set; }

        public static bool EternityVanillaBehaviour { get; set; }

        public static bool DownedAnyBoss { get => downedAnyBoss; set => downedAnyBoss = value; }

        public static int SkipMutantP1 { get => skipMutantP1; set => skipMutantP1 = value; }

        public static bool WOFDroppedDeviGift2 { get => wOFDroppedDeviGift2; set => wOFDroppedDeviGift2 = value; }

        public static bool ShiftingSandEvent { get => shiftingSandEvent; set => shiftingSandEvent = value; }

        public static bool[] DownedBoss { get => downedBoss; set => downedBoss = value; }

        public static bool ShouldBeEternityMode { get => shouldBeEternityMode; set => shouldBeEternityMode = value; }

        public static bool MasochistModeReal { get => masochistModeReal; set => masochistModeReal = value; }

        public static bool CanPlayMaso { get => canPlayMaso; set => canPlayMaso = value; }

        public static bool DownedFishronEX { get => downedFishronEX; set => downedFishronEX = value; }

        public static bool DownedDevi { get => downedDevi; set => downedDevi = value; }

        public static bool DownedAbom { get => downedAbom; set => downedAbom = value; }

        public static bool DownedMutant { get => downedMutant; set => downedMutant = value; }

        public static bool AngryMutant { get => angryMutant; set => angryMutant = value; }

        public static bool HaveForcedAbomFromGoblins { get => haveForcedAbomFromGoblins; set => haveForcedAbomFromGoblins = value; }

        public static bool ReceivedTerraStorage { get => receivedTerraStorage; set => receivedTerraStorage = value; }

        public static bool SpawnedDevi { get => spawnedDevi; set => spawnedDevi = value; }

        public static bool DownedBetsy { get => downedBetsy; set => downedBetsy = value; }

        public static bool SwarmActive { get => swarmActive; set => swarmActive = value; }
        
        public static bool HaveForcedMutantFromKS { get => haveForcedMutantFromKS; set => haveForcedMutantFromKS = value;}

        public static bool PlacedMutantStatue;

        public static string DungeonBrickType = "B";

        public static Point CoffinArenaCenter { get; set; }

        public override void Unload() => DownedBoss = null;

        private static void ResetFlags()
        {
            SwarmActive = false;
            DownedBetsy = false;

            ShouldBeEternityMode = false;
            EternityMode = false;
            EternityVanillaBehaviour = true;
            CanPlayMaso = true;
            MasochistModeReal = false;
            DownedFishronEX = false;
            DownedDevi = false;
            DownedAbom = false;
            DownedMutant = false;
            AngryMutant = false;

            HaveForcedAbomFromGoblins = false;
            HaveForcedMutantFromKS = false;
            SkipMutantP1 = 0;

            ReceivedTerraStorage = false;
            SpawnedDevi = false;

            for (int i = 0; i < DownedBoss.Length; i++)
                DownedBoss[i] = false;

            DungeonBrickType = "B";
            DownedAnyBoss = false;
            WOFDroppedDeviGift2 = false;
            ShiftingSandEvent = false;

            PlacedMutantStatue = false;
        }

        public override void OnWorldLoad() => ResetFlags();

        public override void OnWorldUnload() => ResetFlags();

        public override void SaveWorldData(TagCompound tag)
        {

            List<string> downed = [];
            if (DownedBetsy)
                downed.Add("betsy");

            if (ShouldBeEternityMode)
                downed.Add("shouldBeEternityMode");

            if (EternityMode)
                downed.Add("eternity");

            if (CanPlayMaso)
                downed.Add("CanPlayMaso");

            if (MasochistModeReal)
                downed.Add("getReal");

            if (DownedFishronEX)
                downed.Add("downedFishronEX");

            if (DownedDevi)
                downed.Add("downedDevi");

            if (DownedAbom)
                downed.Add("downedAbom");

            if (DownedMutant)
                downed.Add("downedMutant");

            if (AngryMutant)
                downed.Add("AngryMutant");

            if (HaveForcedAbomFromGoblins)
                downed.Add("haveForcedAbomFromGoblins");

            if (HaveForcedMutantFromKS)
                downed.Add("haveForcedMutantFromKS");

            if (ReceivedTerraStorage)
                downed.Add("ReceivedTerraStorage");

            if (SpawnedDevi)
                downed.Add("spawnedDevi");

            if (DownedAnyBoss)
                downed.Add("downedAnyBoss");

            if (WOFDroppedDeviGift2)
                downed.Add("WOFDroppedDeviGift2");

            if (ShiftingSandEvent)
                downed.Add("ShiftingSandEvent");

            if (PlacedMutantStatue)
                downed.Add("PlacedMutantStatue");

            for (int i = 0; i < DownedBoss.Length; i++)
            {
                if (DownedBoss[i])
                    downed.Add("downedBoss" + i.ToString());
            }

            tag.Add("dungeonColor", DungeonBrickType);
            tag.Add("downed", downed);
            tag.Add("mutantP1", SkipMutantP1);
            tag.Add("CoffinArenaCenterX", CoffinArenaCenter.X);
            tag.Add("CoffinArenaCenterY", CoffinArenaCenter.Y);
            tag.Add("IceGolemTimer", WorldUpdatingSystem.IceGolemTimer);
            tag.Add("SandElementalTimer", WorldUpdatingSystem.SandElementalTimer);
            tag.Add("WyvernTimer", WorldUpdatingSystem.WyvernTimer);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downed");
            DownedBetsy = downed.Contains("betsy");
            ShouldBeEternityMode = downed.Contains("shouldBeEternityMode");
            EternityMode = downed.Contains("eternity") || downed.Contains("masochist");
            EternityVanillaBehaviour = true;
            CanPlayMaso = true; // downed.Contains("CanPlayMaso");
            MasochistModeReal = downed.Contains("getReal");
            DownedFishronEX = downed.Contains("downedFishronEX");
            DownedDevi = downed.Contains("downedDevi");
            DownedAbom = downed.Contains("downedAbom");
            DownedMutant = downed.Contains("downedMutant");
            AngryMutant = downed.Contains("AngryMutant");
            HaveForcedAbomFromGoblins = downed.Contains("haveForcedAbomFromGoblins");
            HaveForcedMutantFromKS = downed.Contains("haveForcedMutantFromKS");
            ReceivedTerraStorage = downed.Contains("ReceivedTerraStorage");
            SpawnedDevi = downed.Contains("spawnedDevi");
            DownedAnyBoss = downed.Contains("downedAnyBoss");
            WOFDroppedDeviGift2 = downed.Contains("WOFDroppedDeviGift2");
            ShiftingSandEvent = downed.Contains("ShiftingSandEvent");
            PlacedMutantStatue = downed.Contains("PlacedMutantStatue");

            for (int i = 0; i < DownedBoss.Length; i++)
                DownedBoss[i] = downed.Contains($"downedBoss{i}") || downed.Contains($"downedChampion{i}");

            if (tag.ContainsKey("mutantP1"))
                SkipMutantP1 = tag.GetAsInt("mutantP1");
            int coffinX = 0;
            if (tag.ContainsKey("CoffinArenaCenterX"))
                coffinX = tag.GetAsInt("CoffinArenaCenterX");
            int coffinY = 0;
            if (tag.ContainsKey("CoffinArenaCenterY"))
                coffinY = tag.GetAsInt("CoffinArenaCenterY");
            CoffinArena.SetArenaPosition(new(coffinX, coffinY));

            if (tag.ContainsKey("dungeonColor"))
                DungeonBrickType = tag.GetString("dungeonColor");

            if (tag.ContainsKey("IceGolemTimer"))
                WorldUpdatingSystem.IceGolemTimer = tag.GetAsInt("IceGolemTimer");
            if (tag.ContainsKey("SandElementalTimer"))
                WorldUpdatingSystem.SandElementalTimer = tag.GetAsInt("SandElementalTimer");
            if (tag.ContainsKey("WyvernTimer"))
                WorldUpdatingSystem.WyvernTimer = tag.GetAsInt("WyvernTimer");
        }

        public override void NetReceive(BinaryReader reader)
        {
            SkipMutantP1 = reader.ReadInt32();

            BitsByte flags = reader.ReadByte();
            DownedBetsy = flags[0];
            EternityMode = flags[1];
            DownedFishronEX = flags[2];
            DownedDevi = flags[3];
            DownedAbom = flags[4];
            DownedMutant = flags[5];
            AngryMutant = flags[6];
            HaveForcedAbomFromGoblins = flags[7];
            

            flags = reader.ReadByte();
            ReceivedTerraStorage = flags[0];
            SpawnedDevi = flags[1];
            MasochistModeReal = flags[2];
            CanPlayMaso = flags[3];
            ShouldBeEternityMode = flags[4];
            DownedAnyBoss = flags[5];
            WOFDroppedDeviGift2 = flags[6];
            PlacedMutantStatue = flags[7];

            for (int i = 0; i < DownedBoss.Length; i++)
            {
                int bits = i % 8;
                if (bits == 0)
                    flags = reader.ReadByte();

                DownedBoss[i] = flags[bits];
            }

            EternityVanillaBehaviour = reader.ReadBoolean();

            DungeonBrickType = reader.ReadString();
            CoffinArenaCenter = reader.ReadVector2().ToPoint();
            ShiftingSandEvent = reader.ReadBoolean();
            HaveForcedMutantFromKS = reader.ReadBoolean();

            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            CoffinArena.Rectangle = new(x, y, width, height);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(SkipMutantP1);

            writer.Write(new BitsByte
            {
                [0] = DownedBetsy,
                [1] = EternityMode,
                [2] = DownedFishronEX,
                [3] = DownedDevi,
                [4] = DownedAbom,
                [5] = DownedMutant,
                [6] = AngryMutant,
                [7] = HaveForcedAbomFromGoblins,
            });

            writer.Write(new BitsByte
            {
                [0] = ReceivedTerraStorage,
                [1] = SpawnedDevi,
                [2] = MasochistModeReal,
                [3] = CanPlayMaso,
                [4] = ShouldBeEternityMode,
                [5] = DownedAnyBoss,
                [6] = WOFDroppedDeviGift2,
                [7] = PlacedMutantStatue
            });

            BitsByte bitsByte = new();
            for (int i = 0; i < DownedBoss.Length; i++)
            {
                int bit = i % 8;

                if (bit == 0 && i != 0)
                {
                    writer.Write(bitsByte);
                    bitsByte = new BitsByte();
                }

                bitsByte[bit] = DownedBoss[i];
            }
            writer.Write(bitsByte);

            writer.Write(EternityVanillaBehaviour);

            writer.Write(DungeonBrickType);
            writer.WriteVector2(CoffinArenaCenter.ToVector2());
            writer.Write(ShiftingSandEvent);
            writer.Write(HaveForcedMutantFromKS);
            writer.Write(CoffinArena.Rectangle.X);
            writer.Write(CoffinArena.Rectangle.Y);
            writer.Write(CoffinArena.Rectangle.Width);
            writer.Write(CoffinArena.Rectangle.Height);
        }

        public static void SetDeviDowned()
        {
            NPC.SetEventFlagCleared(ref downedDevi, -1);
        }

        public static void SetAbomDowned()
        {
            NPC.SetEventFlagCleared(ref downedAbom, -1);
        }
        
        public static void SetMutantDowned()
        {
            NPC.SetEventFlagCleared(ref downedMutant, -1);
        }
    }
}
