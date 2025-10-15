﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Fargowiltas.FargoSets;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    [LegacyName("FrigidGemstone")]
    public class FrigidGrasp : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<FrigidGraspKeyEffect>()];

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 4);
            Item.defense = 3;
        }

        /*public static void PassiveEffects(Player player, Item item)
        {
        }*/
        public static void ActiveEffects(Player player, Item item)
        {
            player.AddEffect<FrigidGraspKeyEffect>(item);
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Chilled] = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Magic;
            tooltipColor = null;
            scaling = null;
            return FrigidGraspKeyEffect.BaseDamage(Main.LocalPlayer);
        }

        //public override void UpdateInventory(Player player) => PassiveEffects(player, Item);
        /*
        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            //player.ReplaceItem(Item, ModContent.ItemType<FrigidGemstoneInactive>());
            return false;
        }
        */

        //public override void UpdateVanity(Player player) => PassiveEffects(player, Item);
        //public override bool CanRightClick() => true;
        //public override void RightClick(Player player)
        //{
            //player.ReplaceItem(Item, ModContent.ItemType<FrigidGemstoneInactive>());
        //}
    }
    /*
    public class FrigidGemstoneInactive : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4); 
        }

        void Effects(Player player)
        {
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.AddEffect<FrigidGraspKeyEffect>(Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => Effects(player);

        //public override void UpdateInventory(Player player) => Effects(player);
        public override bool AltFunctionUse(Player player) 
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<FrigidGemstone>());
            return false;
        }

        public override void UpdateVanity(Player player) => Effects(player);
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<FrigidGemstone>());
        }
    }
    */
    public class FrigidGraspKeyEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override int ToggleItemType => ModContent.ItemType<FrigidGrasp>();
        public static int BaseDamage(Player player) => (int)(14 * player.ActualClassDamage(DamageClass.Magic));
        public override void ActiveSkillHeld(Player player, bool stunned)
        {
            if (stunned)
                return;
            player.FargoSouls().FrigidGraspKey();
        }
    }
}