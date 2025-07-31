using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class TophatSquirrelBanner : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Placables", "TophatSquirrelBanner");
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 10, 0);
            Item.createTile = ModContent.TileType<FMMBanner>();
            Item.placeStyle = 0;
        }
    }
}