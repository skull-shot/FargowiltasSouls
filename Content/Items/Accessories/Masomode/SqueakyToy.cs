using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class SqueakyToy : SoulsItem
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
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.SqueakyToyBuff>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.GuiltyBuff>()] = true;
            player.AddEffect<SqueakEffect>(Item);
        }
        private bool lastLMouse = false;
        public override void HoldItem(Player player) //doing this instead of making an item use animation lo
        {
            if (!lastLMouse && Main.mouseLeft)
            {
                FargoSoulsPlayer.Squeak(player.Center, 0.25f);
            }
            lastLMouse = Main.mouseLeft;
        }
    }
    public class SqueakEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<BionomicHeader>();
        public override int ToggleItemType => ModContent.ItemType<SqueakyToy>();
        public override bool MutantsPresenceAffects => true;
    }
}