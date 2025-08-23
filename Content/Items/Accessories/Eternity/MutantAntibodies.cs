using FargowiltasSouls.Content.Buffs.Eternity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class MutantAntibodies : SoulsItem
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
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Wet] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.buffImmune[ModContent.BuffType<OceanicMaulBuff>()] = true;
            player.FargoSouls().MutantAntibodies = true;

            DamageClass damageClass = player.ProcessDamageTypeFromHeldItem();
            player.GetDamage(damageClass) += 0.2f;

            player.rabid = true;
            if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
                player.dripping = true;
        }
    }
}