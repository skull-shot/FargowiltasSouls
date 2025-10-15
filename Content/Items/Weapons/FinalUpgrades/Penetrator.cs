﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Weapons.FinalUpgrades;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls.Content.Items.Weapons.FinalUpgrades
{
    [LegacyName("HentaiSword")]
    public class Penetrator : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/FinalUpgrades", Name);

        public override int NumFrames => 10;
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 10));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;

            PrefixLegacy.ItemSets.ItemsThatCanHaveLegendary2[Type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 1700;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.shootSpeed = 6f;
            Item.knockBack = 7f;
            Item.width = 72;
            Item.height = 72;
            //Item.scale = 1.3f;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Content.Projectiles.Weapons.FinalUpgrades.Penetrator>();
            Item.value = Item.sellPrice(0, 70);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        int forceSwordTimer;

        public override bool CanUseItem(Player player)
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;

            if (forceSwordTimer > 0)
            {
                Item.shoot = ModContent.ProjectileType<PenetratorSword>();
                Item.shootSpeed = 6f;

                Item.useAnimation = 16;
                Item.useTime = 16;

                Item.useStyle = ItemUseStyleID.Swing;
                Item.DamageType = DamageClass.Melee;
            }
            else if (player.altFunctionUse == 2)
            {
                if (player.controlUp && player.controlDown)
                {
                    Item.shoot = ModContent.ProjectileType<PenetratorWand>();
                    Item.shootSpeed = 6f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                }
                else if (player.controlUp && !player.controlDown)
                {
                    /*Item.shoot = ModContent.ProjectileType<PenetratorSpinThrown>();
                    Item.shootSpeed = 6f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;*/

                    Item.shoot = ModContent.ProjectileType<PenetratorSpinBoundary>();
                    Item.shootSpeed = 1f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                    Item.useTurn = true;
                }
                else if (player.controlDown && !player.controlUp)
                {
                    Item.shoot = ModContent.ProjectileType<PenetratorSpinBoundary>();
                    Item.shootSpeed = 1f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                    Item.useTurn = true;
                }
                else
                {
                    Item.shoot = ModContent.ProjectileType<PenetratorThrown>();
                    Item.shootSpeed = 25f;
                    Item.useAnimation = 85;
                    Item.useTime = 85;
                }

                Item.DamageType = DamageClass.Ranged;
            }
            else
            {
                if (player.controlUp && !player.controlDown)
                {
                    Item.shoot = ModContent.ProjectileType<PenetratorSpin>();
                    Item.shootSpeed = 1f;
                    Item.useTurn = true;
                }
                else if (player.controlDown && !player.controlUp)
                {
                    Item.shoot = ModContent.ProjectileType<PenetratorDive>();
                    Item.shootSpeed = 6f;
                }
                else if (player.controlDown && player.controlUp)
                {
                    Item.shoot = ModContent.ProjectileType<Content.Projectiles.Weapons.FinalUpgrades.Penetrator>();
                    Item.shootSpeed = 6f;
                }
                else
                {
                    Item.shoot = ModContent.ProjectileType<PenetratorSword>();
                    Item.shootSpeed = 6f;
                    Item.useStyle = ItemUseStyleID.Swing;
                }

                Item.useAnimation = 16;
                Item.useTime = 16;

                Item.DamageType = DamageClass.Melee;
            }

            return true;
        }

        public override void UpdateInventory(Player player)
        {
            if (forceSwordTimer > 0)
                forceSwordTimer -= 1;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<PenetratorSword>()] > 0)
                forceSwordTimer = 3;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (forceSwordTimer > 0 || (player.altFunctionUse != 2 && !player.controlUp && !player.controlDown))
            {
                velocity = new Vector2(velocity.X < 0 ? 1 : -1, -1);
                velocity.Normalize();
                velocity *= PenetratorSword.MUTANT_SWORD_SPACING;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, -Math.Sign(velocity.X));
                return false;
            }
            else if (player.altFunctionUse == 2) // Right-click
            {
                if (player.controlUp)
                {
                    if (player.controlDown) // Giga-beam
                        return player.ownedProjectileCounts[Item.shoot] < 1;

                    /*if (player.ownedProjectileCounts[Item.shoot] < 1) // Remember to transfer any changes here to Penetratorspinthrown!
                    {
                        Vector2 speed = Main.MouseWorld - player.MountedCenter;

                        if (speed.Length() < 360)
                            speed = Vector2.Normalize(speed) * 360;

                        Projectile.NewProjectile(source, position, Vector2.Normalize(speed), Item.shoot, damage, knockback, player.whoAmI, speed.X, speed.Y);
                    }*/

                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: 1);

                    return false;
                }

                return true;
            }

            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                if (player.controlUp && !player.controlDown)
                    return true;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<Dash>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<Dash2>()] < 1)
                {
                    float dashAI = 0;
                    float speedModifier = 2f;
                    int dashType = ModContent.ProjectileType<Dash>();

                    if (player.controlUp && player.controlDown) // Super-dash
                    {
                        dashAI = 1;
                        speedModifier = 2.5f;
                    }

                    Vector2 speed = velocity;

                    if (player.controlDown && !player.controlUp) //dive
                    {
                        dashAI = 2;
                        speed = new Vector2(Math.Sign(velocity.X) * 0.0001f, speed.Length());
                        dashType = ModContent.ProjectileType<Dash2>();
                    }

                    int p = Projectile.NewProjectile(source, position, Vector2.Normalize(speed) * speedModifier * Item.shootSpeed,
                        dashType, damage, knockback, player.whoAmI, speed.ToRotation(), dashAI);
                    if (p != Main.maxProjectiles)
                        Projectile.NewProjectile(source, position, speed, Item.shoot, damage, knockback, player.whoAmI, Main.projectile[p].identity, 1f);
                }
            }

            return false;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.Text");
                shader.TrySetParameter("mainColor", new Color(28, 222, 152));
                shader.TrySetParameter("secondaryColor", new Color(168, 245, 228));
                shader.Apply("PulseUpwards");
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerMoon"))
                .AddIngredient(ModContent.ItemType<EternalEnergy>(), 30)
                .AddIngredient(ModContent.ItemType<AbomEnergy>(), 30)
                .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 30)
                .AddIngredient(ModContent.ItemType<BrokenSpearhead>())
                .AddIngredient(ModContent.ItemType<MutantEye>())
                .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
                .Register();
        }
    }
}