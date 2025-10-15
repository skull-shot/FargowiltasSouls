﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class RainEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(255, 236, 0);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
            if (!hideVisual && player.HasEffect<RainInnerTubeEffect>())
                player.hasFloatingTube = true;
        }
        public static void AddEffects(Player player, Item item)
        {
            player.buffImmune[BuffID.Wet] = true;
            player.AddEffect<RainUmbrellaEffect>(item);
            player.AddEffect<RainInnerTubeEffect>(item);
            player.AddEffect<RainWetEffect>(item);
            player.AddEffect<RainFeatherfallEffect>(item);
            player.AddEffect<LightningImmunity>(item);
        }
        public override void UpdateVanity(Player player)
        {
            player.AddEffect<LightningImmunity>(Item);
            player.hasFloatingTube = true;
        }
        public override void UpdateInventory(Player player)
        {
            player.AddEffect<LightningImmunity>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()

                .AddIngredient(ItemID.RainHat)
                .AddIngredient(ItemID.RainCoat)
                .AddIngredient(ItemID.UmbrellaHat)
                .AddIngredient(ItemID.FloatingTube) //inner tube
                .AddIngredient(ItemID.Umbrella)
                .AddIngredient(ItemID.WaterGun)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }
    public class RainUmbrellaEffect : AccessoryEffect
    {
        public override int ToggleItemType => ModContent.ItemType<RainEnchant>();
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff(ModContent.BuffType<RainCDBuff>()))
            {
                player.FargoSouls().AddMinion(EffectItem(player), true, ModContent.ProjectileType<RainUmbrella>(), 0, 0);
                if (player.HasEffect<RainFeatherfallEffect>())
                {
                    player.slowFall = true;
                }
            }
        }
    }
    public class RainFeatherfallEffect : AccessoryEffect
    {
        public override int ToggleItemType => ModContent.ItemType<RainEnchant>();
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
    }

    public class RainWetEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            target.AddBuff(BuffID.Wet, 180);
        }
    }
    public class RainInnerTubeEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<RainEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            player.canFloatInWater = true;
        }
    }
    public class LightningImmunity : AccessoryEffect
    {
        public override Header ToggleHeader => null;
    }
}
