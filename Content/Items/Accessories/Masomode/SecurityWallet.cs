using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class SecurityWallet : SoulsItem
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
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        public static void PassiveEffects(Player player)
        {
            player.buffImmune[Terraria.ModLoader.ModContent.BuffType<Buffs.Masomode.MidasBuff>()] = true;
            player.buffImmune[Terraria.ModLoader.ModContent.BuffType<Buffs.Masomode.LoosePocketsBuff>()] = true;
            player.FargoSouls().SecurityWallet = true;
        }

        public override void UpdateInventory(Player player)
        {
            PassiveEffects(player);
        }

        public override void UpdateVanity(Player player)
        {
            PassiveEffects(player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PassiveEffects(player);
        }
    }
}