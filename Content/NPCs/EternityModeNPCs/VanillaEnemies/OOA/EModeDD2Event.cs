using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ID.NPCID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class EModeDD2Event : ModSystem
    {
        #region Static Methods
        public static NPC GetEterniaCrystal()
        {
            int n = NPC.FindFirstNPC(NPCID.DD2EterniaCrystal);
            if (n == -1)
                return null;

            return Main.npc[n];
        }

        public static float GetWaveProgressPercent()
        {
            if (!DD2Event.Ongoing || Main.invasionProgressMax == 1)
                return -1;

            return NPC.waveKills / Main.invasionProgressMax;
        }

        public static bool IsFinalWave()
        {
            if (!DD2Event.Ongoing)
                return false;
            if (DD2Event.OngoingDifficulty == 1 && NPC.waveNumber == 5)
                return true;
            if (DD2Event.OngoingDifficulty == 2 && NPC.waveNumber == 7)
                return true;
            if (DD2Event.OngoingDifficulty == 3 && NPC.waveNumber == 7)
                return true;
            return false;
        }

        public static bool IsDD2Boss(int type)
        {
            return type == DD2DarkMageT1 || type == DD2DarkMageT3 || type == DD2OgreT2 || type == DD2OgreT3 || type == DD2Betsy;
        }
        #endregion

        #region Detours
        public override void Load()
        {
            On_DD2Event.SpawnMonsterFromGate += EMode_SpawnMonsterFromGate;
            On_Lang.GetInvasionWaveText += EMode_GetWaveText;
        }

        public override void Unload()
        {
            On_DD2Event.SpawnMonsterFromGate -= EMode_SpawnMonsterFromGate;
            On_Lang.GetInvasionWaveText -= EMode_GetWaveText;
        }

        public static void EMode_SpawnMonsterFromGate(On_DD2Event.orig_SpawnMonsterFromGate orig, Vector2 gateBottom)
        {
            if (!WorldSavingSystem.EternityMode)
            {
                orig(gateBottom);
                return;
            }

            switch (DD2Event.OngoingDifficulty)
            {
                case 1:
                    Difficulty_1_EMode_SpawnMonsterFromGate(gateBottom);
                    break;
                case 2:
                    Difficulty_2_EMode_SpawnMonsterFromGate(gateBottom);
                    break;
                case 3:
                    //Difficulty_3_EMode_SpawnMonsterFromGate(gateBottom);
                    orig(gateBottom);
                    break;
            }
        }

        public static NetworkText EMode_GetWaveText(On_Lang.orig_GetInvasionWaveText orig, int wave, params short[] npcIds)
        {
            if (!WorldSavingSystem.EternityMode || !DD2Event.Ongoing)
            {
                return orig(wave, npcIds);
            }

            switch (DD2Event.OngoingDifficulty)
            {
                case 1:
                    return orig(wave, Difficulty_1_EMode_GetWaveText(wave));
                case 2:
                    return orig(wave, Difficulty_2_EMode_GetWaveText(wave));
                case 3:
                    break;
            }
            // TODO: Add Correct Text

            return orig(wave, npcIds);
        }
        #endregion

        #region Help Methods
        #region SpawnMonsterFromGate
        private static void Difficulty_1_EMode_SpawnMonsterFromGate(Vector2 gateBottom)
        {
            int x = (int) gateBottom.X;
            int y = (int) gateBottom.Y;
            int num1 = 50;
            int num2 = 6;
            if (NPC.waveNumber > 4)
                num2 = 12;
            else if (NPC.waveNumber > 3)
                num2 = 8;

            int num3 = 6;
            if (NPC.waveNumber > 4)
                num3 = 8;

            for (int i = 1; i < Main.CurrentFrameFlags.ActivePlayersCount; i++)
            {
                num1 = (int)((double)num1 * 1.3);
                num2 = (int)((double)num2 * 1.3);
                num3 = (int)((double)num3 * 1.3);
            }

            int npc = 200;
            switch (NPC.waveNumber)
            {
                case 1: // Goblins and Goblin Bombers
                    if (NPC.CountNPCS(DD2GoblinT1) + NPC.CountNPCS(DD2GoblinBomberT1) < num1)
                        npc = OOAChanceNPC(x, y, DD2GoblinT1, DD2GoblinBomberT1, 5);
                    break;
                case 2: // Goblins, Goblin Bombers, and Wyverns
                    if (Main.rand.NextBool(6) && NPC.CountNPCS(DD2WyvernT1) < num2)
                        npc = OOANPC(x, y, DD2WyvernT1);
                    else if (NPC.CountNPCS(DD2GoblinT1) + NPC.CountNPCS(DD2GoblinBomberT1) < num1)
                        npc = OOAChanceNPC(x, y, DD2GoblinT1, DD2GoblinBomberT1, 6);
                    break;
                case 3: // Goblins, Goblin Bombers, and Javelinsts
                    if (Main.rand.NextBool(5) && NPC.CountNPCS(DD2JavelinstT1) < num2)
                        npc = OOANPC(x, y, DD2JavelinstT1);
                    else if (NPC.CountNPCS(DD2GoblinT1) + NPC.CountNPCS(DD2GoblinBomberT1) < num1)
                        npc = OOAChanceNPC(x, y, DD2GoblinT1, DD2GoblinBomberT1, 5);
                    break;
                case 4: // Goblins, Goblin Bombers, Wyverns, and Javelinsts
                    if (Main.rand.NextBool(5) && NPC.CountNPCS(DD2JavelinstT1) < num3)
                        npc = OOANPC(x, y, DD2JavelinstT1);
                    else if (Main.rand.NextBool(7) && NPC.CountNPCS(DD2WyvernT1) < num2)
                        npc = OOANPC(x, y, DD2WyvernT1);
                    else if (NPC.CountNPCS(DD2GoblinT1) + NPC.CountNPCS(DD2GoblinBomberT1) < num1)
                        npc = OOAChanceNPC(x, y, DD2GoblinT1, DD2GoblinBomberT1, 4);
                    break;
                case 5: // Goblins, Goblin Bombers, Wyverns, Javelinsts, and Dark Mage
                    if (GetWaveProgressPercent() > 0.5f && !NPC.AnyNPCs(DD2DarkMageT1))
                        npc = OOANPC(x, y, DD2DarkMageT1);
                    else if (Main.rand.NextBool(10) && NPC.CountNPCS(DD2WyvernT1) < num3)
                        npc = OOANPC(x, y, DD2WyvernT1);
                    else if (Main.rand.NextBool(4) && NPC.CountNPCS(DD2JavelinstT1) < num2)
                        npc = OOANPC(x, y, DD2JavelinstT1);
                    else if (NPC.CountNPCS(DD2GoblinT1) + NPC.CountNPCS(DD2GoblinBomberT1) < num1)
                        npc = OOAChanceNPC(x, y, DD2GoblinT1, DD2GoblinBomberT1, 3);
                    break;
                default:
                    npc = OOANPC(x, y, DD2GoblinT1);
                    break;
            }

            if (Main.dedServ && npc < 200)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc);
        }

        private static void Difficulty_2_EMode_SpawnMonsterFromGate(Vector2 gateBottom)
        {
            int x = (int)gateBottom.X;
            int y = (int)gateBottom.Y;
            int num = 50;
            int num2 = 5;
            if (NPC.waveNumber > 1)
                num2 = 8;

            if (NPC.waveNumber > 3)
                num2 = 10;

            if (NPC.waveNumber > 5)
                num2 = 12;

            int num3 = 5;
            if (NPC.waveNumber > 4)
                num3 = 7;

            int num4 = 2;
            int num5 = 8;
            if (NPC.waveNumber > 3)
                num5 = 12;

            int num6 = 3;
            if (NPC.waveNumber > 5)
                num6 = 5;

            for (int i = 1; i < Main.CurrentFrameFlags.ActivePlayersCount; i++)
            {
                num = (int)((double)num * 1.3);
                num2 = (int)((double)num2 * 1.3);
                num5 = (int)((double)num * 1.3);
                num6 = (int)((double)num * 1.35);
            }

            int num7 = 200;
            int num8 = 200;
            switch (NPC.waveNumber)
            {
                case 1: // Goblins and Javelinsts
                    if (Main.rand.NextBool(20) && NPC.CountNPCS(DD2JavelinstT2) < num2)
                        num7 = OOANPC(x, y, DD2JavelinstT2);
                    else if (NPC.CountNPCS(DD2GoblinT2) < num)
                        num7 = OOANPC(x, y, DD2GoblinT2);
                    break;
                case 2: // Goblins, Drakins, and Kobold Walkers
                    if (Main.rand.NextBool(10) && NPC.CountNPCS(DD2DrakinT2) < num6)
                        num7 = OOANPC(x, y, DD2DrakinT2);
                    else if (Main.rand.NextBool(4) && NPC.CountNPCS(DD2KoboldWalkerT2) < num5)
                        num7 = OOANPC(x, y, DD2KoboldWalkerT2);
                    else if (NPC.CountNPCS(DD2GoblinT2) < num)
                        num7 = OOANPC(x, y, DD2GoblinT2);
                    break;
                case 3: // Goblins, Wyverns, Javelinsts, and Drakins
                    if (Main.rand.NextBool(7) && NPC.CountNPCS(DD2WyvernT2) < num3)
                        num7 = OOANPC(x, y, DD2WyvernT2);
                    else if (Main.rand.NextBool(9) && NPC.CountNPCS(DD2DrakinT2) < num5)
                        num7 = OOANPC(x, y, DD2DrakinT2);
                    else if (Main.rand.NextBool(8) && NPC.CountNPCS(DD2JavelinstT2) < num2)
                        num7 = OOANPC(x, y, DD2JavelinstT2);
                    else if (NPC.CountNPCS(DD2GoblinT2) < num)
                        num7 = OOANPC(x, y, DD2GoblinT2);
                    break;
                case 4: // Goblin Bombers, Wyverns, Drakins, Kobold Walkers, and Kobold Flyers
                    if (Main.rand.NextBool(10) && NPC.CountNPCS(DD2DrakinT2) < num6)
                        num7 = OOANPC(x, y, DD2DrakinT2);
                    else if (Main.rand.NextBool(9) && NPC.CountNPCS(DD2WyvernT2) < num3)
                        num7 = OOANPC(x, y, DD2WyvernT2);
                    else if (Main.rand.NextBool(7) && NPC.CountNPCS(DD2KoboldFlyerT2) < num2)
                        num7 = OOANPC(x, y, DD2KoboldFlyerT2);
                    else if (Main.rand.NextBool(3) && NPC.CountNPCS(DD2KoboldWalkerT2) < num5)
                        num7 = OOANPC(x, y, DD2KoboldWalkerT2);
                    else if (NPC.CountNPCS(DD2GoblinBomberT2) < num)
                        num7 = OOANPC(x, y, DD2GoblinBomberT2);
                    break;
                case 5: // Goblins, Goblin Bombers, Javelinsts, Wither Beasts, Kobold Walkers, and Kobold Flyers
                    if (Main.rand.NextBool(8) && NPC.CountNPCS(DD2WitherBeastT2) < num6)
                        num7 = OOANPC(x, y, DD2WitherBeastT2);
                    else if (Main.rand.NextBool(9) && NPC.CountNPCS(DD2JavelinstT2) < num3)
                        num7 = OOANPC(x, y, DD2JavelinstT2);
                    else if (Main.rand.NextBool(4) && NPC.CountNPCS(DD2KoboldWalkerT2) + NPC.CountNPCS(DD2KoboldFlyerT2) < num5)
                        num7 = OOAChanceNPC(x, y, DD2KoboldWalkerT2, DD2KoboldFlyerT2, 4);
                    else if (NPC.CountNPCS(DD2GoblinBomberT2) + NPC.CountNPCS(DD2GoblinT2) < num)
                    {
                        if (Main.rand.NextBool(3))
                            num7 = OOANPC(x, y, DD2GoblinBomberT2);

                        num8 = OOANPC(x, y, DD2GoblinT2);
                    }
                    break;
                case 6: // Goblins, Wyverns, Javelinsts, Wither Beasts, Drakins, and Kobold Flyers
                    if (Main.rand.Next(7) == 0 && NPC.CountNPCS(DD2DrakinT2) < num6)
                        num7 = OOANPC(x, y, DD2DrakinT2);
                    else if (Main.rand.Next(15) == 0 && NPC.CountNPCS(DD2WitherBeastT2) < num4)
                        num7 = OOANPC(x, y, DD2WitherBeastT2);
                    else if (Main.rand.Next(9) == 0 && NPC.CountNPCS(DD2KoboldFlyerT2) < num5)
                        num7 = OOANPC(x, y, DD2KoboldFlyerT2);
                    else if (Main.rand.Next(9) == 0 && NPC.CountNPCS(DD2WyvernT2) < num3)
                        num7 = OOANPC(x, y, DD2WyvernT2);
                    else if (Main.rand.Next(3) == 0 && NPC.CountNPCS(DD2JavelinstT2) < num2)
                        num7 = OOANPC(x, y, DD2JavelinstT2);
                    if (Main.rand.NextBool(3) && NPC.CountNPCS(DD2GoblinT2) < num)
                        num8 = OOANPC(x, y, DD2GoblinT2);
                    break;
                case 7: // Goblins Bombers, Wyverns, Javelinsts, Wither Beasts, Drakins, Kobold Walkers, Kobold Flyers, and Ogre
                    {

                        if (GetWaveProgressPercent() > 0.5f && !NPC.AnyNPCs(DD2OgreT2))
                            num7 = OOANPC(x, y, DD2OgreT2);
                        else if (Main.rand.NextBool(7) && NPC.CountNPCS(DD2DrakinT2) < num6)
                            num7 = OOANPC(x, y, DD2DrakinT2);
                        else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(DD2WitherBeastT2) + NPC.CountNPCS(DD2JavelinstT2) < num4)
                            num7 = OOAChanceNPC(x, y, DD2JavelinstT2, DD2WitherBeastT2, 3);
                        else if (Main.rand.Next(7) == 0 && NPC.CountNPCS(DD2KoboldWalkerT2) + NPC.CountNPCS(DD2KoboldFlyerT2) < num5)
                            num7 = OOAChanceNPC(x, y, DD2KoboldWalkerT2, DD2KoboldFlyerT2, 4);
                        else if (Main.rand.Next(11) == 0 && NPC.CountNPCS(DD2WyvernT2) < num3)
                            num7 = OOANPC(x, y, DD2WyvernT2);
                        if (Main.rand.NextBool(3) && NPC.CountNPCS(DD2GoblinBomberT2) < num)
                            num8 = OOANPC(x, y, DD2GoblinBomberT2);

                        break;
                    }
                default:
                    num7 = OOANPC(x, y, DD2GoblinT2);
                    break;
            }

            if (Main.dedServ && num7 < 200)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num7);

            if (Main.dedServ && num8 < 200)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num8);
        }

        private static void Difficulty_3_EMode_SpawnMonsterFromGate(Vector2 gateBottom)
        {

        }

        private static int OOANPC(int x, int y, int type)
        {
            return NPC.NewNPC(GetSpawnSource_OldOnesArmy(), x, y, type);
        }
        private static int OOAChanceNPC(int x, int y, int commonType, int rareType, int chanceDenom)
        {
            if (Main.rand.NextBool(chanceDenom))
                return OOANPC(x, y, rareType);
            else
                return OOANPC(x, y, commonType);
        }
        #endregion

        #region GetWaveText
        private static short[] Difficulty_1_EMode_GetWaveText(int wave)
        {
            switch(wave)
            {
                case 1:
                    return [
                        DD2GoblinT1,
                        DD2GoblinBomberT1
                    ];
                case 2:
                    return [
                        DD2GoblinT1,
                        DD2GoblinBomberT1,
                        DD2WyvernT1
                    ];
                case 3:
                    return [
                        DD2GoblinT1,
                        DD2GoblinBomberT1,
                        DD2JavelinstT1
                    ];
                case 4:
                    return [
                        DD2GoblinT1,
                        DD2GoblinBomberT1,
                        DD2WyvernT1,
                        DD2JavelinstT1
                    ];
                case 5:
                    return [
                        DD2GoblinT1,
                        DD2GoblinBomberT1,
                        DD2WyvernT1,
                        DD2JavelinstT1,
                        DD2DarkMageT1
                    ];
            }
            return [];
        }

        private static short[] Difficulty_2_EMode_GetWaveText(int wave)
        {
            switch (wave)
            {
                case 1:
                    return [
                        DD2GoblinT2,
                        DD2JavelinstT2
                    ];
                case 2:
                    return [
                        DD2GoblinT2,
                        DD2DrakinT2,
                        DD2KoboldWalkerT2
                    ];
                case 3:
                    return [
                        DD2GoblinT2,
                        DD2WyvernT2,
                        DD2JavelinstT2,
                        DD2DrakinT2
                        
                    ];
                case 4:
                    return [
                        DD2GoblinBomberT2,
                        DD2WyvernT2,
                        DD2DrakinT2,
                        DD2KoboldWalkerT2,
                        DD2KoboldFlyerT2
                        
                    ];
                case 5:
                    return [
                        DD2GoblinT2,
                        DD2GoblinBomberT2,
                        DD2JavelinstT2,
                        DD2WitherBeastT2,
                        DD2KoboldWalkerT2,
                        DD2KoboldFlyerT2
                        
                    ];
                case 6:
                    return [
                        DD2GoblinT2,
                        DD2WyvernT2,
                        DD2JavelinstT2,
                        DD2WitherBeastT2,
                        DD2DrakinT2,
                        DD2KoboldFlyerT2
                    ];
                case 7:
                    return [ 
                        DD2GoblinBomberT2,
                        DD2WyvernT2,
                        DD2JavelinstT2,
                        DD2DrakinT2,
                        DD2KoboldWalkerT2,
                        DD2KoboldFlyerT2,
                        DD2OgreT2    
                    ];
            }
            return [];
        }

        private static short[] Difficulty_3_EMode_GetWaveText(int wave)
        {
            return [];
        }
        #endregion
        #endregion

        private static IEntitySource GetSpawnSource_OldOnesArmy() => new EntitySource_OldOnesArmy();
    }
}
