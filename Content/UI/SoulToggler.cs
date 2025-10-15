﻿using Fargowiltas.Assets.Textures;
using Fargowiltas.Common.Configs;
using Fargowiltas.Content.UI;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Header = FargowiltasSouls.Core.Toggler.Header;
using UISearchBar = Fargowiltas.Content.UI.UISearchBar;

namespace FargowiltasSouls.Content.UI
{
    public class SoulToggler : FargoUI
    {
        public override bool MenuToggleSound => true;
        public override string InterfaceLayerName => "Fargos: Soul Toggler";
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex - 1;

        public readonly static Regex RemoveItemTags = new(@"\[[^\[\]]*\]");

        public bool NeedsToggleListBuilding;
        public string DisplayMod;
        public string SortCategory;

        public const int BackWidth = 400;
        public const int BackHeight = 658;

        public UIDragablePanel BackPanel;
        public UIPanel InnerPanel;
        public UIPanel PresetPanel;
        public UIScrollbarClamped Scrollbar;
        public UIToggleList ToggleList;
        public UISearchBar SearchBar;
        public UICloseButton CloseButton;

        public FargoUIPresetButton OffButton;
        public FargoUIPresetButton OnButton;
        public FargoUIPresetButton MinimalButton;
        public FargoUIPresetButton SomeEffectsButton;
        public FargoUIPresetButton[] CustomButton = new FargoUIPresetButton[3];
        public FargoUIDisplayAllButton DisplayAllButton;
        //public FargoUIReloadButton ReloadButton;

        public override void OnLoad()
        {
            CombinedUI.AddUI<SoulToggler>(Language.GetText("Mods.FargowiltasSouls.UI.SoulToggler"), 3);
        }
        public override void UpdateUI()
        {
            if (!Main.playerInventory && FargoClientConfig.Instance.HideTogglerWhenInventoryIsClosed)
                FargoUIManager.Close<SoulToggler>();
        }
        public override void OnOpen()
        {
            NeedsToggleListBuilding = true;
        }
        public override void OnClose()
        {
            if (FargoClientConfig.Instance.ToggleSearchReset)
            {
                SearchBar.Input = "";

            }
            NeedsToggleListBuilding = true;
        }

