﻿using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Gaia
{
    [AutoloadEquip(EquipType.Head)]
    public class GaiaHelmet : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Armor/Gaia", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 5);
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            DamageClass damageClass = player.ProcessDamageTypeFromHeldItem();
            player.GetDamage(damageClass) += 0.1f;
            player.GetCritChance(DamageClass.Generic) += 6;
            player.statManaMax2 += 50;
            player.manaCost -= 0.1f;
            player.huntressAmmoCost90 = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GaiaPlate>() && legs.type == ModContent.ItemType<GaiaGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            if (fargoPlayer.GaiaOffense)
            {
                player.armorEffectDrawOutlinesForbidden = true;
                player.armorEffectDrawShadow = true;
            }
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = getSetBonusString();
            GaiaSetBonus(player);
        }

        public static string getSetBonusString()
        {
            string key = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
            return Language.GetTextValue($"Mods.FargowiltasSouls.SetBonus.Gaia", key);
        }
        public static void GaiaSetBonusKey(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.GaiaSet)
            {
                modPlayer.GaiaOffense = !modPlayer.GaiaOffense;

                if (modPlayer.GaiaOffense)
                    SoundEngine.PlaySound(SoundID.Item4, player.Center);

                Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                const int max = 36; //make some indicator dusts
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = baseVel * 6f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + player.Center;
                    Vector2 vector7 = vector6 - player.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.NextBool() ? 107 : 110, 0f, 0f, 0, default);
                    Main.dust[d].scale = 2.5f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }
            }
        }
        public static void GaiaSetBonus(Player player)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.GaiaSet = true;

            if (fargoPlayer.GaiaOffense)
            {
                DamageClass damageClass = player.ProcessDamageTypeFromHeldItem();
                player.GetDamage(damageClass) += 0.30f;
                player.GetCritChance(damageClass) += 15;
                player.GetArmorPenetration(DamageClass.Generic) += 20;
                player.statDefense -= 20;
                player.statLifeMax2 -= player.statLifeMax / 10;
                player.endurance -= 0.15f;
                Lighting.AddLight(player.Center, new Vector3(1, 1, 1));
                if (Main.rand.NextBool(3)) //visual dust
                {
                    int type = Main.rand.NextBool() ? 107 : 110;
                    int dust = Dust.NewDust(player.position, player.width, player.height, type, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 87, default, 1);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y -= 1f;
                    Main.dust[dust].velocity *= 1.8f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHusk, 3)
            .AddIngredient(ItemID.ShroomiteBar, 6)
            .AddIngredient(ItemID.SpectreBar, 6)
            .AddIngredient(ItemID.SpookyWood, 100)
            .AddTile(TileID.LunarCraftingStation)

            .Register();
        }
    }
}
