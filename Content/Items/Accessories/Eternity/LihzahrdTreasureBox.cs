using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class LihzahrdTreasureBox : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<DiveEffect>()];

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
            player.noKnockback = true;
            player.buffImmune[ModContent.BuffType<DaybrokenBuff>()] = true;
            player.buffImmune[ModContent.BuffType<FusedBuff>()] = true;
            //player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;
            player.FargoSouls().LihzahrdTreasureBoxItem = Item;
            player.AddEffect<LihzahrdGroundPound>(Item);
            player.AddEffect<DiveEffect>(Item);
            player.AddEffect<LihzahrdBoulders>(Item);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Melee;
            tooltipColor = null;
            scaling = null;
            return LihzahrdGroundPound.BaseDamage(Main.LocalPlayer) * 3; //display highest damaging projectile
        }
    }
    public class DiveEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<LihzahrdTreasureBox>();
        public override bool ActiveSkill => true;
    }
    public class LihzahrdGroundPound : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<LihzahrdTreasureBox>();
        public static int BaseDamage(Player player) => (int)(150 * player.ActualClassDamage(DamageClass.Melee));
    }
    public class LihzahrdBoulders : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<VerdantHeader>();
        public override int ToggleItemType => ModContent.ItemType<LihzahrdTreasureBox>();
        public override bool ExtraAttackEffect => true;
    }
}