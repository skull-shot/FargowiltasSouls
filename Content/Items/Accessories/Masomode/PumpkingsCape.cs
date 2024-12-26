using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Front, EquipType.Back)]
    public class PumpkingsCape : SoulsItem
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
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.AddEffect<PumpkingsCapeEffect>(Item);
            player.AddEffect<ParryEffect>(Item);
            player.buffImmune[ModContent.BuffType<LivingWastelandBuff>()] = true;
        }
    }
    public class PumpkingsCapeEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<HeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<PumpkingsCape>();
    }
}