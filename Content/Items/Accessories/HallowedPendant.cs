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
    //[AutoloadEquip(EquipType.Neck)]
    public class HallowedPendant : SoulsItem
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
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 7);
        }
        public static void ActiveEffects(Player player, Item item)
        {
            player.AddEffect<DefenseStarEffect>(item);
            player.AddEffect<DefenseBeeEffect>(item);
            player.longInvince = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddIngredient(ItemID.StarVeil)
                .AddIngredient(ItemID.HoneyComb)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddIngredient(ItemID.BeeCloak)
                .AddIngredient(ItemID.CrossNecklace)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class DefenseBeeEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ItemID.BeeCloak;
        public override void PostUpdateEquips(Player player)
        {
            player.honeyCombItem = EffectItem(player);
        }
    }
    public class DefenseStarEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ItemID.StarVeil;
        public override void PostUpdateEquips(Player player)
        {
            player.starCloakItem = EffectItem(player);
        }
    }
}