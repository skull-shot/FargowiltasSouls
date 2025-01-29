using FargowiltasSouls.Assets.UI;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Essences;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using Humanizer;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static FargowiltasSouls.Content.UI.Elements.DifficultyOption;

namespace FargowiltasSouls.Content.UI
{
    public class DifficultySelectionMenu : FargoUI
    {
        public override bool MenuToggleSound => true;
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex + 1;
        public override string InterfaceLayerName => "Fargos: Difficulty Selection Menu";

        public const float GoldenRatio = 1.618033988749f;
        public const int BackHeight = 100;
        public const int BackWidth = (int)(BackHeight * GoldenRatio);

        public UIPanel BackPanel;
        public UIPanel VanillaPanel;
        public UIPanel EternityPanel;
        public UIPanel MasoPanel;

        public int OpenTime = 0;

        public static MethodInfo JourneyMode_SetValue;
        public override void OnLoad()
        {
            JourneyMode_SetValue = typeof(CreativePowers.DifficultySliderPower).GetMethod("SetValueKeyboardForced", LumUtils.UniversalBindingFlags);
        }
        public override void UpdateUI()
        {
            if (Main.gameMenu)
                FargoUIManager.Close(this);
        }
        public override void OnOpen()
        {
            UpdateElements();
            OpenTime = 0;
        }
        public override void OnClose()
        {
            
        }
        public void UpdateElements()
        {
            BackPanel.RemoveAllChildren();

            var title = new UIText(Language.GetTextValue("Mods.FargowiltasSouls.UI.SelectDifficulty"));
            title.Left.Set(-57, 0.5f);
            title.Top.Set(5, 0);
            BackPanel.Append(title);

            float halfWidth = -BackWidth / 2;
            AddNewOption(new VanillaDifficultyOption(), halfWidth * 0.6f);
            AddNewOption(new EternityDifficultyOption(), 0);
            AddNewOption(new MasoDifficultyOption(), -halfWidth * 0.6f);
        }
        public DifficultyOption AddNewOption(DifficultyOption option, float centerX)
        {
            const int OptionSize = 40;
            option.Left.Set(centerX - OptionSize / 2, 0.5f);
            option.Top.Set(-OptionSize / 2, 0.5f);
            option.Width.Set(OptionSize, 0);
            option.Height.Set(OptionSize, 0);
            BackPanel.Append(option);
            return option;
        }
        public override void OnInitialize()
        {

            Vector2 offset = new(-BackWidth / 2, -BackHeight / 2);

            BackPanel = new UIPanel();
            BackPanel.Left.Set(offset.X, 0.5f);
            BackPanel.Top.Set(offset.Y, 0.5f);
            BackPanel.Width.Set(BackWidth, 0);
            BackPanel.Height.Set(BackHeight, 0);
            BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = BackPanel.PaddingBottom = 0;
            BackPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            Append(BackPanel);

            base.OnInitialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (OpenTime < 30)
                OpenTime++;
            if (Main.mouseItem.type != ItemID.None)
                OpenTime = 0;
            if (!BackPanel.IsMouseHovering)
            {
                if (Main.mouseLeft && OpenTime >= 30)
                    FargoUIManager.Close(this);
            }
            else
                Main.LocalPlayer.mouseInterface = true;
        }
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }
    }
}
