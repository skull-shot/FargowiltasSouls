using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Quests
{
    public class DeviQuestWorldSavingSystem : ModSystem
    {
        public static bool canSave = true;

        public override void SaveWorldData(TagCompound tag)
        {
            if (!canSave)
                return;

            TagCompound q = new TagCompound();

            foreach (var quest in sortQuests())
            {
                q.Add(quest.GetID(), quest.SerializeData());
            }

            tag.Add("DeviQuests", q);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            TagCompound quests = tag.GetCompound("DeviQuests");
            if (quests == null)
                return;

            foreach (var quest in sortQuests())
            {
                string ID = quest.GetID();
                if (quests.ContainsKey(ID))
                {
                    DeviQuest.LoadInstance(quest, quests.GetCompound(ID));
                }
            }
        }

        public override void AddRecipes()
        {
            FargowiltasSouls.questTracker.FinalizeQuests();
        }

        private static List<DeviQuest> sortQuests() => FargowiltasSouls.questTracker.SortedQuests;
    }
}
