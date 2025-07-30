using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Corruption
{
    public class Clinger : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Clinger);
    }
}
