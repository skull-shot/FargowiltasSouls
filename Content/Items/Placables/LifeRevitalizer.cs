using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class LifeRevitalizer : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Placables", "LifeRevitalizer");
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Expert;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.LifeRevitalizerPlaced>();

            Item.expert = true;
        }
    }
}