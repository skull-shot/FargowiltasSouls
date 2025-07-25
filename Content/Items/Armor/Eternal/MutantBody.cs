using FargowiltasSouls.Content.Rarities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Eternal
{
    [AutoloadEquip(EquipType.Body)]
    public class MutantBody : SoulsItem
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
            Item.value = Item.sellPrice(0, 70);
            Item.defense = 70;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.70f;
            player.GetCritChance(DamageClass.Generic) += 30;

            player.statLifeMax2 += 200;
            player.statManaMax2 += 200;

            player.endurance += 0.3f;

            player.lifeRegen += 7;
            player.lifeRegenCount += 7;
            player.lifeRegenTime += 7;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "MutantBody"))
            .AddIngredient(null, "AbomEnergy", 15)
            .AddIngredient(null, "EternalEnergy", 15)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}