using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class CoffinPainting : ModItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Placables", "CoffinPainting");
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 34;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.consumable = true;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
            Item.createTile = ModContent.TileType<CoffinPaintingSheet>();
        }
    }
}
