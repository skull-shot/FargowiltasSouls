﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Eridanus
{
    [AutoloadEquip(EquipType.Head)]
    public class EridanusHat : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Armor/Eridanus", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 14);
            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 5;

            player.maxMinions += 1;
            player.maxTurrets += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<EridanusBattleplate>() && legs.type == ModContent.ItemType<EridanusLegwear>();
        }

        public override void ArmorSetShadows(Player player)
        {
            //player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = getSetBonusString(player);
            EridanusSetBonus(player, Item);
        }

        public static string getSetBonusString(Player player)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            string key = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
            return Language.GetTextValue($"Mods.FargowiltasSouls.SetBonus.Eridanus{(fargoPlayer.EridanusEmpower ? "On" : "Off")}", key);
        }
        public static void EridanusSetBonusKey(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.EridanusSet)
            {
                modPlayer.EridanusEmpower = !modPlayer.EridanusEmpower;
            }
        }
        public const int ClassDuration = 60 * 12 + 30;
        public static void EridanusSetBonus(Player player, Item item)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.EridanusSet = true;
            player.statManaMax2 += 30;
            player.maxMinions += 3;

            //if (player.whoAmI == Main.myPlayer && fargoPlayer.DoubleTap)
            //fargoPlayer.EridanusEmpower = !fargoPlayer.EridanusEmpower;

            if (fargoPlayer.EridanusEmpower)
            {
                if (fargoPlayer.EridanusTimer % ClassDuration == 1) //make dust whenever changing classes
                {
                    SoundEngine.PlaySound(SoundID.Item4, player.Center);

                    int type;
                    switch (fargoPlayer.EridanusTimer / ClassDuration)
                    {
                        case 0: type = 127; break; //solar
                        case 1: type = 229; break; //vortex
                        case 2: type = 242; break; //nebula
                        default: type = 135; break; //stardust
                    }

                    const int max = 100; //make some indicator dusts
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 vector6 = Vector2.UnitY * 20f;
                        vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, type, 0f, 0f, 0, default, 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = vector7;
                    }

                    for (int i = 0; i < 50; i++) //make some indicator dusts
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, type, 0f, 0f, 0, default, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 24f;
                    }

                    //if (Main.myPlayer == player.whoAmI)
                    //{
                    //    for (int i = 0; i < Main.maxProjectiles; i++) //clear minions
                    //    {
                    //        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI
                    //            && Main.projectile[i].type != ModContent.ProjectileType<Projectiles.Minions.EridanusMinion>()
                    //            && Main.projectile[i].minionSlots > 0)
                    //        {
                    //            Main.projectile[i].Kill();
                    //        }
                    //    }
                    //}
                }

                if (++fargoPlayer.EridanusTimer > ClassDuration * 4) //handle loop
                {
                    fargoPlayer.EridanusTimer = 0;
                }

                void Bonuses(DamageClass damageClass)
                {
                    player.GetDamage(damageClass) += 0.70f; // 70%

                    //if (damageClass == DamageClass.Summon)
                        //fargoPlayer.MinionCrits = true;

                    player.GetCritChance(damageClass) += 30;

                    if (player.HeldItem.CountsAsClass(damageClass))
                        fargoPlayer.AttackSpeed += .3f;
                }

                switch (fargoPlayer.EridanusTimer / ClassDuration) //damage boost according to current class
                {
                    case 0: Bonuses(DamageClass.Melee); break;
                    case 1: Bonuses(DamageClass.Ranged); break;
                    case 2: Bonuses(DamageClass.Magic); break;
                    default: Bonuses(DamageClass.Summon); break;
                }

                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<EridanusMinion>()] < 1)
                    {
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, ModContent.ProjectileType<EridanusMinion>(), 300, 12f, player.whoAmI, -1);
                    }
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<EridanusRitual>()] < 1)
                    {
                        Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, ModContent.ProjectileType<EridanusRitual>(), 0, 0f, player.whoAmI);
                    }
                }
            }
            else //eridanus off, give weaker boosts
            {
                DamageClass damageClass = player.ProcessDamageTypeFromHeldItem();

                player.GetDamage(damageClass) += 0.15f;
                player.GetCritChance(damageClass) += 10;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Eridanium>(), 5)
            .AddIngredient(ItemID.FragmentSolar, 5)
            .AddIngredient(ItemID.FragmentVortex, 5)
            .AddIngredient(ItemID.FragmentNebula, 5)
            .AddIngredient(ItemID.FragmentStardust, 5)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}