using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Styx
{
    [AutoloadEquip(EquipType.Body)]
    public class StyxChestplate : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Armor/Styx", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ModContent.RarityType<AbominableRarity>();
            Item.value = Item.sellPrice(0, 25);
            Item.defense = 35;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetCritChance(DamageClass.Generic) += 10;
            player.endurance += 0.04f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SoulofMight, 15)
            .AddIngredient(ItemID.LunarBar, 5)
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}