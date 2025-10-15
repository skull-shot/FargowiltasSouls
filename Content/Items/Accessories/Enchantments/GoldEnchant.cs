﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class GoldEnchant : BaseEnchant
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<GoldKeyEffect>()];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(231, 178, 28);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<GoldEffect>(Item);
            player.AddEffect<GoldKeyEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldHelmet)
                .AddIngredient(ItemID.GoldChainmail)
                .AddIngredient(ItemID.GoldGreaves)
                .AddIngredient(ItemID.RubyStaff)
                .AddIngredient(ItemID.GoldenDelight)
                .AddIngredient(ItemID.Goldfish)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }
    public class GoldEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<WillHeader>();
        public override int ToggleItemType => ModContent.ItemType<GoldEnchant>();
        
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            target.AddBuff(BuffID.Midas, 120, true);
        }
    }
    public class GoldKeyEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<GoldEnchant>();
        public override bool ActiveSkill => true;
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            player.FargoSouls().GoldKey(stunned);
        }
    }
}
