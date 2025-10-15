using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Summons
{
    public class SquirrelCoatofArms : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Summons", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = Item.CommonMaxStack;
            Item.useAnimation = 30;
            Item.UseSound = FargosSoundRegistry.TrojanSummon;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<TrojanSquirrel>());
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("Wood", 20)
                .AddRecipeGroup("FargowiltasSouls:AnySquirrel")
                .AddTile(TileID.WorkBenches)
                .DisableDecraft()
                .Register();
        }
    }
}