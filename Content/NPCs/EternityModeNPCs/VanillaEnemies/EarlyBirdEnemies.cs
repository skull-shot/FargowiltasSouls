using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
    public class EarlyBirdEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.WyvernHead,
            NPCID.WyvernBody,
            NPCID.WyvernBody2,
            NPCID.WyvernBody3,
            NPCID.WyvernLegs,
            NPCID.WyvernTail,
            NPCID.Mimic,
            NPCID.IceMimic,
            NPCID.Medusa,
            NPCID.PigronCorruption,
            NPCID.PigronCrimson,
            NPCID.PigronHallow,
            NPCID.AngryNimbus,
            NPCID.MushiLadybug,
            NPCID.AnomuraFungus,
            NPCID.ZombieMushroom,
            NPCID.ZombieMushroomHat,
            NPCID.IceGolem,
            NPCID.SandElemental
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            if (!Main.hardMode)
            {
                switch (npc.type)
                {
                    case NPCID.IceGolem:
                    case NPCID.SandElemental:
                        npc.lifeMax = (int)Math.Round(npc.lifeMax * 0.4);
                        npc.defense /= 4;
                        break;
                    case NPCID.AngryNimbus:
                        npc.lifeMax /= 3;
                        npc.damage /= 2;
                        npc.defense /= 4;
                        npc.knockBackResist *= 2;
                        break;
                    default: break;
                }
            }
        }
    }
}
