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
    public class OncomingMutantManager : FargoUI
    {
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex;
        public override string InterfaceLayerName => "Fargos: Oncoming Mutant Manager";

        public UIOncomingMutant OncomingMutant;
        public override void OnLoad()
        {
            FargoUIManager.Open<OncomingMutantManager>();
        }
        public override void UpdateUI()
        {
            if (!Main.playerInventory || Main.LocalPlayer.chest != -1)
                FargoUIManager.Close<OncomingMutantManager>();
            else
                FargoUIManager.Open<OncomingMutantManager>();
        }
        public override void OnActivate()
        {
            const int x = 570;
            const int y = 250;

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
            OncomingMutant.Left.Set(x, 0);
            OncomingMutant.Top.Set(y, 0);
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

            //FargoUIManager.Toggle<SoulToggler>();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.playerInventory && Main.LocalPlayer.chest == -1)
            {
                //base.Draw(spriteBatch);
                OncomingMutant.Draw(spriteBatch);
            }
        }
    }
}
