using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
    public class Bats : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.JungleBat,
            NPCID.IceBat,
            NPCID.Vampire,
            NPCID.VampireBat,
            NPCID.GiantFlyingFox,
            NPCID.Hellbat,
            NPCID.Lavabat,
            NPCID.IlluminantBat,
            NPCID.CaveBat,
            NPCID.GiantBat,
            NPCID.SporeBat
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            //if (Main.rand.NextBool(4)) Horde(npc, Main.rand.Next(5) + 1);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}
