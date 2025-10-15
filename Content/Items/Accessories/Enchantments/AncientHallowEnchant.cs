﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AncientHallowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(150, 133, 100);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 180000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static int BaseDamage(Player player) => player.FargoSouls().ForceEffect<AncientHallowEnchant>()? 350 : 200;
        public static void AddEffects(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            bool minion = player.AddEffect<AncientHallowMinion>(item);
            modPlayer.AddMinion(item, minion, ModContent.ProjectileType<HallowSword>(), BaseDamage(player), 2);
        }

        public static Color GetFairyQueenWeaponsColor(float alphaChannelMultiplier, float lerpToWhite, float rawHueOverride)
        {
            float num = rawHueOverride;

            float num2 = (num + 0.5f) % 1f;
            float saturation = 1f;
            float luminosity = 0.5f;

            Color color3 = Main.hslToRgb(num2, saturation, luminosity, byte.MaxValue);
            //color3 *= this.Opacity;
            if (lerpToWhite != 0f)
            {
                color3 = Color.Lerp(color3, Color.White, lerpToWhite);
            }
            color3.A = (byte)(color3.A * alphaChannelMultiplier);
            return color3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyAncientHallowHead")
                .AddIngredient(ItemID.AncientHallowedPlateMail)
                .AddIngredient(ItemID.AncientHallowedGreaves)
                .AddIngredient(ItemID.SwordWhip) //durendal
                .AddIngredient(ItemID.BouncingShield)
                .AddIngredient(ItemID.MagicMissile)
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Summon;
            tooltipColor = null;
            scaling = null;
            return (int)(BaseDamage(Main.LocalPlayer) * Main.LocalPlayer.ActualClassDamage(DamageClass.Summon));
        }
    }
    public class AncientHallowMinion : AccessoryEffect
    {
        public override int ToggleItemType => ModContent.ItemType<AncientHallowEnchant>();
        public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
        public override bool MinionEffect => true;
    }
}
