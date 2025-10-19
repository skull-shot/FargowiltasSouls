using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace FargowiltasSouls.Content.Quests.UI
{
    internal class DeviQuestListOption : UIPanel
    {
        public DeviQuest quest;
        UIText title;
        NewQuestIcon icon;
        public Action<DeviQuest> OnSelect;
        public Func<DeviQuest?> selectedQuest;

        public DeviQuestListOption(DeviQuest quest)
        {
            this.quest = quest;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            OnSelect?.Invoke(quest);
            OnActivate();
            UpdateIcon();
            //BackgroundColor = Color.Black * 0.4f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void OnActivate()
        {
            UpdateElements();
            base.OnActivate();
        }

        public void UpdateElements()
        {
            if (title != null)
                RemoveChild(title);

            title = new UIText(quest.GetName());
            title.Left.Set(0, 0);
            title.Top.Set(5, 0);
            title.HAlign = 0.5f;

            Append(title);
        }

        public void UpdateIcon()
        {
            if (quest.HasBeenInteractedWith())
            {
                if (icon != null)
                    icon.RemoveChild(icon);
            }
        }

        public override void OnInitialize()
        {
            if (!quest.HasBeenInteractedWith())
            {
                icon = new NewQuestIcon();
                icon.Left.Set(6, 0);
                icon.Top.Set(0, 0.5f);
                icon.Height.Set(0, 1f);
                Append(icon);
            }

            base.OnInitialize();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            //Vector2 position = GetDimensions().Position();
            //Texture2D text = TextureAssets.Extra[ExtrasID.EmoteBubble].Value;
            //Rectangle frame = new Rectangle(240, 30, 20, 20);

            //spriteBatch.Draw(text, position + frame.Size() / 2, frame, Color.White);
        }
    }


    public class NewQuestIcon : UIElement
    {
        float scale = 1f;
        float OpenTime = 0;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Vector2 position = GetDimensions().Position();
            Texture2D text = FargoAssets.GetTexture2D("UI/DeviQuests", "ExclamationPoint").Value;
            int frameY = Math.Floor(OpenTime / 8f) % 2 == 0 ? 0 : text.Height / 2;
            Rectangle frame = new Rectangle(0, frameY, text.Width, text.Height / 2);
            spriteBatch.Draw(text, position, frame, Color.White, scale, frame.Size() / 2, 1, SpriteEffects.None, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            OpenTime++;
            scale = (float)Math.Sin(OpenTime / 20f) * (MathHelper.Pi / 16f);
        }
    }
}
