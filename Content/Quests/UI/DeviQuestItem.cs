using Fargowiltas.Content.UI;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Quests.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.UI.Elements
{
    // Exists to be displayed as an item icon in the Toggler UI
    public class DeviQuestBoardItem : ModItem
    {
        public override string Texture => FargoAssets.GetAssetString("UI", "DeviQuestItem");

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
            CombinedUI.ToggleUI<DeviQuestBoard>();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<DeviatingEnergy>()).AddTile(TileID.Anvils).Register();
        }
    }
}