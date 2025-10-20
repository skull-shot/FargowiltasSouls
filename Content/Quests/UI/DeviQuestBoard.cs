using Fargowiltas.Content.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Content.Quests.UI
{
    public class DeviQuestBoard : FargoUI
    {
        public static string GetLocalizedUIText(string key) => Language.GetTextValue($"Mods.FargowiltasSouls.UI.DeviQuestBoard.{key}");

        public UIPanel BackPanel;
        public UIPanel QuestListBackPanel;
        public UIList QuestList;
        internal DeviQuestInfoPanel QuestInfoPanel;

        public int OpenTime = 0;

        public const float GoldenRatio = 1.618033988749f;
        public const int BackHeight = (int)(250 * GoldenRatio);
        public const int BackWidth = (int)(BackHeight * GoldenRatio);

        internal DeviQuest? selectedQuest = null;

        internal void OnQuestSelected(DeviQuest quest)
        {
            if (quest.Equals(selectedQuest))
                return;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            selectedQuest = quest;
            selectedQuest.Interact();
            QuestInfoPanel.SetQuest(quest);
            QuestInfoPanel.Activate();
            UpdateList();
        }

        internal DeviQuest? GetSelectedQuest() => selectedQuest;

        internal void OnQuestClaimed()
        {

        }

        public override void UpdateUI()
        {
            if (Main.gameMenu || Main.playerInventory)
                FargoUIManager.Close(this);
        }
        public override void OnOpen()
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            
            BackPanel?.RemoveAllChildren();

            UpdateElements();
            CreateList();

            OpenTime = 0;
        }
        public override void OnClose()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            selectedQuest = null;
        }

        public void UpdateElements()
        {
            BackPanel.RemoveAllChildren();


            // back panel for list
            QuestListBackPanel = new UIPanel();
            QuestListBackPanel.Left.Set(6, 0);
            QuestListBackPanel.Top.Set(40, 0);
            QuestListBackPanel.Width.Set(220, 0);
            QuestListBackPanel.Height.Set(20, 0.8f);
            QuestListBackPanel.PaddingLeft = QuestListBackPanel.PaddingRight = QuestListBackPanel.PaddingTop = QuestListBackPanel.PaddingBottom = 0;
            QuestListBackPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            BackPanel.Append(QuestListBackPanel);

            // quest list
            QuestList = new UIList();
            QuestList.Left.Set(0, 0);
            QuestList.Top.Set(0, 0);
            QuestList.Width.Set(200, 0);
            QuestList.Height.Set(20, 1f);
            QuestList.ListPadding = 6f;

            QuestListBackPanel.Append(QuestList);

            // scrollbar
            UIScrollbar scrollbar = new UIScrollbar();
            scrollbar.Height.Set(0, 0.96f);
            scrollbar.Top.Set(0, 0.02f);
            scrollbar.Left.Set(QuestList.Left.Pixels + QuestList.Width.Pixels, 0);

            QuestListBackPanel.Append(scrollbar);
            QuestList.SetScrollbar(scrollbar);

            // info panel
            QuestInfoPanel = new DeviQuestInfoPanel(selectedQuest);
            QuestInfoPanel.Left.Set(240, 0);
            QuestInfoPanel.Top.Set(40, 0);
            QuestInfoPanel.Width.Set(BackWidth - 246, 0);
            QuestInfoPanel.Height.Set(20, 0.8f);
            QuestInfoPanel.PaddingLeft = QuestInfoPanel.PaddingRight = QuestInfoPanel.PaddingTop = QuestInfoPanel.PaddingBottom = 0;
            QuestInfoPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;
            QuestInfoPanel.Activate();

            BackPanel.Append(QuestInfoPanel);
        }

        public void CreateList()
        {
            foreach (var quest in FargowiltasSouls.questTracker.SortedQuests)
            {
                if (!quest.IsUnlocked())
                    continue;

                var QuestUI = new DeviQuestListOption(quest);
                QuestUI.Width.Set(0, 1f);
                QuestUI.Height.Set(50, 0);
                QuestUI.BackgroundColor = new Color(63, 82, 151) * 0.7f;
                QuestUI.Activate();

                QuestUI.OnSelect = OnQuestSelected;
                QuestUI.selectedQuest = GetSelectedQuest;

                QuestList.Add(QuestUI);
            }
        }

        public void UpdateList()
        {
            if (selectedQuest == null)
                return;

            foreach (var item in QuestList)
            {
                if (item is DeviQuestListOption opt)
                {
                    if (opt.quest == selectedQuest)
                    {
                        opt.BackgroundColor = Color.Black * 0.6f;
                        opt.UpdateElements();
                    }
                    else
                    {
                        opt.BackgroundColor = new Color(63, 82, 151) * 0.7f;
                    }
                } 
            }
        }

        public override void OnInitialize()
        {

            Vector2 offset = new(-BackWidth / 2, -BackHeight / 2);

            BackPanel = new UIPanel();
            BackPanel.Left.Set(offset.X, 0.5f);
            BackPanel.Top.Set(offset.Y, 0.5f);
            BackPanel.Width.Set(BackWidth, 0);
            BackPanel.Height.Set(BackHeight, 0);
            BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = BackPanel.PaddingBottom = 0;
            BackPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            Append(BackPanel);

            base.OnInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (OpenTime < 30)
                OpenTime++;
            if (Main.mouseItem.type != ItemID.None)
                OpenTime = 0;
            if (!BackPanel.IsMouseHovering)
            {
                if (Main.mouseLeft && OpenTime >= 30)
                    FargoUIManager.Close(this);
            }
            else
                Main.LocalPlayer.mouseInterface = true;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }
    }
}
