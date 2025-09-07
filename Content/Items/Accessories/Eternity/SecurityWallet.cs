using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class SecurityWallet : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
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

        public static void PassiveEffects(Player player, Item item)
        {
            player.buffImmune[Terraria.ModLoader.ModContent.BuffType<MidasBuff>()] = true;
            player.FargoSouls().SecurityWallet = true;
            player.AddEffect<GoldToPiggy>(item);
        }

        public override void UpdateInventory(Player player)
        {
            PassiveEffects(player, Item);
        }

        public override void UpdateVanity(Player player)
        {
            PassiveEffects(player, Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PassiveEffects(player, Item);
        }
    }

    public class GoldToPiggy : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LithosphericHeader>();
        public override int ToggleItemType => ModContent.ItemType<SecurityWallet>();

        public override void PostUpdateEquips(Player player)
        {
            for (int i = 50; i <= 53; i++) //detect coins in coin slots
            {
                if (!player.inventory[i].IsAir && player.inventory[i].IsACoin)
                    player.FargoSouls().GoldEnchMoveCoins = true;
            }
        }
    }
}