using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2Utils : ModSystem
    {
        private static List<short> waveIds = new List<short>();

        #region Util Methods
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
            if (DD2Event.OngoingDifficulty == 2 && NPC.waveNumber == 6)
                return true;
            if (DD2Event.OngoingDifficulty == 3 && NPC.waveNumber == 7)
                return true;
            return false;
        }

        public static bool IsDD2MiniBoss(int type)
        {
            return type == NPCID.DD2DarkMageT1 || type == NPCID.DD2DarkMageT3 || type == NPCID.DD2OgreT2 || type == NPCID.DD2OgreT3;
        }

        /// <summary>
        /// Checks if the current wave contains the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool WaveContainsType(int type)
        {
            if (!DD2Event.Ongoing)
                return false;
            return waveIds.Contains((short)type);
        }
        #endregion

        #region Detours
        public override void Load()
        {
            On_Lang.GetInvasionWaveText += AddWaveText;
            On_DD2Event.StopInvasion += ClearWaveIds;
        }

        public override void Unload()
        {
            On_Lang.GetInvasionWaveText -= AddWaveText;
            On_DD2Event.StopInvasion -= ClearWaveIds;
        }

        public static NetworkText AddWaveText(On_Lang.orig_GetInvasionWaveText orig, int wave, params short[] npcIds)
        {
            if (!WorldSavingSystem.EternityMode || !DD2Event.Ongoing)
            {
                waveIds = npcIds.ToList();
                return orig(wave, npcIds);
            }

            short[] new_enemies = GetEnemiesForWave(wave);
            int total = npcIds.Length + new_enemies.Length;
            if (new_enemies.Length == 0)
            {
                waveIds = npcIds.ToList();
                return orig(wave, npcIds);
            }
            int old_index = 0;
            int new_index = 0;
            short[] newIds = new short[total];
            while (new_index + old_index != total)
            {
                // make sure minibosses/betsy show up last in wave text
                if (old_index == npcIds.Length || ((IsDD2MiniBoss(npcIds[old_index]) || npcIds[old_index] == NPCID.DD2Betsy) && new_index != new_enemies.Length))
                {
                    newIds[old_index + new_index] = new_enemies[new_index];
                    new_index++;
                    continue;
                }
                newIds[old_index + new_index] = npcIds[old_index];
                old_index++;
            }
            waveIds = newIds.ToList();
            return orig(wave, newIds);
        }

        public static void ClearWaveIds(On_DD2Event.orig_StopInvasion orig, bool win)
        {
            orig(win);
            waveIds = new List<short>();
        }
        #endregion

        #region Help Methods
        private static short[] GetEnemiesForWave(int wave)
        {
            switch (DD2Event.OngoingDifficulty)
            {
                case 1:
                    return T1_GetEnemiesForWave(wave);
                case 2:
                    return T2_GetEnemiesForWave(wave);
                case 3:
                    return T3_GetEnemiesForWave(wave);
            }
            return [];
        }

        private static short[] T1_GetEnemiesForWave(int wave)
        {
            switch (wave)
            {
                case 1:
                case 2:
                    break;
                case 3:
                    return [
                        (short)ModContent.NPCType<DD2Shielder>()
                    ];
                case 4:
                    return [
                        (short)ModContent.NPCType<DD2Jammer>()
                    ];
                case 5:
                    break;
            }
            return [];
        }

        private static short[] T2_GetEnemiesForWave(int wave)
        {
            switch (wave)
            {
                case 1:
                    break;
                case 2:
                case 3:
                    return [
                        (short)ModContent.NPCType<DD2Shielder>()
                    ];
                case 4:
                case 5:
                    return [
                        (short)ModContent.NPCType<DD2Shielder>(),
                        (short)ModContent.NPCType<DD2Jammer>()
                    ];
                case 6:
                    return [
                        (short)ModContent.NPCType<DD2Shielder>()
                    ];
            }
            return [];
        }

        private static short[] T3_GetEnemiesForWave(int wave)
        {
            switch (wave)
            {
                case 1:
                case 2:
                    return [
                        (short)ModContent.NPCType<DD2Jammer>()
                    ];
                case 3:
                case 4:
                    return [
                        (short)ModContent.NPCType<DD2Shielder>()
                    ];
                case 5:
                case 6:
                    return [
                        (short)ModContent.NPCType<DD2Shielder>(),
                        (short)ModContent.NPCType<DD2Jammer>()
                    ];
                case 7:
                    return [
                        (short)ModContent.NPCType<DD2Jammer>()
                    ];
            }
            return [];
        }
        #endregion
    }
}
