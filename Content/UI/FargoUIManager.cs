using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI
{
    public static class FargoUIManager
    {
        public static UserInterface TogglerUserInterface { get; private set; }

        public static UserInterface TogglerToggleUserInterface { get; private set; }

        public static UserInterface ActiveSkillUserInterface { get; private set; }

        public static UserInterface CooldownBarUserInterface { get; private set; }

        public static SoulToggler SoulToggler { get; private set; }

        public static SoulTogglerButton SoulTogglerButton { get; private set; }

        public static ActiveSkillMenu ActiveSkillMenu { get; private set; }

        public static CooldownBarManager CooldownBarManager { get; private set; }

        private static GameTime LastUpdateUIGameTime { get; set; }

        public static Asset<Texture2D> CheckMark { get; private set; }

        public static Asset<Texture2D> CheckBox { get; private set; }

        public static Asset<Texture2D> Cross { get; private set; }

        public static Asset<Texture2D> SoulTogglerButtonTexture { get; private set; }

        public static Asset<Texture2D> SoulTogglerButton_MouseOverTexture { get; private set; }

        public static Asset<Texture2D> PresetButtonOutline { get; private set; }

        public static Asset<Texture2D> PresetOffButton { get; private set; }

        public static Asset<Texture2D> PresetOnButton { get; private set; }

        public static Asset<Texture2D> PresetMinimalButton { get; private set; }

        public static Asset<Texture2D> PresetCustomButton { get; private set; }

        public static Asset<Texture2D> ReloadButtonTexture { get; private set; }
        public static Asset<Texture2D> DisplayAllButtonTexture { get; private set; }

        public static Asset<Texture2D> OncomingMutantTexture { get; private set; }

        public static Asset<Texture2D> OncomingMutantAuraTexture { get; private set; }

        public static Asset<Texture2D> OncomingMutantntTexture { get; private set; }

        public static Asset<Texture2D> CooldownBarTexture { get; private set; }

        public static Asset<Texture2D> CooldownBarFillTexture { get; private set; }

        public static void LoadUI()
        {
            if (!Main.dedServ)
            {
                // Load textures
                CheckMark = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CheckMark", AssetRequestMode.ImmediateLoad);
                CheckBox = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CheckBox", AssetRequestMode.ImmediateLoad);
                Cross = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/Cross", AssetRequestMode.ImmediateLoad);
                SoulTogglerButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/SoulTogglerToggle", AssetRequestMode.ImmediateLoad);
                SoulTogglerButton_MouseOverTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/SoulTogglerToggle_MouseOver", AssetRequestMode.ImmediateLoad);
                PresetButtonOutline = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOutline", AssetRequestMode.ImmediateLoad);
                PresetOffButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOff", AssetRequestMode.ImmediateLoad);
                PresetOnButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOn", AssetRequestMode.ImmediateLoad);
                PresetMinimalButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetMinimal", AssetRequestMode.ImmediateLoad);
                PresetCustomButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetCustom", AssetRequestMode.ImmediateLoad);
                DisplayAllButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/DisplayAllButton", AssetRequestMode.ImmediateLoad);
                ReloadButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/ReloadButton", AssetRequestMode.ImmediateLoad);
                OncomingMutantTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutant", AssetRequestMode.ImmediateLoad);
                OncomingMutantAuraTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutantAura", AssetRequestMode.ImmediateLoad);
                OncomingMutantntTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutantnt", AssetRequestMode.ImmediateLoad);
                CooldownBarTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CooldownBar", AssetRequestMode.ImmediateLoad);
                CooldownBarFillTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CooldownBarFill", AssetRequestMode.ImmediateLoad);

                // Initialize UserInterfaces
                TogglerUserInterface = new();
                TogglerToggleUserInterface = new();
                ActiveSkillUserInterface = new();
                CooldownBarUserInterface = new();

                // Activate UIs
                SoulToggler = new();
                SoulToggler.Activate();
                SoulTogglerButton = new();
                SoulTogglerButton.Activate();
                ActiveSkillMenu = new();
                ActiveSkillMenu.Activate();
                CooldownBarManager = new();
                CooldownBarManager.Activate();

                TogglerToggleUserInterface.SetState(SoulTogglerButton);
                CooldownBarUserInterface.SetState(CooldownBarManager);
            }
        }
        public static void UpdateUI(GameTime gameTime)
        {
            LastUpdateUIGameTime = gameTime;

            if (!Main.playerInventory && ClientConfig.Instance.HideTogglerWhenInventoryIsClosed)
                CloseSoulToggler();
            if (!Main.playerInventory)
                CloseSoulTogglerButton();
            else
                OpenSoulTogglerButton();
            if (Main.gameMenu)
                CloseActiveSkillMenu();

            if (TogglerUserInterface?.CurrentState != null)
                TogglerUserInterface.Update(gameTime);
            if (TogglerToggleUserInterface?.CurrentState != null)
                TogglerToggleUserInterface.Update(gameTime);
            if (ActiveSkillUserInterface.CurrentState != null)
                ActiveSkillUserInterface.Update(gameTime);
        }

        public static bool IsSoulTogglerOpen() => TogglerUserInterface?.CurrentState == null;

        public static void CloseSoulToggler()
        {
            TogglerUserInterface?.SetState(null);

            if (ClientConfig.Instance.ToggleSearchReset)
            {
                SoulToggler.SearchBar.Input = "";

            }
            SoulToggler.NeedsToggleListBuilding = true;
        }

        public static bool IsTogglerOpen() => TogglerUserInterface.CurrentState == SoulToggler;

        public static void OpenToggler() => TogglerUserInterface.SetState(SoulToggler);
        public static void CloseSoulTogglerButton() => TogglerToggleUserInterface.SetState(null);
        public static void OpenSoulTogglerButton() => TogglerToggleUserInterface.SetState(SoulTogglerButton);

        public static void ToggleActiveSkillMenu()
        {
            if (ActiveSkillUserInterface.CurrentState != null)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                var dragPanel = ((ActiveSkillMenu)ActiveSkillUserInterface.CurrentState)?.DragPanel;
                if (dragPanel.dragging)
                    dragPanel.DragEnd(Main.MouseScreen);

                CloseActiveSkillMenu();
            }
            else
            {
                ActiveSkillMenu = new();
                SoundEngine.PlaySound(SoundID.MenuOpen);
                OpenActiveSkillMenu();
            }
                
        }

        public static void OpenActiveSkillMenu() => ActiveSkillUserInterface.SetState(ActiveSkillMenu);
        public static void CloseActiveSkillMenu() => ActiveSkillUserInterface.SetState(null);

        public static void ToggleSoulToggler()
        {
            if (IsSoulTogglerOpen())
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                OpenToggler();
            }
            else if (IsTogglerOpen())
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                CloseSoulToggler();
            }
        }
        public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex((layer) => layer.Name == "Vanilla: Inventory");
            if (index != -1)
            {
                layers.Insert(index - 1, new LegacyGameInterfaceLayer("Fargos: Soul Toggler", delegate
                {
                    if (LastUpdateUIGameTime != null && TogglerUserInterface?.CurrentState != null)
                        TogglerUserInterface.Draw(Main.spriteBatch, LastUpdateUIGameTime);

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(index, new LegacyGameInterfaceLayer("Fargos: Soul Toggler Toggler", delegate
                {
                    if (LastUpdateUIGameTime != null && TogglerToggleUserInterface?.CurrentState != null)
                        TogglerToggleUserInterface.Draw(Main.spriteBatch, LastUpdateUIGameTime);

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(index + 1, new LegacyGameInterfaceLayer("Fargos: Cooldown Bars", delegate
                {
                    if (LastUpdateUIGameTime != null && CooldownBarUserInterface?.CurrentState != null)
                        CooldownBarUserInterface.Draw(Main.spriteBatch, LastUpdateUIGameTime);

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(index + 1, new LegacyGameInterfaceLayer("Fargos: Active Skill Menu", delegate
                {
                    if (LastUpdateUIGameTime != null && ActiveSkillUserInterface?.CurrentState != null)
                        ActiveSkillUserInterface.Draw(Main.spriteBatch, LastUpdateUIGameTime);

                    return true;
                }, InterfaceScaleType.UI));
                
            }
        }
    }
}
