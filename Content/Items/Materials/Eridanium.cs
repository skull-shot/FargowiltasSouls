using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Materials
{
    public class Eridanium : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Materials", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Purple;
            Item.width = 12;
            Item.height = 12;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }
    }
}
