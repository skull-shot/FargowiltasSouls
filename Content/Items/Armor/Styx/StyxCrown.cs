﻿using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Achievements;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Armor;
using FargowiltasSouls.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Styx
{
    [AutoloadEquip(EquipType.Head)]
    public class StyxCrown : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Armor/Styx", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ModContent.RarityType<AbominableRarity>();
            Item.value = Item.sellPrice(0, 20);
            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 10;

            player.maxMinions += 3;
            player.maxTurrets += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StyxChestplate>() && legs.type == ModContent.ItemType<StyxLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            //player.armorEffectDrawOutlinesForbidden = true;
        }

        public const int MINIMUM_CHARGE_TIME = 40 * 60;
        public const int MAX_SCYTHES = 12;
        public const int METER_THRESHOLD = 1500000 / MAX_SCYTHES; //based off mutant hp 7.7M
        public const int MINIMUM_DPS = METER_THRESHOLD * MAX_SCYTHES / MINIMUM_CHARGE_TIME * 60;

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = getSetBonusString();
            StyxSetBonus(player, Item);
        }

        public static string getSetBonusString()
        {
            string key = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
            return Language.GetTextValue($"Mods.FargowiltasSouls.SetBonus.Styx", key);
        }
        public static void StyxSetBonusKey(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.StyxSet && player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<StyxGazerArmor>()] <= 0)
            {
                int scytheType = ModContent.ProjectileType<StyxArmorScythe>();
                bool superAttack = modPlayer.StyxAttackReadyTimer > 0;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].friendly && Main.projectile[i].type == scytheType && Main.projectile[i].owner == player.whoAmI)
                    {
                        if (!superAttack)
                        {
                            Projectile.NewProjectile(Main.projectile[i].GetSource_FromThis(), Main.projectile[i].Center, Vector2.Normalize(Main.projectile[i].velocity) * 24f, ModContent.ProjectileType<StyxArmorScythe2>(),
                                Main.projectile[i].damage, Main.projectile[i].knockBack, player.whoAmI, -1, -1);
                        }

                        Main.projectile[i].Kill();
                    }
                }

                if (superAttack)
                {
                    Vector2 speed = Vector2.Normalize(Main.MouseWorld - player.Center);
                    bool flip = speed.X < 0;
                    speed = speed.RotatedBy(MathHelper.PiOver2 * (flip ? 1 : -1));
                    Projectile.NewProjectile(player.GetSource_Misc(""), player.Center, speed, ModContent.ProjectileType<StyxGazerArmor>(), 0, 14f, player.whoAmI, MathHelper.Pi / 120 * (flip ? -1 : 1));

                    player.controlUseItem = false; //this kills other heldprojs
                    player.releaseUseItem = true;
                    modPlayer.StyxAttackReadyTimer = 0;

                    if (Main.myPlayer == player.whoAmI)
                    {
                        ModContent.GetInstance<SuperAttackAchievement>().Condition.Complete();
                    }
                }
            }
        }
        public static void StyxSetBonus(Player player, Item item)
        {
            player.GetDamage(player.ProcessDamageTypeFromHeldItem()) += 0.20f;

            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.StyxSet = true;

            int scytheType = ModContent.ProjectileType<StyxArmorScythe>();
            if (fargoPlayer.StyxMeter > METER_THRESHOLD)
            {
                fargoPlayer.StyxMeter -= METER_THRESHOLD;
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[scytheType] < MAX_SCYTHES)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, scytheType, 0, 10f, player.whoAmI, player.ownedProjectileCounts[scytheType], -1f);
                    if (++player.ownedProjectileCounts[scytheType] >= MAX_SCYTHES)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(FargosSoundRegistry.StyxCharge, player.Center);
                    }
                }
            }

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[scytheType] >= MAX_SCYTHES)
            {
                fargoPlayer.StyxMeter = 0;
                fargoPlayer.StyxTimer = 0;
                fargoPlayer.StyxAttackReadyTimer = FargoSoulsPlayer.SuperAttackMaxWindow;
            }
        }
        public enum ChargeContext
        {
            DealDamage,
            DPS,
            Graze
        }
        public static float StyxChargeMultiplier(Player player, ChargeContext context)
        {
            return 1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SoulofSight, 15)
            .AddIngredient(ItemID.LunarBar, 5)
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}
