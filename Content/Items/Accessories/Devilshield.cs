using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Accessories.BionomicCluster;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories
{
    //[AutoloadEquip(EquipType.Shield)]
    public class Devilshield : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 12);
            Item.defense = 10;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
            player.noKnockback = true;
            if (player.statLife <= player.statLifeMax2 * 0.5 || player.statLife >= player.statLifeMax2)
            {
                player.endurance += 0.20f;
            }
        }
        public static void ActiveEffects(Player player, Item item)
        {
            player.AddEffect<FleshKnuckleEffect>(item);
            player.AddEffect<PaladinShieldEffect>(item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BeetleHusk, 5)
                .AddIngredient<Devilstone>(5)
                .AddIngredient(ItemID.HeroShield)
                .AddIngredient(ItemID.FrozenTurtleShell)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.BeetleHusk, 5)
                .AddIngredient<Devilstone>(5)
                .AddIngredient(ItemID.FrozenShield)
                .AddIngredient(ItemID.FleshKnuckles)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class PaladinShieldEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ItemID.PaladinsShield;

        public override void PostUpdateEquips(Player player)
        {
            if (player.statLife > player.statLifeMax2 * .25)
            {
                player.hasPaladinShield = true;
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    Player target = Main.player[k];

                    if (target.active && player != target && Vector2.Distance(target.Center, player.Center) < 400) target.AddBuff(BuffID.PaladinsShield, 30);
                }
            }
        }
    }
    public class FleshKnuckleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ItemID.FleshKnuckles;

        public override void PostUpdateEquips(Player player)
        {
            player.aggro += 400;
        }
    }
}