using Fargowiltas.Assets.Textures;
using FargowiltasSouls.Content.Quests.DeviQuestTasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.UI;

namespace FargowiltasSouls.Content.Quests.UI
{
    internal class DeviQuestCheckbox : UIElement
    {
        public const int CheckboxTextSpace = 4;

        public static DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;

        public DeviQuestBaseTask task;

        public DeviQuestCheckbox(DeviQuestBaseTask task)
        {
            this.task = task;

            //Width.Set(19, 0);
            //Height.Set(21, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();
            bool complete = task.IsComplete();

            Texture2D text = FargoMutantAssets.UI.Toggler.CheckBox.Value;
            Rectangle frame = text.Frame();
            Vector2 origin2 = frame.Size() / 2;

            spriteBatch.Draw(text, position, frame, Color.White, 0, origin2, 1, SpriteEffects.None, 0f);

            if (complete)
            {
                Texture2D text2 = FargoMutantAssets.UI.Toggler.CheckMark.Value;
                Rectangle frame2 = text2.Frame();
                Vector2 origin22 = frame2.Size() / 2;

                spriteBatch.Draw(text2, position, frame2, Color.White, 0, origin22, 1, SpriteEffects.None, 0f);
            }
        }
    }
}
