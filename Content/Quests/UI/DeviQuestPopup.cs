using Fargowiltas.Content.UI;
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.Quests.UI
{
    public class DeviQuestPopup : FargoUI
    {
        string displayText;
        float OpenTime = 0;
        float yOffset = 0;
        float baseOffset = 0;

        public override void OnOpen()
        {
            OpenTime = 0;
            displayText = DeviQuestPopupSystem.GetTextFromState();

            SoundEngine.PlaySound(FargosSoundRegistry.Victory with { Pitch = 0f, Volume = 3f });
        }

        public override void OnClose()
        {
            DeviQuestPopupSystem.ResetState();
        }

        public override void Update(GameTime gameTime)
        {
            OpenTime++;
            RemoveAllChildren();

            UIPanel panel = new UIPanel();
            yOffset = 50f * (float)Math.Min(1, 4 * Math.Sin(MathHelper.TwoPi * OpenTime / 360));
            panel.Top.Set(Main.screenHeight - yOffset, 0);
            panel.Left.Set(0, 0.15f);
            panel.Width.Set(15 * Main.UIScale * displayText.Length, 0);
            panel.Height.Set(30 * Main.UIScale, 0);
            panel.BackgroundColor = Color.Gray * 0.5f;

            Append(panel);

            UIText title = new UIText(displayText, Main.UIScale);
            title.Left.Set(0,0);
            title.HAlign = 0.5f;
            title.VAlign = 0.5f;

            panel.Append(title);

            if (OpenTime > 180)
                FargoUIManager.Close(this);

            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }

    public class DeviQuestPopupSystem : ModSystem
    {
        private static PopupState popupState = PopupState.None;

        public enum PopupState {
            None = -1,
            QuestComplete,
            NewQuests
        }

        public static void CreatePopup(PopupState state)
        {
            popupState = state;
            FargoUIManager.Open<DeviQuestPopup>();
        }

        public static string GetTextFromState()
        {
            switch (popupState)
            {
                case PopupState.QuestComplete:
                    return Language.GetTextValue("Mods.FargowiltasSouls.DeviQuests.QuestComplete");
                case PopupState.NewQuests:
                    return Language.GetTextValue("Mods.FargowiltasSouls.DeviQuests.NewQuests");
                default:
                    return "{}";
            }
        }

        public static void ResetState()
        {
            popupState = PopupState.None;
        }
    }
}
