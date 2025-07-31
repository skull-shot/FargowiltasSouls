using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class NecromanticBrew : SoulsItem
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
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<LethargicBuff>()] = true;
            player.FargoSouls().NecromanticBrewItem = Item;
            player.AddEffect<NecroBrewSpin>(Item);
            player.AddEffect<SkeleMinionEffect>(Item);

        }
    }
    public class NecroBrewSpin : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<NecromanticBrew>();
    }
    public class SkeleMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<NecromanticBrew>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff<SouloftheMasochistBuff>())
                player.AddBuff(ModContent.BuffType<SkeletronArmsBuff>(), 2);
        }
    }
}