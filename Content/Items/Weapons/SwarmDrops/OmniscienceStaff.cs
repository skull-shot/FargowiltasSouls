﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    [LegacyName("OpticStaffEX")]   
    
    public class OmniscienceStaff : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/SwarmDrops", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 2;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GeminiGlaives>();
        }

        public override void SetDefaults()
        {
            Item.damage = 250;
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.width = 24;
            Item.height = 24;
            Item.useAnimation = 37;
            Item.useTime = 37;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item82;
            Item.value = Item.sellPrice(0, 25);
            Item.rare = ModContent.RarityType<AbominableRarity>();
            Item.buffType = ModContent.BuffType<OmniscienceBuff>();
            Item.shoot = ModContent.ProjectileType<OpticRetinazer>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            velocity = velocity.RotatedBy(Math.PI / 2);

            player.SpawnMinionOnCursor(source, player.whoAmI, ModContent.ProjectileType<OpticRetinazer>(), Item.damage, knockback, default, velocity);
            player.SpawnMinionOnCursor(source, player.whoAmI, ModContent.ProjectileType<OpticSpazmatism>(), Item.damage, knockback, default, -velocity);
            return false;
        }

        /*
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.OpticStaff)
            .AddIngredient(null, "TwinRangs")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerTwins"))
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
        */
    }
}