using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Quests
{
    public class DeviQuestReward : TagSerializable
    {
        public TagCompound SerializeData() => new TagCompound
        {
            ["id"] = itemId,
            ["stack"] = stack
        };

        private int itemId;
        private int stack;

        public DeviQuestReward(int itemId, int stack = 1)
        {
            this.itemId = itemId;
            this.stack = stack;
        }

        public int GetItem() => itemId;
        public int GetStack() => stack;

        public string GetRewardString()
        {
            if (stack == 1)
                return Language.GetTextValue("Mods.FargowiltasSouls.DeviQuests.RewardSingle", new Item(itemId).Name, itemId);
            else
                return Language.GetTextValue("Mods.FargowiltasSouls.DeviQuests.RewardStack", stack, new Item(itemId).Name, itemId);
        }
    }
}
