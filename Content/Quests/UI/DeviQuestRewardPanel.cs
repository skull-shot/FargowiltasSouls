using Terraria.GameContent.UI.Elements;

namespace FargowiltasSouls.Content.Quests.UI
{
    internal class DeviQuestRewardPanel : UIPanel
    {
        UIList rewards;
        DeviQuest? quest = null;

        public DeviQuestRewardPanel(DeviQuest? quest)
        {
            this.quest = quest;
        }

        public override void OnActivate()
        {
            // title
            var rTitle = new UIText(DeviQuestBoard.GetLocalizedUIText("RewardsHeader"));
            rTitle.Left.Set(0, 0);
            rTitle.Top.Set(5, 0);
            rTitle.HAlign = 0.5f;
            Append(rTitle);

            if (quest == null)
                return;

            // actual list
            rewards = new UIList();
            rewards.Left.Set(6, 0);
            rewards.Top.Set(30, 0);
            rewards.Width.Set(0, 1f);
            rewards.Height.Set(0, 1f);
            rewards.ListPadding = -12f;

            Append(rewards);

            foreach (var r in quest.GetRewardStrings())
            {
                var rText = new UIText("> " + r.GetRewardString());
                rText.Left.Set(6, 0);
                rText.Width.Set(0, 1f);
                rText.Height.Set(20, 0);
                rText.IsWrapped = true;
                rText.TextOriginX = 0f;
                rewards.Add(rText);
            }

            base.OnActivate();
        }
    }
}
