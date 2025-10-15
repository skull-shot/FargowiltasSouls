using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Summons
{
    public class ChampionySigil : SigilOfChampions
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Summons", "SigilOfChampions");

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player) => true;

        public override bool ConsumeItem(Player player) => player.altFunctionUse != 2;

        public override void AddRecipes() { }
    }
}