        public override void OnInitialize()
        {
            Vector2 baseOffset = CombinedUI.CenterRight;
            Vector2 offset = new(baseOffset.X, baseOffset.Y - BackHeight / 2f);

            NeedsToggleListBuilding = true;
            DisplayMod = "";
            SortCategory = "";

            // This entire layout is cancerous and dangerous to your health because red protected UIElements children
            // If I want to give extra non-children to BackPanel to count as children when seeing if it should drag, I have to abandon
            // all semblence of organization in favour of making it work. Enjoy my write only UI laying out.
            // Oh well, at least it works...

            Scrollbar = new UIScrollbarClamped();
            Scrollbar.SetView(200f, 1000f);
            Scrollbar.Width.Set(20, 0);
            Scrollbar.OverflowHidden = true;
            Scrollbar.OnScrollWheel += HotbarScrollFix;

            ToggleList = [];
            ToggleList.SetScrollbar(Scrollbar);
            ToggleList.OnScrollWheel += HotbarScrollFix;

            BackPanel = new UIDragablePanel(Scrollbar, ToggleList);
            BackPanel.Left.Set(offset.X, 0);
            BackPanel.Top.Set(offset.Y, 0);
            BackPanel.Width.Set(BackWidth, 0);
            BackPanel.Height.Set(BackHeight, 0);
            BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = BackPanel.PaddingBottom = 0;
            BackPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            InnerPanel = new UIPanel();
            InnerPanel.Width.Set(BackWidth - 12, 0);
            InnerPanel.Height.Set(BackHeight - 70, 0);
            InnerPanel.Left.Set(6, 0);
            InnerPanel.Top.Set(32, 0);
            InnerPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            SearchBar = new UISearchBar(BackWidth - 8, 26);
            SearchBar.Left.Set(4, 0);
            SearchBar.Top.Set(4, 0);
            SearchBar.OnTextChange += SearchBar_OnTextChange;

            ToggleList.Width.Set(InnerPanel.Width.Pixels - InnerPanel.PaddingLeft * 2f - Scrollbar.Width.Pixels, 0);
            ToggleList.Height.Set(InnerPanel.Height.Pixels - InnerPanel.PaddingTop * 2f, 0);

            Scrollbar.Height.Set(InnerPanel.Height.Pixels - 16, 0);
            Scrollbar.Left.Set(InnerPanel.Width.Pixels - Scrollbar.Width.Pixels - 18, 0);

            PresetPanel = new UIPanel();
            PresetPanel.Left.Set(5, 0);
            PresetPanel.Top.Set(SearchBar.Height.Pixels + InnerPanel.Height.Pixels + 8, 0);
            PresetPanel.Width.Set(BackWidth - 10, 0);
            PresetPanel.Height.Set(32, 0);
            PresetPanel.PaddingTop = PresetPanel.PaddingBottom = 0;
            PresetPanel.PaddingLeft = PresetPanel.PaddingRight = 0;
            PresetPanel.BackgroundColor = new Color(74, 95, 172);

            OffButton = new FargoUIPresetButton(FargoMutantAssets.UI.Toggler.PresetOff.Value, (toggles) =>
            {
                toggles.SetAll(false);
            }, () => Language.GetTextValue("Mods.FargowiltasSouls.UI.TurnAllTogglesOff"), () => Main.LocalPlayer.FargoSouls().Toggler);
            OffButton.Top.Set(6, 0);
            OffButton.Left.Set(8, 0);

            OnButton = new FargoUIPresetButton(FargoMutantAssets.UI.Toggler.PresetOn.Value, (toggles) =>
            {
                toggles.SetAll(true);
            }, () => Language.GetTextValue("Mods.FargowiltasSouls.UI.TurnAllTogglesOn"), () => Main.LocalPlayer.FargoSouls().Toggler);
            OnButton.Top.Set(6, 0);
            OnButton.Left.Set(30, 0);

            SomeEffectsButton = new FargoUIPresetButton(FargoMutantAssets.UI.Toggler.PresetMinimal.Value, (toggles) =>
            {
                toggles.SomeEffects();
            }, () => Language.GetTextValue("Mods.FargowiltasSouls.UI.SomeEffectsPreset"), () => Main.LocalPlayer.FargoSouls().Toggler);
            SomeEffectsButton.Top.Set(6, 0);
            SomeEffectsButton.Left.Set(52, 0);

            MinimalButton = new FargoUIPresetButton(FargoMutantAssets.UI.Toggler.PresetMinimal.Value, (toggles) =>
            {
                toggles.MinimalEffects();
            }, () => Language.GetTextValue("Mods.FargowiltasSouls.UI.MinimalEffectsPreset"), () => Main.LocalPlayer.FargoSouls().Toggler);
            MinimalButton.Top.Set(6, 0);
            MinimalButton.Left.Set(74, 0);

            CloseButton = new UICloseButton();
            CloseButton.Left.Set(-18, 1f);
            CloseButton.Top.Set(-2, 0);
            CloseButton.OnLeftClick += CloseButton_OnLeftClick;

            Append(BackPanel);
            BackPanel.Append(InnerPanel);
            BackPanel.Append(SearchBar);
            BackPanel.Append(PresetPanel);
            InnerPanel.Append(Scrollbar);
            InnerPanel.Append(ToggleList);
            PresetPanel.Append(OffButton);
            PresetPanel.Append(OnButton);
            PresetPanel.Append(SomeEffectsButton);
            PresetPanel.Append(MinimalButton);
            BackPanel.Append(CloseButton);

            const int xOffset = 74; //ensure this matches the Left.Set of preceding button
            for (int i = 0; i < SoulToggleBackend.CustomPresetCount; i++)
            {
                int slot = i + 1;
                CustomButton[i] = new FargoUIPresetButton(FargoMutantAssets.UI.Toggler.PresetCustom.Value,
                toggles => toggles.LoadCustomPreset(slot),
                toggles => toggles.SaveCustomPreset(slot),
                () => Language.GetTextValue("Mods.FargowiltasSouls.UI.CustomPreset", slot), () => Main.LocalPlayer.FargoSouls().Toggler);
                CustomButton[i].Top.Set(6, 0);
                CustomButton[i].Left.Set(xOffset + 22 * slot, 0);
                PresetPanel.Append(CustomButton[i]);

                if (slot == SoulToggleBackend.CustomPresetCount) //after last panel is loaded, load reload button
                {
                    DisplayAllButton = new FargoUIDisplayAllButton(FargoMutantAssets.UI.Toggler.DisplayAllButton.Value,
                        () => Language.GetTextValue("Mods.FargowiltasSouls.UI.DisplayAll"),
                        () => Language.GetTextValue("Mods.FargowiltasSouls.UI.DisplayEquipped"));
                    DisplayAllButton.OnLeftClick += DisplayAllButton_OnLeftClick;
                    DisplayAllButton.Top.Set(6, 0);
                    DisplayAllButton.Left.Set(xOffset + 22 * (slot + 1), 0);
                    PresetPanel.Append(DisplayAllButton);
                }

            }

            base.OnInitialize();
        }
        private void CloseButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            FargoUIManager.Close<SoulToggler>();
        }
        private void DisplayAllButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            DisplayAllButton.DisplayAll = !DisplayAllButton.DisplayAll;
            NeedsToggleListBuilding = true;
        }
        private void SearchBar_OnTextChange(string oldText, string currentText) => NeedsToggleListBuilding = true;

        private void HotbarScrollFix(UIScrollWheelEvent evt, UIElement listeningElement) => Main.LocalPlayer.ScrollHotbar(PlayerInput.ScrollWheelDelta / 120);
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.mouseInterface && (Main.mouseLeft || Main.mouseRight))
            {
                NeedsToggleListBuilding = true;
            }
            base.Update(gameTime);
            FargoSoulsPlayer modPlayer = Main.LocalPlayer.FargoSouls();
            if (NeedsToggleListBuilding && modPlayer.ToggleRebuildCooldown <= 0)
            {
                BuildList();
                NeedsToggleListBuilding = false;
                modPlayer.ToggleRebuildCooldown = 30;
            }
        }

        public void BuildList()
        {
            ToggleList.Clear();
            Player player = Main.LocalPlayer;
            SoulToggleBackend toggler = player.FargoSouls().Toggler;
            AccessoryEffectPlayer effectPlayer = player.AccessoryEffects();


            bool alwaysDisplay = DisplayAllButton.DisplayAll;

            bool SearchMatches(string[] words) => words.Any(s => s.StartsWith(SearchBar.Input, StringComparison.OrdinalIgnoreCase));

            IEnumerable<Header> LoadedHeaders = ToggleLoader.LoadedHeaders;

            bool hasMinions = false;
            bool hasExtraAttacks = false;
            bool hasExtraJumps = false;
            foreach (ExtraJump jump in ExtraJumpLoader.OrderedJumps)
            {
                ExtraJumpState state = player.GetJumpState(jump);
                if (state.Enabled)
                {
                    hasExtraJumps = true;
                    break;
                }
            }

            var deactivatedMinions = AccessoryEffectLoader.GetEffect<MinionsDeactivatedEffect>();

            for (int i = 0; i < AccessoryEffectLoader.AccessoryEffects.Count; i++)
            {
                if (effectPlayer.EquippedEffects[i] && AccessoryEffectLoader.AccessoryEffects[i].MinionEffect)
                    hasMinions = true;
                if (effectPlayer.EquippedEffects[i] && AccessoryEffectLoader.AccessoryEffects[i].ExtraAttackEffect)
                    hasExtraAttacks = true;
                if (effectPlayer.EquippedEffects[i] && AccessoryEffectLoader.AccessoryEffects[i].ExtraJumpEffect)
                    hasExtraJumps = true;
            }
            if (effectPlayer.EquippedEffects[deactivatedMinions.Index])
                ToggleList.Add(new UIToggle(deactivatedMinions, deactivatedMinions.Mod.Name));
            else if (hasMinions)
                ToggleList.Add(new MinionsToggle());
            if (hasExtraAttacks)
                ToggleList.Add(new ExtraAttacksToggle());
            if (hasExtraJumps)
                ToggleList.Add(new ExtraJumpsDisabledToggle());

            DisplayToggles(LoadedHeaders.OrderBy(h => h.Priority));

            void DisplayToggles(IEnumerable<Header> headers)
            {
                foreach (Header header in headers)
                {
                    string[] headerWords = header.GetRawToggleName().Split(' ');
                    IEnumerable<Toggle> headerToggles = toggler.Toggles.Values.Where((toggle) =>
                    {
                        string[] words = toggle.GetRawToggleName().Split(' ');
                        return
                        (effectPlayer.Equipped(toggle.Effect) || alwaysDisplay) &&
                        toggle.Header == header &&
                        toggle.Effect.Index != deactivatedMinions.Index &&
                        (string.IsNullOrEmpty(DisplayMod) || toggle.Mod == DisplayMod) &&
                        (string.IsNullOrEmpty(SortCategory) || toggle.Category == SortCategory) &&
                        (string.IsNullOrEmpty(SearchBar.Input) || SearchMatches(words) || SearchMatches(headerWords));
                    });
                    if (!headerToggles.Any())
                        continue;
                    if (ToggleList.Count > 0) // Don't add for the first header
                        ToggleList.Add(new UIText("", 0.2f)); // Blank line

                    (string text, int item) = (header.HeaderDescription, header.Item);
                    ToggleList.Add(new FargoUIHeader(text, header.Mod.Name, item, (BackWidth - 16, 20)));
                    foreach (Toggle toggle in headerToggles)
                    {
                        ToggleList.Add(new UIToggle(toggle.Effect, toggle.Mod));
                    }
                }
            }
            if (ToggleList.Count == 0) // empty, no toggles
            {
                ToggleList.Clear();
                ToggleList.Add(new FargoUIHeader($"[i:{ModContent.ItemType<TogglerIconItem>()}] {Language.GetTextValue("Mods.FargowiltasSouls.UI.NoToggles")}", FargowiltasSouls.Instance.Name, ModContent.ItemType<TogglerIconItem>(), (BackWidth - 16, 20)));
            }

            //old
            /*
            foreach (Toggle toggle in DisplayToggles)
            {
                if (ToggleLoader.LoadedHeaders.ContainsKey(toggle.InternalName) && SearchBar.IsEmpty)
                {
                    if (ToggleList.Count > 0) // Don't add for the first header
                        ToggleList.Add(new UIText("", 0.2f)); // Blank line

                    (string name, int item) = ToggleLoader.LoadedHeaders[toggle.InternalName];
                    ToggleList.Add(new FargoUIHeader(name, toggle.Mod, item, (BackWidth - 16, 20)));
                }
                else if (!SearchBar.IsEmpty)
                {
                    int index = togglesAsLists.FindIndex(t => t.InternalName == toggle.InternalName);
                    int closestHeader = ToggleLoader.HeaderToggles.OrderBy(i =>
                        Math.Abs(index - i)).First();

                    if (closestHeader > index)
                        closestHeader = ToggleLoader.HeaderToggles[ToggleLoader.HeaderToggles.FindIndex(i => i == closestHeader) - 1];

                    (string name, int item) = ToggleLoader.LoadedHeaders[togglesAsLists[closestHeader].InternalName];

                    if (!usedHeaders.Contains(name))
                    {
                        if (ToggleList.Count > 0) // Don't add for the first header
                            ToggleList.Add(new UIText("", 0.2f)); // Blank line

                        ToggleList.Add(new FargoUIHeader(name, toggle.Mod, item, (BackWidth - 16, 20)));
                        usedHeaders.Add(name);
                    }
                }
                ToggleList.Add(new UIToggle(toggle.InternalName, toggle.Mod));
            }
            */
        }

        /*public void SetPositionToPoint(Point point)
        {
            BackPanel.Left.Set(point.X, 0);
            BackPanel.Top.Set(point.Y, 0);
        }

        public Point GetPositionAsPoint() => new Point((int)BackPanel.Left.Pixels, (int)BackPanel.Top.Pixels);*/
    }
}
