﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Projectiles.Weapons.FinalUpgrades;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.FinalUpgrades
{
    public class PhantasmalLeashOfCthulhu : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/FinalUpgrades", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 2800; //eoc base life funny
            Item.width = 30;
            Item.height = 10;
            Item.value = Item.sellPrice(1);
            Item.rare = ItemRarityID.Purple;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.knockBack = 6f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<PhantasmalFlail>();
            Item.shootSpeed = 45f;
            Item.UseSound = null;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(0, Main.DiscoG, 255);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<LeashofCthulhu>(), 1)
            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 15)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}