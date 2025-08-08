using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Hell
{
    public class HellEnemies : EModeNPCBehaviour
    {
        public static bool HellBuffActive => Main.hardMode;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Hellbat,
            NPCID.LavaSlime,
            NPCID.FireImp,
            NPCID.Demon,
            NPCID.VoodooDemon,
            NPCID.BoneSerpentBody,
            NPCID.BoneSerpentHead,
            NPCID.BoneSerpentTail,
            NPCID.Lavabat,
            NPCID.RedDevil,
            NPCID.BurningSphere,
            NPCID.Lavafly,
            NPCID.MagmaSnail,
            NPCID.HellButterfly,
            NPCID.DemonTaxCollector
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.OnFire3] = true;
        }

    }
}
