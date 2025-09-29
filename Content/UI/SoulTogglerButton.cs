using Fargowiltas.Content.UI;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI
{
    public class SoulTogglerButton : FargoUI
    {
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex;
        public override string InterfaceLayerName => "Fargos: Soul Toggler Toggler";
        public UIImage Icon;
        public FargoUIHoverTextImageButton IconHighlight;
        public UIImage IconFlash;
        public UIOncomingMutant OncomingMutant;
        public override void OnLoad()
        {
            FargoUIManager.Open<SoulTogglerButton>();
        }
        public override void UpdateUI()
        {
            if (!Main.playerInventory || Main.LocalPlayer.chest != -1)
                FargoUIManager.Close<SoulTogglerButton>();
            else
                FargoUIManager.Open<SoulTogglerButton>();
        }
        public override void OnActivate()
        {
            const int x = 570;
            const int y = 275;

            IconFlash = new UIImage(FargoAssets.UI.Toggler.SoulTogglerButton_MouseOverTexture);
            IconFlash.Left.Set(x, 0);
            IconFlash.Top.Set(y, 0);
            Append(IconFlash);

            Icon = new UIImage(FargoAssets.UI.Toggler.SoulTogglerButtonTexture);
            Icon.Left.Set(x, 0); //26
            Icon.Top.Set(y, 0); //300
            Append(Icon);

            IconHighlight = new FargoUIHoverTextImageButton(FargoAssets.UI.Toggler.SoulTogglerButton_MouseOverTexture, Language.GetTextValue("Mods.FargowiltasSouls.UI.SoulTogglerButton"));
            IconHighlight.Left.Set(0, 0);
            IconHighlight.Top.Set(0, 0);
            IconHighlight.SetVisibility(1f, 0);
            IconHighlight.OnMouseOver += IconHighlight_MouseOver;
            IconHighlight.OnLeftClick += IconHighlight_OnClick;
            Icon.Append(IconHighlight);

            OncomingMutant = new UIOncomingMutant(FargoAssets.UI.OncomingMutantTexture.Value,
                FargoAssets.UI.OncomingMutantAura.Value,
                FargoAssets.UI.OncomingMutantnt.Value,
                Language.GetTextValue("Mods.FargowiltasSouls.UI.EternityEnabled"),
                Language.GetTextValue("Mods.FargowiltasSouls.UI.MasochistEnabled"),
                Language.GetTextValue("Mods.FargowiltasSouls.UI.EternityDisabled"),
                Language.GetTextValue("Mods.FargowiltasSouls.UI.RightClickSelectDifficulty"),
                Language.GetTextValue("Mods.FargowiltasSouls.UI.HoldShift"),
                Language.GetTextValue("Mods.FargowiltasSouls.UI.ExpandedEternity"),
                Language.GetTextValue("Mods.FargowiltasSouls.UI.ExpandedMasochist"),
                Language.GetTextValue("Mods.FargowiltasSouls.UI.ExpandedFeatures"),
                Language.GetTextValue("Mods.FargowiltasSouls.UI.MasochistMultiplayer")
                );
            OncomingMutant.Left.Set(610, 0);
            OncomingMutant.Top.Set(250, 0);
            Append(OncomingMutant);

            base.OnActivate();
        }
        private void IconHighlight_MouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            FargoUIManager.Get<SoulToggler>().NeedsToggleListBuilding = true;
        }
        private void IconHighlight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!Main.playerInventory || Main.LocalPlayer.chest != -1)
            {
                return;
            }

            FargoUIManager.Toggle<SoulToggler>();
            Main.LocalPlayer.FargoSouls().HasClickedWrench = true;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.playerInventory && Main.LocalPlayer.chest == -1)
            {
                //base.Draw(spriteBatch);

                Icon.Draw(spriteBatch);
                IconHighlight.Draw(spriteBatch);
                OncomingMutant.Draw(spriteBatch);
                if (!Main.LocalPlayer.FargoSouls().HasClickedWrench && Main.GlobalTimeWrappedHourly % 1f < 0.5f)
                {
                    if (Main.LocalPlayer.AccessoryEffects().EquippedEffects.Any(p => p))
                        IconFlash.Draw(spriteBatch);
                }
            }

        }
    }
}
