using FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2LanePortal : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DD2LanePortal);

        public int Timer;

        public int ShieldersSpawned;

        public override bool SafePreAI(NPC npc)
        {
            //Main.NewText(GetWaveProgressPercent());
            if (DD2Event.EnemySpawningIsOnHold)
            {
                if (Main.invasionProgressMax == 1)
                    ShieldersSpawned = 0;

                Timer = 0;
                return base.SafePreAI(npc);
            }
            Timer++;

            float percent = GetWaveProgressPercent();
            if (!IsFinalWave() && ShieldersSpawned * 0.4f <= percent && FargoSoulsUtil.HostCheck)
            {
                SpawnFromDD2Portal(npc, ModContent.NPCType<DD2Shielder>());
                ShieldersSpawned++;
            }

            return base.SafePreAI(npc);
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

        public static int SpawnFromDD2Portal(NPC portal, int typeToSpawn)
        {
            if (!FargoSoulsUtil.HostCheck)
                return -1;
            return NPC.NewNPC(portal.GetSource_FromAI(), (int)portal.Bottom.X, (int)portal.Bottom.Y, typeToSpawn);
        }

    }
}
