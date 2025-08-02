using System;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Martians
{
    public class MartianProbe : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MartianProbe);
        public override bool SafePreAI(NPC npc)
        {
            if (!NPC.downedGolemBoss && npc.ai[0] == 2f && (npc.position.Y < -npc.height || npc.ai[1] >= 179f) && Main.netMode != 1)
            {
                Vector2 spawnpos = new(npc.Center.X, npc.Center.Y + 16 * 60);
                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnpos, NPCID.MartianSaucerCore);
                npc.active = false;
                npc.netUpdate = true;
                return false; //kill regular martian madness spawning
            }
            else return base.SafePreAI(npc);
        }
    }
}
