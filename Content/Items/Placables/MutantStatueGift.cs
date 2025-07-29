using FargowiltasSouls.Assets.Textures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class MutantStatueGift : MutantStatue
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Placables", "MutantStatueGift");
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Tiles.MutantStatueGift>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<MutantStatue>())
                .AddIngredient(ModContent.ItemType<Masochist>())
                .Register();
        }
    }
}