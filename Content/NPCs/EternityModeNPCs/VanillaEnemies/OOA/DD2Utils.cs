using FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2Utils : ModSystem
    {
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
        #endregion

        #region Detours
        public override void Load()
        {
            On_Lang.GetInvasionWaveText += AddWaveText;
        }

        public override void Unload()
        {
            On_Lang.GetInvasionWaveText -= AddWaveText;
        }

        public static NetworkText AddWaveText(On_Lang.orig_GetInvasionWaveText orig, int wave, params short[] npcIds)
        {
            if (!WorldSavingSystem.EternityMode || IsFinalWave())
                return orig(wave, npcIds);

            // etherian shielder
            short[] newIds = new short[npcIds.Length + 1];
            for (int i = 0; i < npcIds.Length; i++)
                newIds[i] = npcIds[i];
            newIds[npcIds.Length] = (short)ModContent.NPCType<DD2Shielder>();
            return orig(wave, newIds);
        }
        #endregion
    }
}
