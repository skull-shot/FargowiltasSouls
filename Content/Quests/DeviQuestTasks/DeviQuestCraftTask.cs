using System;
using Terraria;
using Terraria.Localization;

namespace FargowiltasSouls.Content.Quests.DeviQuestTasks
{
    /// <summary>
    /// Generic task for crafting an item
    /// </summary>
    internal class DeviQuestCraftTask : DeviQuestBaseTask
    {
        int craftType;
        bool wasCrafted;
        /// <summary>
        /// Creates a quest task to craft the given item
        /// </summary>
        /// <param name="typeToCraft"></param>
        public DeviQuestCraftTask(int typeToCraft)
        {
            craftType = typeToCraft;

            Init();
        }

        public int GetCraftType() => craftType;

        public override void OnProgress() { wasCrafted = true; }

        public override Func<bool> CreateTaskFinishCondition()
            => () => wasCrafted;

        public override Func<string> CreateTaskDescription() => () =>
        {
            string name = new Item(craftType).Name;
            return Language.GetTextValue("Mods.FargowiltasSouls.DeviQuests.CraftTask", name);
        };
    }
}
