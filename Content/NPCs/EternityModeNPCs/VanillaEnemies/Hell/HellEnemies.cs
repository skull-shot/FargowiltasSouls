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
        public static bool HellBuffActive => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.Lifelight] || NPC.downedPlantBoss;
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
    public class HellEnemyDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            LeadingConditionRule rule = new(new HellBuffDropCondition());
            switch (npc.type)
            {
                case NPCID.Demon:
                case NPCID.VoodooDemon:
                    rule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Devilstone>(), 3, 1, 4));
                    break;
                case NPCID.RedDevil:
                    rule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Devilstone>(), 1, 5, 8));
                    break;
            }
            npcLoot.Add(rule);
        }
    }
}
