using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PirateInvasion
{
    public class Pirates : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.PirateCaptain,
            NPCID.PirateCorsair,
            NPCID.PirateCrossbower,
            NPCID.PirateDeadeye,
            NPCID.PirateShipCannon,
            NPCID.PirateDeckhand,
            NPCID.PirateGhost
        );

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);
            target.AddBuff(ModContent.BuffType<UnluckyBuff>(), 60 * 30);
            target.AddBuff(ModContent.BuffType<MidasBuff>(), 600);
        }
    }
}
