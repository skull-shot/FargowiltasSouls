using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Souls;
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
    public class MoltenShield : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

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
        }
        public static void ActiveEffects(Player player, Item item)
        {
            player.AddEffect<FleshKnuckleEffect>(item);
            player.AddEffect<FrozenTurtleEffect>(item);
            player.AddEffect<PaladinShieldEffect>(item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BeetleHusk, 5)
                .AddIngredient(ItemID.HeroShield)
                .AddIngredient(ItemID.FrozenTurtleShell)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.BeetleHusk, 5)
                .AddIngredient(ItemID.FrozenShield)
                .AddIngredient(ItemID.FleshKnuckles)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
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
    public class FrozenTurtleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ItemID.FrozenTurtleShell;

        public override void PostUpdateEquips(Player player)
        {
            if (player.statLife <= player.statLifeMax2 * 0.5)
                player.AddBuff(BuffID.IceBarrier, 5, true);
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