﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ChlorophyteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(36, 137, 0);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<ChloroMinion>(item);
            player.FargoSouls().ChlorophyteEnchantActive = true;
            player.AddEffect<JungleJump>(item);
            player.AddEffect<JungleDashEffect>(item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyChloroHead")
                .AddIngredient(ItemID.ChlorophytePlateMail)
                .AddIngredient(ItemID.ChlorophyteGreaves)
                .AddIngredient(null, "JungleEnchant")
                .AddIngredient(ItemID.ChlorophyteClaymore)
                .AddIngredient(ItemID.AcornAxe) // Axe of Regrowth
                                                //grape juice
                                                //.AddIngredient(ItemID.Seedling);
                                                //plantero pet

                .AddTile<EnchantedTreeSheet>()
               .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Summon;
            tooltipColor = null;
            scaling = null;
            return (int)(ChloroMinion.BaseDamage(Main.LocalPlayer) * Main.LocalPlayer.ActualClassDamage(DamageClass.Summon));
        }
    }
    public class ChloroMinion : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<ChlorophyteEnchant>();
        public override bool MinionEffect => true;
        public static int BaseDamage(Player player)
        {
            int dmg = player.ForceEffect<ChloroMinion>() ? 50 : 30;
            if (player.HasEffect<NatureEffect>())
                dmg = 150;
            return dmg;
        }
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Chlorofuck>()] == 0)
            {
                const int max = 5;
                float rotation = 2f * (float)Math.PI / max;
                for (int i = 0; i < max; i++)
                {
                    Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                    FargoSoulsUtil.NewSummonProjectile(GetSource_EffectItem(player), spawnPos, Vector2.Zero, ModContent.ProjectileType<Chlorofuck>(), BaseDamage(player), 10f, player.whoAmI, Chlorofuck.Cooldown, rotation * i);
                }
            }
        }
    }
}
