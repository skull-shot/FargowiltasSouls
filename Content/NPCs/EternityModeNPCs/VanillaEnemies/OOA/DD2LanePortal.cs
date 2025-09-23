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
            if (DD2Event.EnemySpawningIsOnHold)
            {
                if (Main.invasionProgressMax == 1)
                    ShieldersSpawned = 0;

                Timer = 0;
                return base.SafePreAI(npc);
            }
            Timer++;

            float percent = DD2Utils.GetWaveProgressPercent();
            // shielders
            if (DD2Utils.WaveContainsType(ModContent.NPCType<DD2Shielder>()) && ShieldersSpawned * 0.4f <= percent && FargoSoulsUtil.HostCheck)
            {
                SpawnFromDD2Portal(npc, ModContent.NPCType<DD2Shielder>());
                ShieldersSpawned++;
            }
            // jammers
            if (DD2Utils.WaveContainsType(ModContent.NPCType<DD2Jammer>()) && Timer % (60 * 5) == 0 && FargoSoulsUtil.HostCheck)
                SpawnFromDD2Portal(npc, ModContent.NPCType<DD2Jammer>());

            return base.SafePreAI(npc);
        }

        public static int SpawnFromDD2Portal(NPC portal, int typeToSpawn)
        {
            if (!FargoSoulsUtil.HostCheck)
                return -1;
            return NPC.NewNPC(portal.GetSource_FromAI(), (int)portal.Bottom.X, (int)portal.Bottom.Y, typeToSpawn);
        }
    }
}
