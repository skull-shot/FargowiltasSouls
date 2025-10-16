using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class FusedLens : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
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
            bool dubious = EffectItem(player).type != ModContent.ItemType<FusedLens>();
            bool sotm = false; //grants all buffs while under any debuff if sotm

            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int type = player.buffType[i];
                if (player.FargoSouls().MasochistSoul && type > 0 && Main.debuff[type] && FargowiltasSouls.DebuffIDs.Contains(type) && type != ModContent.BuffType<MutantPresenceBuff>())
                    sotm = true;

                if (sotm || (type > 0 && Main.debuff[type] && (FargowiltasSouls.DefenseReducingDebuffs.Contains(type) || (type == ModContent.BuffType<BerserkerInstallBuff>() && player.FargoSouls().BerserkedFromAgitation == true) || (type == 88 && EmodeItemBalance.HasEmodeChange(player, ItemID.RodofDiscord).Any(s => s == "RodofDiscord")))) || player.FargoSouls().TwinsInstall && type != ModContent.BuffType<MutantPresenceBuff>())
                {
                    if (!dubious) player.FargoSouls().FusedLensIchor = true;
                    player.GetCritChance(DamageClass.Generic) += 15;
                    break;
                }
            }

            if (sotm || player.lifeRegen < 0 || player.FargoSouls().TwinsInstall)
            {
                if (!dubious) player.FargoSouls().FusedLensCursed = true;
                player.FargoSouls().AttackSpeed += 0.15f;
            }
        }
    }
    public class FusedLensInstall : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DubiousHeader>();
        public override int ToggleItemType => ModContent.ItemType<FusedLens>();
        
    }
}