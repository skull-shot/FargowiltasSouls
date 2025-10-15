﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    [LegacyName("MechanicalLeashOfCthulhu")]
    public class LeashofCthulhu : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/SwarmDrops", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.damage = 250;
            Item.width = 30;
            Item.height = 10;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.knockBack = 6f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<LeashofCthulhuFlail>();
            Item.shootSpeed = 50f;
            Item.UseSound = null;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Eyeleash>())
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerEye"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}