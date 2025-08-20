using System.Collections.Generic;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class FusedLens : SoulsItem
    {
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<DebuffInstallKeyEffect>()];

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;

            player.FargoSouls().FusedLens = true;
            player.AddEffect<FusedLensInstall>(Item);
            player.AddEffect<FusedLensStats>(Item);
            player.AddEffect<DebuffInstallKeyEffect>(Item);
        }
    }
    public class FusedLensStats : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override void PostUpdateEquips(Player player)
        {
            bool dubious = false;
            if (EffectItem(player).type != ModContent.ItemType<FusedLens>())
                dubious = true;

            if (player.lifeRegen < 0 && !dubious || player.FargoSouls().TwinsInstall)
            {
                player.FargoSouls().FusedLensCursed = true;
                player.FargoSouls().AttackSpeed += 0.15f;
            }
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int type = player.buffType[i];
                if (type > 0 && Main.debuff[type] && (FargowiltasSouls.DefenseReducingDebuffs.Contains(type) || type == ModContent.BuffType<BerserkerInstallBuff>() && player.FargoSouls().BerserkedFromAgitation == true || type == 88 && EmodeItemBalance.HasEmodeChange(player, ItemID.RodofDiscord)))
                {
                    if (!dubious || player.FargoSouls().TwinsInstall)
                    {
                        player.FargoSouls().FusedLensIchor = true;
                        player.GetCritChance(DamageClass.Generic) += 15;
                    }
                }
            }
        }
    }
    public class FusedLensInstall : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DubiousHeader>();
        public override int ToggleItemType => ModContent.ItemType<FusedLens>();
        
    }
}