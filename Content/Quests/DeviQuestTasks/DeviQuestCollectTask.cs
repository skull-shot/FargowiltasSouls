using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Quests.DeviQuestTasks
{
    /// <summary>
    /// Generic task for collecting an item
    /// </summary>
    public class DeviQuestCollectTask : DeviQuestBaseTask
    {
        int itemId;
        int curCount;
        int count;

        public override void OnSave(ref TagCompound tag)
        {
            tag.Add("count", curCount);
        }

        public override void OnLoad(TagCompound tag)
        {
            curCount = tag.GetAsInt("count");
        }

        public DeviQuestCollectTask(int itemId, int count = 1)
        {
            this.itemId = itemId;
            this.count = count;

            curCount = 0;

            Init();
        }

        public override void OnProgress(object arg0) { curCount = (int)arg0; }

        public int GetItemID() => itemId;
        public int GetMaxCount() => count;

        public override void OnComplete()
        {
            curCount = Math.Min(count, curCount);
        }

        public override Func<string> CreateTaskDescription() => () =>
        {
            string name = new Item(itemId).Name;
            if (count == 1)
                return Language.GetTextValue("Mods.FargowiltasSouls.DeviQuests.CollectSingle", name);
            else
                return Language.GetTextValue("Mods.FargowiltasSouls.DeviQuests.CollectPlural", name, curCount, count);
        };

        public override Func<bool> CreateTaskFinishCondition()
            => () => curCount >= count;
    }
}
