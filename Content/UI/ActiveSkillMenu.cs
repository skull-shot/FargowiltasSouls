using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Header = FargowiltasSouls.Core.Toggler.Header;

namespace FargowiltasSouls.Content.UI
{
    public class ActiveSkillMenu : UIState
    {
        public static int BackWidth => 300;
        public static int BackHeight => 520;

        public FargoUIDragablePanel BackPanel;
        public UIPanel EquippedPanel;
        public UIPanel AvailablePanel;

        public override void OnInitialize()
        {
            Vector2 offset = new(Main.screenWidth / 2f + BackWidth / 2, Main.screenHeight / 2f - 200);

            BackPanel = new FargoUIDragablePanel();
            BackPanel.Left.Set(-BackWidth - 300, 1f);
            BackPanel.Top.Set(offset.Y, 0);
            BackPanel.Width.Set(BackWidth, 0);
            BackPanel.Height.Set(BackHeight, 0);
            BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = BackPanel.PaddingBottom = 0;
            BackPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            float equippedPanelHeight = 80;
            float outline = 6;

            EquippedPanel = new UIPanel();
            EquippedPanel.Width.Set(BackWidth - outline * 2, 0);
            EquippedPanel.Height.Set(equippedPanelHeight, 0);
            EquippedPanel.Left.Set(outline, 0);
            EquippedPanel.Top.Set(outline, 0);
            EquippedPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            AvailablePanel = new UIPanel();
            AvailablePanel.Width.Set(BackWidth - outline * 2, 0);
            AvailablePanel.Height.Set(BackHeight - equippedPanelHeight - outline * 3, 0);
            AvailablePanel.Left.Set(outline, 0);
            AvailablePanel.Top.Set(equippedPanelHeight + outline * 2, 0);
            AvailablePanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            Append(BackPanel);
            BackPanel.Append(EquippedPanel);
            BackPanel.Append(AvailablePanel);

            var title = new UIText("Active Skills");
            title.Left.Set(-50, 0.5f);
            title.Top.Set(5, 0);
            BackPanel.Append(title);

            var title2 = new UIText("Available Skills");
            title2.Left.Set(-60, 0.5f);
            title2.Top.Set(AvailablePanel.Top.Pixels, 0);
            BackPanel.Append(title2);

            // Equipped boxes
            float spacing = 6;
            float boxWidth = 42;
            float boxXAmt = 3; 
            for (int x = 0; x < boxXAmt; x++)
            {
                var panel = new UIPanel();
                panel.Width.Set(boxWidth, 0);
                panel.Height.Set(boxWidth, 0);
                panel.Left.Set(spacing * (x + 1) + boxWidth * x, 0);
                panel.Top.Set(spacing, 0);
                EquippedPanel.Append(panel);
            }

            var equippedEffects = Main.LocalPlayer.AccessoryEffects().EquippedEffects;
            int skills = 0;
            // Available boxes
            for (int i = 0; i < AccessoryEffectLoader.AccessoryEffects.Count; i++)
            {
                if (equippedEffects[i] && AccessoryEffectLoader.AccessoryEffects[i].ActiveSkill)
                    skills++;
            }
            float panelWidth = BackWidth - outline * 2;
            boxXAmt = ((panelWidth - spacing) / (spacing + boxWidth)) - 1; // amount of boxes per row
            float height = spacing;
            while (height < (BackHeight - equippedPanelHeight - outline * 3) - boxWidth - spacing && skills > 0)
            {
                for (int x = 0; x < boxXAmt; x++)
                {
                    var panel = new UIPanel();
                    panel.Width.Set(boxWidth, 0);
                    panel.Height.Set(boxWidth, 0);
                    panel.Left.Set(spacing * (x + 1) + boxWidth * x, 0);
                    panel.Top.Set(height, 0);
                    AvailablePanel.Append(panel);
                    skills -= 1;
                    if (skills <= 0)
                        break;
                }
                height += boxWidth + spacing;
            }

            base.OnInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
