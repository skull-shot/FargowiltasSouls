using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Materials
{
    public class AbomEnergy : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Materials", Name);
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<AbominableRarity>();
            Item.value = Item.sellPrice(0, 4, 0, 0);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}