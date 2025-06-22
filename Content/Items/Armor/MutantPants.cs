using FargowiltasSouls.Content.Rarities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class MutantPants : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ModContent.RarityType<EternitySoulRarity>();
            Item.value = Item.sellPrice(0, 50);
            Item.defense = 50;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.50f;
            player.GetCritChance(DamageClass.Generic) += 20;

            player.moveSpeed += 0.4f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.4f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "MutantPants"))
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(null, "EternalEnergy", 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}