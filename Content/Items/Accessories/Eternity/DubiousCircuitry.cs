using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Eternity.Accessories.VerdantDoomsayerMask;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class DubiousCircuitry : SoulsItem
    {
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<DebuffInstallKeyEffect>(),
             AccessoryEffectLoader.GetEffect<RemoteLightningEffect>()];

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 5);
            Item.defense = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;
            player.buffImmune[BuffID.Electrified] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.DefenselessBuff>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.NanoInjectionBuff>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.LightningRodBuff>()] = true;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            modPlayer.FusedLens = true;
            modPlayer.DubiousCircuitry = true;
            player.AddEffect<FusedLensInstall>(Item);
            player.AddEffect<FusedLensStats>(Item);
            player.AddEffect<DebuffInstallKeyEffect>(Item);

            player.AddEffect<ProbeMinionEffect>(Item);
            player.AddEffect<GroundStickDR>(Item);
            player.AddEffect<RemoteLightningEffect>(Item);

            player.AddEffect<ReinforcedStats>(Item);
            player.endurance += 0.04f;
            player.noKnockback = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<FusedLens>())
            .AddIngredient(ModContent.ItemType<GroundStick>())
            .AddIngredient(ModContent.ItemType<ReinforcedPlating>())
            .AddIngredient(ItemID.HallowedBar, 10)
            .AddIngredient(ItemID.SoulofFright, 5)
            .AddIngredient(ItemID.SoulofMight, 5)
            .AddIngredient(ItemID.SoulofSight, 5)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)

            .Register();
        }
    }
}