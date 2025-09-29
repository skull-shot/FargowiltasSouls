using Fargowiltas.Content.UI;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI
{
    public class ActiveSkillMenuButton : FargoUI
    {
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex - 1;
        public override string InterfaceLayerName => "Fargos: Active Skill Menu Button";
        public UIImage Icon;
        public FargoUIHoverTextImageButton IconHighlight;
        //public const int x = 495;
        //public const int y = -112;
        public override void UpdateUI()
        {
            if (!Main.playerInventory || Main.LocalPlayer.chest != -1)
                FargoUIManager.Close<ActiveSkillMenuButton>();
            else
                FargoUIManager.Open<ActiveSkillMenuButton>();
        }
        public override void OnOpen()
        {
            RemoveAllChildren();
            OnInitialize();
        }
        public override void OnInitialize()
        {
            const int x = 497;
            const int y = 315;
            Icon = new UIImage(FargoAssets.UI.ActiveSkillMenu.ActiveSkillMenuButton.Value);
            Icon.Left.Set(x, 0);
            Icon.Top.Set(y, 0);
            Append(Icon);

            IconHighlight = new FargoUIHoverTextImageButton(FargoAssets.UI.ActiveSkillMenu.ActiveSkillMenuButtonHover.Asset, Language.GetTextValue("Mods.FargowiltasSouls.UI.ActiveSkillMenuButton"));
            IconHighlight.Left.Set(0, 0);
            IconHighlight.Top.Set(0, 0);
            IconHighlight.SetVisibility(1f, 0);
            IconHighlight.OnLeftClick += IconHighlight_OnClick;
            Icon.Append(IconHighlight);
        }
        private void IconHighlight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!Main.playerInventory || Main.LocalPlayer.chest == -1)
            {
                return;
            }

            FargoUIManager.Toggle<ActiveSkillMenu>();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.playerInventory && Main.LocalPlayer.chest == -1)
            {
                Icon.Draw(spriteBatch);
                IconHighlight.Draw(spriteBatch);
            }

        }
    }
}
