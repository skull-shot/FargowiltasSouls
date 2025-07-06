using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Consumables
{
    public class OrdinaryCarrot : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.UseSound = SoundID.Item2;
            Item.value = Item.sellPrice(0, 0, 10, 0);
        }
        public override bool CanUseItem(Player player)
        {
            if (player.FargoSouls().OrdinaryCarrot)
            {
                return false;
            }
            else return base.CanUseItem(player);
        }
        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.AddBuff(BuffID.NightOwl, 10800);
                player.AddBuff(BuffID.WellFed3, 10800);
                player.FargoSouls().OrdinaryCarrot = true;
            }
            return true;
        }
    }
    public class MasoCarrotEffect : AccessoryEffect
    {
        public override bool DefaultToggle => false;
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<OrdinaryCarrot>();
    }
}