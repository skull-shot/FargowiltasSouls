using FargowiltasSouls.Assets.Textures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class TwentyTwoPainting : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Placables", "TwentyTwoPainting");
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.createTile = ModContent.TileType<TwentyTwoPaintingSheet>();
        }
    }
}