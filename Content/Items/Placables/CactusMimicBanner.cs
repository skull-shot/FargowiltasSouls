using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class CactusMimicBanner : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Placables", "CactusMimicBanner");
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.CactusMimicBannerSheet>());
            Item.width = 14;
            Item.height = 36;
            Item.SetShopValues(ItemRarityColor.Blue1, Terraria.Item.buyPrice(silver: 10));
        }
    }
}
