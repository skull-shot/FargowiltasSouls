using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Gaia
{
    [AutoloadEquip(EquipType.Body)]
    public class GaiaPlate : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.endurance += 0.1f;
            player.lifeRegen += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHusk, 6)
            .AddIngredient(ItemID.ShroomiteBar, 9)
            .AddIngredient(ItemID.SpectreBar, 9)
            .AddIngredient(ItemID.SpookyWood, 150)
            .AddTile(TileID.LunarCraftingStation)

            .Register();
        }
    }
}