using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace FargowiltasSouls.Content.Quests.UI
{
    internal class DeviQuestInfoButton : UIPanel
    {
        DeviQuest quest;
        private int state;
        private string key;
        public Action<int> OnPress;

        public DeviQuestInfoButton(DeviQuest quest, string key, int state)
        {
            this.quest = quest;
            this.key = key;
            this.state = state;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            OnPress?.Invoke(state);
        }

        public override void OnActivate()
        {
            bool claimable = quest.IsQuestComplete() && !Main.LocalPlayer.QuestPlayer().HasClaimed(quest);

            var title = new UIText(DeviQuestBoard.GetLocalizedUIText(key));
            title.Left.Set(0, 0);
            title.Top.Set(5, 0);
            title.HAlign = 0.5f;
            if (state == 2 && !claimable)
                title.TextColor *= 0.3f;
            Append(title);

            if (state == 2 && claimable)
            {
                var icon = new NewQuestIcon();
                icon.Top.Set(-6, 0);
                icon.Left.Set(-6, 0);
                Append(icon);
            }

            Initialize();
            base.OnActivate();
        }
    }
}
