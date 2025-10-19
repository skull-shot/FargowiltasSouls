using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;

namespace FargowiltasSouls.Content.Quests.UI
{
    internal class DeviQuestInfoPanel : UIPanel
    {
        DeviQuest? quest;
        int panelState = 0;

        UIPanel BackPanel;

        public DeviQuestInfoPanel(DeviQuest? quest)
        {
            this.quest = quest;
        }

        public void SetQuest(DeviQuest? quest)
        {
            this.quest = quest;
        }

        public void SubmitButtonPressed()
        {
            //DeviQuestPopupSystem.CreatePopup(DeviQuestPopupSystem.PopupState.QuestComplete);
            if (!quest.IsQuestComplete())
                return;

            quest.HandleSubmitRequest(Main.LocalPlayer);
        }

        public void OnButtonPress(int state)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (quest != null && state == 2)
            {
                if (!Main.LocalPlayer.QuestPlayer().HasClaimed(quest))
                    state = quest.IsQuestComplete() ? 1 : 0;

                SubmitButtonPressed();
            }

            if (state != 2)
                panelState = state;

            Activate();
        }

        public override void OnActivate()
        {
            RemoveAllChildren();
            

            if (quest == null)
            {
                var blankInfo = new UIText(DeviQuestBoard.GetLocalizedUIText("InfoBlank"));
                blankInfo.HAlign = 0.5f;
                blankInfo.VAlign = 0.5f;

                Append(blankInfo);
                return;
            }

            // title
            var title = new UIText(quest.GetName());
            title.Left.Set(0, 0);
            title.Top.Set(5, 0);
            title.HAlign = 0.5f;
            Append(title);

            // description
            var desc = new UIText(quest.GetDescription());
            desc.Left.Set(6, 0);
            desc.Top.Set(45, 0);
            desc.Width.Set(-6, 1f);
            desc.TextOriginX = 0f;
            desc.IsWrapped = true;
            Append(desc);

            // buttons
            string[] keys = {"TaskButton","RewardButton","SubmitButton"};
            for (int i = 0; i < 3; i++)
            {
                var button = new DeviQuestInfoButton(quest, keys[i], i);
                button.Top.Set(56 * i, 0.5f);
                button.Height.Set(50, 0);
                button.Left.Set(6, 0);
                button.Width.Set(-4, 0.2f);

                button.OnPress = OnButtonPress;

                if (i == 2 && (!quest.IsQuestComplete() || Main.LocalPlayer.QuestPlayer().HasClaimed(quest)))
                    button.BackgroundColor = Color.Black * 0.6f;

                Append(button);
            }

            // panel
            BackPanel = new UIPanel();
            if (panelState == 0)
                BackPanel = new DeviQuestTaskPanel(quest);
            else if (panelState == 1)
                BackPanel = new DeviQuestRewardPanel(quest);
            BackPanel.Left.Set(6, 0.2f);
            BackPanel.Width.Set(-12, 0.8f);
            BackPanel.Top.Set(0, 0.5f);
            BackPanel.Height.Set(-9, 0.5f);
            BackPanel.PaddingBottom = BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = 0;

            Append(BackPanel);

            base.OnActivate();
        }
    }
}
