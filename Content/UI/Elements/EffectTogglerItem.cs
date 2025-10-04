using Fargowiltas.Content.UI;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.UI.Elements
{
    // Exists to be displayed as an item icon in the Toggler UI
    public class EffectTogglerItem : ModItem
    {
        public override string Texture => FargoAssets.GetAssetString("UI", "EffectTogglerItem");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.useAnimation = 4;
            Item.useTime = 4;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override bool? UseItem(Player player)
        {
            CombinedUI.ToggleUI<SoulToggler>();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(RecipeGroupID.IronBar).AddTile(TileID.Anvils).Register();
        }
    }
}