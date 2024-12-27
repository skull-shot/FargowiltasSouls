using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class LihzahrdTreasureBox : SoulsItem
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
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[ModContent.BuffType<FusedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LihzahrdCurseBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;
            player.FargoSouls().LihzahrdTreasureBoxItem = Item;
            player.AddEffect<LihzahrdGroundPound>(Item);
            player.AddEffect<DiveEffect>(Item);
            player.AddEffect<LihzahrdBoulders>(Item);
        }
    }
    public class LihzahrdGroundPound : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<LihzahrdTreasureBox>();
    }
    public class LihzahrdBoulders : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<LihzahrdTreasureBox>();
        public override bool ExtraAttackEffect => true;
    }
}