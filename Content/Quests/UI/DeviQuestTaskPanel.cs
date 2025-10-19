using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;

namespace FargowiltasSouls.Content.Quests.UI
{
    internal class DeviQuestTaskPanel : UIPanel
    {
        UIList tasks;
        DeviQuest? quest = null;

        public DeviQuestTaskPanel(DeviQuest? quest)
        {
            this.quest = quest;
        }

        public override void OnActivate()
        {
            // title
            var tTitle = new UIText(DeviQuestBoard.GetLocalizedUIText("TasksHeader"));
            tTitle.Left.Set(0, 0);
            tTitle.Top.Set(5, 0);
            tTitle.HAlign = 0.5f;
            Append(tTitle);

            if (quest == null)
                return;

            // actual list
            tasks = new UIList();
            tasks.Left.Set(0, 0);
            tasks.Top.Set(30, 0);
            tasks.Width.Set(0, 1f);
            tasks.Height.Set(0, 0.8f);
            tasks.ListPadding = 2f;

            Append(tasks);


            float totalLines = 0;
            int height = 28;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float FontSize = 1f;
            foreach (var t in quest.GetTasks())
            {
                string text = t.TaskDescription();
                string wText = font.CreateWrappedText(text, 0.8f * (GetInnerDimensions().Width - 12 - 19));
                int estimatedLines = wText.Split('\n').Length;
                totalLines += estimatedLines;

                var panel = new UIPanel();
                panel.Top.Set(0, 0);
                panel.Left.Set(0, 0);
                panel.Width.Set(0, 0.90f);
                panel.Height.Set((height * estimatedLines) + 4, 0);
                panel.HAlign = 0.5f;
                Color c = t.IsComplete() ? Color.Lime : Color.Red;
                panel.BackgroundColor = c * 0.4f;
                tasks.Add(panel);

                var tCheck = new DeviQuestCheckbox(t);
                tCheck.Left.Set(6, 0);
                tCheck.Height.Set(0, 0.5f);
                tCheck.Width.Set(19, 0);
                tCheck.Top.Set(-2 * estimatedLines, 0.5f);
                tCheck.HAlign = 0f;

                panel.Append(tCheck);

                var tText = new UIText(t.TaskDescription(), FontSize);
                float textHeight = 20 * FontSize;
                tText.Left.Set(tCheck.Width.Pixels, 0);
                tText.Top.Set((textHeight - height) / (2f), 0);
                tText.Width.Set(-tCheck.Width.Pixels - 12f, 1f);
                tText.Height.Set(0, 1f);
                tText.TextOriginX = 0f;
                tText.IsWrapped = true;
                panel.Append(tText);
            }

            if (totalLines >= 5)
            {
                var scrollBar = new UIScrollbar();
                scrollBar.Left.Set(-12, 1f);
                scrollBar.Height.Set(0, 0.94f);
                scrollBar.Width.Set(GetInnerDimensions().Width - 246, 0);
                scrollBar.VAlign = 0.5f;

                Append(scrollBar);
                tasks.SetScrollbar(scrollBar);

            }

            base.OnActivate();
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {

            base.DrawChildren(spriteBatch);
        }

        private int EstimateWrappedLines(string text, float maxWidth, DynamicSpriteFont font)
        {
            string[] words = text.Split(' ');
            float lineWidth = 0f;
            int lines = 1; // At least one line

            foreach (var word in words)
            {
                Vector2 wordSize = font.MeasureString(word + " "); // add space between words
                if (lineWidth + wordSize.X > maxWidth)
                {
                    lines++;
                    lineWidth = wordSize.X;
                }
                else
                {
                    lineWidth += wordSize.X;
                }
            }

            return lines;
        }
    }

}
