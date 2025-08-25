using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class DungeonSpirit : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DungeonSpirit);

        int invulTimer = 30;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            if (source is NPC sourceNPC && sourceNPC.type == NPCID.Paladin)
                npc.dontTakeDamage = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (--invulTimer < 0)
                npc.dontTakeDamage = false;
        }
    }
}
