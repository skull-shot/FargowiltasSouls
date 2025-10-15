﻿
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Weapons.FinalUpgrades;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.FinalUpgrades
{
    public class SparklingLove : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/FinalUpgrades", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 1700;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 27;
            Item.useTime = 27;
            Item.shootSpeed = 16f;
            Item.knockBack = 14f;
            Item.width = 122;
            Item.height = 118;
            Item.scale = 2f;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = FargosSoundRegistry.DeviSwing;
            Item.shoot = ModContent.ProjectileType<Content.Projectiles.Weapons.FinalUpgrades.SparklingLove>();
            Item.value = Item.sellPrice(0, 70);
            Item.noMelee = true; //no melee hitbox
            Item.noUseGraphic = true; //dont draw Item
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
            
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<SparklingDevi>();
                Item.useStyle = ItemUseStyleID.Swing;
                Item.DamageType = DamageClass.Summon;
                Item.noUseGraphic = false;
                Item.noMelee = false;
                Item.useAnimation = 66;
                Item.useTime = 66;
                Item.mana = 100;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<Content.Projectiles.Weapons.FinalUpgrades.SparklingLove>();
                Item.useStyle = ItemUseStyleID.Swing;
                Item.DamageType = DamageClass.Melee;
                Item.noUseGraphic = true;
                Item.noMelee = true;
                Item.useAnimation = 27;
                Item.useTime = 27;
                Item.mana = 0;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                FargoSoulsUtil.NewSummonProjectile(source, position, velocity, type, Item.damage, knockback, player.whoAmI);
                return false;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.Text");
                shader.TrySetParameter("mainColor", new Color(255, 48, 154));
                shader.TrySetParameter("secondaryColor", new Color(255, 169, 240));
                shader.Apply("PulseCircle");
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), new Color(255, 169, 240), 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            //.AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerMoon"));
            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<BrokenBlade>())
            .AddIngredient(ModContent.ItemType<SparklingAdoration>())

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}