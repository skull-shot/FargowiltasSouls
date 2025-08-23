using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Gaia
{
    [AutoloadEquip(EquipType.Legs)]
    public class GaiaGreaves : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Armor/Gaia", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 5);
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            DamageClass damageClass = player.ProcessDamageTypeFromHeldItem();
            player.GetDamage(damageClass) += 0.08f;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHusk, 3)
            .AddIngredient(ItemID.ShroomiteBar, 6)
            .AddIngredient(ItemID.SpectreBar, 6)
            .AddIngredient(ItemID.SpookyWood, 100)
            .AddTile(TileID.LunarCraftingStation)

            .Register();
        }
    }
}
