﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class FrostEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(122, 189, 185);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FrostEffect>(Item);
            player.AddEffect<SnowEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FrostHelmet)
                .AddIngredient(ItemID.FrostBreastplate)
                .AddIngredient(ItemID.FrostLeggings)
                .AddIngredient(ModContent.ItemType<SnowEnchant>())
                .AddIngredient(ItemID.Frostbrand)
                .AddIngredient(ItemID.IceBow)
                //frost staff
                //coolwhip
                //.AddIngredient(ItemID.BlizzardStaff);
                //.AddIngredient(ItemID.ToySled);
                //.AddIngredient(ItemID.BabyGrinchMischiefWhistle);

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Magic;
            tooltipColor = null;
            scaling = null;
            return (int)((Main.LocalPlayer.FargoSouls().ForceEffect<FrostEnchant>() ? 100 : 50) * Main.LocalPlayer.ActualClassDamage(DamageClass.Magic));
        }
    }
    public class FrostEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<FrostEnchant>();
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (player.HasEffect<NatureEffect>())
            {
                target.AddBuff(BuffID.Frostburn, 60);
                target.AddBuff(BuffID.Frostburn2, 60);
            }
        }
    }
}
