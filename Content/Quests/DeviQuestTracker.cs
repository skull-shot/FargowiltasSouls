using FargowiltasSouls.Content.Bosses.CursedCoffin;
using FargowiltasSouls.Content.Items.Accessories;
using FargowiltasSouls.Content.Quests.DeviQuestTasks;
using FargowiltasSouls.Core.Systems;
using Terraria.ID;
using System.Collections.Generic;
using Terraria;
using static FargowiltasSouls.Content.Quests.DeviQuestFactory;
using static Terraria.ModLoader.ModContent;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using System;

namespace FargowiltasSouls.Content.Quests
{
    internal class DeviQuestTracker
    {
        public DeviQuestTracker()
        {
            FargowiltasSouls.questTracker = this;
            InitializeFargoQuests();
        }

        public List<DeviQuest> SortedQuests;
        internal bool QuestsFinalized = false;

        private void InitializeFargoQuests()
        {
            /*
             * QUICK REFERENCES
             * 
             * Each quest will need an entry in the DeviQuests localization folder
             * 
             * Task Types and their arguments:
             * 
             * Hunt:
             *   - arg0: NPCID of the enemy to kill
             *   - arg1: Amount of kills required
             * Collect: [arg0 : ]
             *   - arg0: ItemID of the item to obtain
             *   - arg1: Amount needed (Defaults to 1 if null)
             * Craft:
             *   - arg0: ItemID of the item to craft
             *   - arg1: null
             * Custom: [arg0 : bol]
             *   - arg0: Function which detemines completion
             *   - arg1: Localized description of task (Displayed in UI task panel)
             */
            SortedQuests = new List<DeviQuest>
            {
                NewQuest("Tutorial1", TaskSingle("Hunt", NPCID.Tim, 3), RewardSingle(ItemID.Cactus, 50)),
                NewQuest("Tutorial2", TaskSingle("Hunt", NPCType<CursedCoffin>(), 1), RewardDouble(ItemType<SoulLantern>(), ItemID.SandBlock, 1, 30)),
                NewQuest("CustomTest", TaskSingle("Custom", () => WorldSavingSystem.DownedBetsy, () => "Murder Betsy!!!"), RewardSingle(ItemID.DefendersForge), () => NPC.downedBoss1),
                NewQuest("CraftTest", TaskSingle("Craft", ItemType<ZephyrBoots>()), RewardSingle(ItemID.GoldCoin, 10)),
                NewQuest("Tutorial3", TaskSingle("Collect", ItemID.DirtBlock, 50), RewardSingle(ItemType<DeviatingEnergy>(), 5)),
                NewQuest("CollectTest", TaskSingle("Collect", ItemType<DimensionSoul>()), RewardDouble(ItemID.FartInABalloon, ItemID.LuckyHorseshoe)),
            };
        }

        internal void FinalizeQuests()
        {
            QuestsFinalized = true;
        }

        public void AddQuest(string modName, string localPath, string questName, List<DeviQuestBaseTask> tasks, List<DeviQuestReward> rewards, Func<bool> unlockCondition)
        {
            SortedQuests.Add(new DeviQuest(modName, localPath, questName, tasks, rewards, unlockCondition));
        }
    }
}
