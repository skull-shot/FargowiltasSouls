using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Tiles;
using Terraria;
using Terraria.Enums;
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
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.TophatSquirrelBannerSheet>());
            Item.width = 14;
            Item.height = 36;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
        }
    }
}