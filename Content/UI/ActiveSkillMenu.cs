﻿using Fargowiltas.Content.UI;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI
{
    public class ActiveSkillMenu : FargoUI
    {
        public override bool MenuToggleSound => true;
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex + 1;
        public override string InterfaceLayerName => "Fargos: Active Skill Menu";
        public const int BackWidth = 300;
        public const int BackHeight = 520;
        public static int EquippedPanelHeight => 80;
        public static int DragPanelHeight => 20;
        public static int Outline => 6;

        public UIPanel BackPanel;
        public UIActiveSkillMenuDrag DragPanel;
        public UIPanel EquippedPanel;
        public UIPanel AvailablePanel;
        public UICloseButton CloseButton;
        public UIOpenKeybindsButton OpenKeybindsButton;

        public static ActiveSkillBox MouseHeldElement = null;
        public static ActiveSkillBox MouseHoveredElement = null;
        public static bool ShouldRefresh;
        public override void OnLoad()
        {
            CombinedUI.AddUI<ActiveSkillMenu>(Language.GetText("Mods.FargowiltasSouls.UI.ActiveSkillMenu"), 4);
        }
        public override void UpdateUI()
        {
            if (Main.gameMenu || !Main.playerInventory)
                FargoUIManager.Close<ActiveSkillMenu>();
        }
        public override void OnOpen()
        {
            Main.playerInventory = true;
            RemoveAllChildren();
            OnInitialize();
            //UpdateSkillList();
        }
        public override void OnClose()
        {
            if (DragPanel.dragging)
                DragPanel.DragEnd(Main.MouseScreen);
        }

        public override void OnInitialize()
        {
            var config = ClientConfig.Instance;
            var baseOffset = CombinedUI.CenterRight;
            if (config.ActiveSkillMenuX == 0f)
            {
                config.ActiveSkillMenuX = baseOffset.X;
                config.OnChanged();
            }
            if (config.ActiveSkillMenuY == 0f)
            {
                config.ActiveSkillMenuY = baseOffset.Y - 72;
                config.OnChanged();
            }
                
            Vector2 offset = new(config.ActiveSkillMenuX, config.ActiveSkillMenuY);

            BackPanel = new UIPanel();
            BackPanel.Left.Set(offset.X, 0f);
            BackPanel.Top.Set(offset.Y, 0);
            BackPanel.Width.Set(BackWidth, 0);
            BackPanel.Height.Set(BackHeight, 0);
            BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = BackPanel.PaddingBottom = 0;
            BackPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            DragPanel = new();
            DragPanel.Width.Set(BackWidth, 0);
            DragPanel.Height.Set(DragPanelHeight, 0);
            DragPanel.Left.Set(0, 0);
            DragPanel.Top.Set(0, 0);

            EquippedPanel = new UIPanel();
            EquippedPanel.Width.Set(BackWidth - Outline * 2, 0);
            EquippedPanel.Height.Set(EquippedPanelHeight, 0);
            EquippedPanel.Left.Set(Outline, 0);
            EquippedPanel.Top.Set(Outline, 0);
            EquippedPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            AvailablePanel = new UIPanel();
            AvailablePanel.Width.Set(BackWidth - Outline * 2, 0);
            AvailablePanel.Height.Set(BackHeight - EquippedPanelHeight - Outline * 3, 0);
            AvailablePanel.Left.Set(Outline, 0);
            AvailablePanel.Top.Set(EquippedPanelHeight + Outline * 2, 0);
            AvailablePanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            CloseButton = new UICloseButton();
            CloseButton.Left.Set(-18, 1f);
            CloseButton.Top.Set(-2, 0);
            CloseButton.OnLeftClick += CloseButton_OnLeftClick;

            var keybindsTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Inputs", (AssetRequestMode)1);
            OpenKeybindsButton = new UIOpenKeybindsButton(keybindsTexture, keybindsTexture.Frame(2, 2))
            {
                VAlign = 0f,
                HAlign = 0f
            };
            OpenKeybindsButton.Left.Set(-100, 1f);
            OpenKeybindsButton.Top.Set(30f, 0f);
            OpenKeybindsButton.OnLeftClick += OpenKeybindsButton_OnLeftClick;


            Append(BackPanel);
            BackPanel.Append(DragPanel);
            BackPanel.Append(EquippedPanel);
            BackPanel.Append(AvailablePanel);
            BackPanel.Append(CloseButton);
            BackPanel.Append(OpenKeybindsButton);

            var title = new UIText(Language.GetTextValue("Mods.FargowiltasSouls.UI.ActiveSkills"));
            title.Left.Set(-50, 0.5f);
            title.Top.Set(5, 0);
            BackPanel.Append(title);

            var title2 = new UIText(Language.GetTextValue("Mods.FargowiltasSouls.UI.AvailableSkills"));
            title2.Left.Set(-60, 0.5f);
            title2.Top.Set(AvailablePanel.Top.Pixels, 0);
            BackPanel.Append(title2);

            UpdateSkillList();

            base.OnInitialize();
        }
        private void CloseButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            FargoUIManager.Close<ActiveSkillMenu>();
        }
        private void OpenKeybindsButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            IngameFancyUI.OpenKeybinds();
        }
        public void UpdateSkillList()
        {
            if ((Main.gameMenu || Main.dedServ))
            {
                FargoUIManager.Close<ActiveSkillMenu>();
                return;
            }
            EquippedPanel.RemoveAllChildren();
            AvailablePanel.RemoveAllChildren();
            MouseHeldElement = null;

            FargoSoulsPlayer sPlayer = Main.LocalPlayer.FargoSouls();
            AccessoryEffectPlayer aPlayer = Main.LocalPlayer.AccessoryEffects();

            /*
            for (int i = 0; i < sPlayer.ActiveSkills.Length; i++)
            {
                var skill = sPlayer.ActiveSkills[i];
                if (skill != null && !aPlayer.Equipped(skill))
                {
                    sPlayer.ActiveSkills[i] = null;

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        sPlayer.SyncActiveSkill(i);
                }
            }
            */

            // Equipped boxes
            float spacing = 6;
            float boxWidth = 42;
            float boxXAmt = 3;
            for (int x = 0; x < boxXAmt; x++)
            {
                var panel = new EquippedSkillBox(x, boxWidth);
                panel.Width.Set(boxWidth, 0);
                panel.Height.Set(boxWidth, 0);
                panel.Left.Set(spacing * (x + 1) + boxWidth * x, 0);
                panel.Top.Set(spacing, 0);
                EquippedPanel.Append(panel);
            }
            var equippedEffects = aPlayer.EquippedEffects;
            Queue<AccessoryEffect> skillList = [];
            // Available boxes
            for (int i = 0; i < AccessoryEffectLoader.AccessoryEffects.Count; i++)
            {
                var effect = AccessoryEffectLoader.AccessoryEffects[i];
                if (equippedEffects[i] && effect.ActiveSkill && !sPlayer.ActiveSkills.Contains(effect))
                {
                    skillList.Enqueue(effect);
                }

            }

            float panelWidth = BackWidth - Outline * 2;
            boxXAmt = ((panelWidth - spacing) / (spacing + boxWidth)) - 1; // amount of boxes per row
            float height = spacing;
            while (height < (BackHeight - EquippedPanelHeight - Outline * 3) - boxWidth - spacing && skillList.Count > 0)
            {
                for (int x = 0; x < boxXAmt; x++)
                {
                    var skill = skillList.Dequeue();
                    var panel = new AvailableSkillBox(skill, skill.Mod.Name, boxWidth);
                    panel.Width.Set(boxWidth, 0);
                    panel.Height.Set(boxWidth, 0);
                    panel.Left.Set(spacing * (x + 1) + boxWidth * x, 0);
                    panel.Top.Set(height, 0);
                    AvailablePanel.Append(panel);

                    if (skillList.Count <= 0)
                        break;
                }
                height += boxWidth + spacing;
            }
            float newHeight = height + boxWidth - spacing * 3;
            float dif = newHeight - AvailablePanel.GetDimensions().Height;
            AvailablePanel.Height.Set(newHeight, 0);
            BackPanel.Height.Set(BackPanel.GetDimensions().Height + dif, 0);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!BackPanel.IsMouseHovering && (Main.mouseLeft || Main.mouseRight))
            {
                if (Main.LocalPlayer.mouseInterface)
                    ShouldRefresh = true;
            }
            if (ShouldRefresh)
            {
                UpdateSkillList();
                ShouldRefresh = false;
            }
        }
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
            if (MouseHoveredElement != null) // Layer hovered element last so it's not under anything
            {
                MouseHoveredElement.Draw(spriteBatch);
                MouseHoveredElement = null;
            }
        }
    }
}
