using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
    public class MeteorHead : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MeteorHead);

        public int Counter;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(Counter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            Counter = binaryReader.Read7BitEncodedInt();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (NPC.downedGolemBoss && Main.rand.NextBool(4))
                npc.Transform(NPCID.SolarCorite);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter > 120)
            {
                Counter = 0;

                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (t != -1 && npc.Distance(Main.player[t].Center) < 600 && FargoSoulsUtil.HostCheck)
                {
                    npc.velocity *= 5;
                    npc.netUpdate = true;
                    NetSync(npc);
                }
            }

            EModeGlobalNPC.Aura(npc, 100, BuffID.OnFire, false, DustID.Torch);
        }
    }
}
