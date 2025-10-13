﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class PureHeart : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 5));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.PureHeart = true;
            player.AddEffect<LightningImmunity>(Item);

            //rotting effect
            player.buffImmune[ModContent.BuffType<RottingBuff>()] = true;
            player.moveSpeed += 0.1f;
            fargoPlayer.RottingHeartItem = Item;
            player.AddEffect<RottingHeartEaters>(Item);
            if (fargoPlayer.RottingHeartCD > 0)
                fargoPlayer.RottingHeartCD--;

            //gutted effect
            player.buffImmune[ModContent.BuffType<BloodthirstyBuff>()] = true;
            player.statLifeMax2 += player.statLifeMax / 10;
            player.AddEffect<GuttedHeartEffect>(Item);
            player.AddEffect<GuttedHeartMinions>(Item);

            //gelic effect
            player.FargoSouls().GelicWingsItem = Item;
            //player.AddEffect<GelicWingJump>(Item);
            player.AddEffect<GelicWingSpikes>(Item);
            player.FargoSouls().WingTimeModifier += .3f;

            //pungent effect
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.AddEffect<PungentEyeballCursor>(Item);
            player.FargoSouls().PungentEyeball = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<RottingHeart>())
            .AddIngredient(ModContent.ItemType<GuttedHeart>())
            .AddIngredient(ModContent.ItemType<GelicWings>())
            .AddIngredient(ModContent.ItemType<PungentEyeball>())
            .AddIngredient(ItemID.ChlorophyteBar, 12)
            .AddIngredient(ItemID.GreenSolution, 50)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)

            .Register();
        }
    }
}