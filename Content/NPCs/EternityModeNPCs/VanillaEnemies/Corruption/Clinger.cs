using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Jungle;
using FargowiltasSouls.Core.NPCMatching;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Corruption
{
    public class Clinger : Snatchers
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Clinger);
    }
}
