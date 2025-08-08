using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    [AutoloadEquip(EquipType.Neck)]
    public class BetsysHeart : SoulsItem
    {
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<BetsyDashEffect>()];

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
            Item.value = Item.sellPrice(gold: 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.OgreSpit] = true;
            player.buffImmune[BuffID.WitheredWeapon] = true;
            player.buffImmune[BuffID.WitheredArmor] = true;
            player.FargoSouls().BetsysHeartItem = Item;
            player.AddEffect<BetsyDashEffect>(Item);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Melee;
            tooltipColor = null;
            scaling = null;
            return BetsyDashEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class BetsyDashEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override int ToggleItemType => ModContent.ItemType<BetsysHeart>();
        public static int BaseDamage(Player player) => (int)(250 * player.ActualClassDamage(DamageClass.Melee));
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (stunned)
                return;
            player.FargoSouls().SpecialDashKey(2);
        }
    }
}
