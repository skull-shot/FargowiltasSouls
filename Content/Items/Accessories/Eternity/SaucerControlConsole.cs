using System.Collections.Generic;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class SaucerControlConsole : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips => 
            [AccessoryEffectLoader.GetEffect<AmmoCycleEffect>()];

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
        }

        public override void UpdateInventory(Player player)
        {
            player.AddEffect<AmmoCycleEffect>(Item);
        }

        public override void UpdateVanity(Player player)
        {
            player.AddEffect<AmmoCycleEffect>(Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AmmoCycleEffect>(Item);
            player.buffImmune[BuffID.VortexDebuff] = true;
            player.buffImmune[ModContent.BuffType<UnstableBuff>()] = true;
            player.AddEffect<UfoMinionEffect>(Item);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Summon;
            tooltipColor = null;
            scaling = null;
            return (int)(UfoMinionEffect.BaseDamage(Main.LocalPlayer) * Main.LocalPlayer.ActualClassDamage(DamageClass.Summon));
        }
    }
    public class UfoMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<HeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<SaucerControlConsole>();
        public override bool MinionEffect => true;
        public static int BaseDamage(Player player) => NPC.downedGolemBoss ? 30 : 15;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff<SouloftheMasochistBuff>())
                player.AddBuff(ModContent.BuffType<SaucerMinionBuff>(), 2);
        }

    }
    public class AmmoCycleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override int ToggleItemType => ModContent.ItemType<SaucerControlConsole>();
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            player.FargoSouls().AmmoCycleKey();
        }
    }
}