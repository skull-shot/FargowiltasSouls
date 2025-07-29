using FargowiltasSouls.Assets.Textures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class MutantStatue : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Placables", "MutantStatue");
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.MutantStatue>();
        }

        public override void AddRecipes()
        {
            if (ModContent.TryFind("Fargowiltas/Mutant", out ModItem mutant))
            {
                CreateRecipe()
                  .AddIngredient(ItemID.StoneBlock, 50)
                  .AddIngredient(mutant)
                  .AddTile(TileID.HeavyWorkBench)
                  .Register();
            }
        }
    }
}