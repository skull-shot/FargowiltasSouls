using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    [LegacyName("SkullCharm")]
    public class CrystalSkull : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 42;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 6);
        }
        public static void ActiveEffects(Player player, Item item, bool hasDownsides)
        {
            player.buffImmune[BuffID.Dazed] = true;
            player.buffImmune[BuffID.Webbed] = true;
            player.aggro -= 400;
            player.FargoSouls().CrystalSkull = true;
            player.AddEffect<PungentMinion>(item);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.endurance -= 0.2f;
            ActiveEffects(player, Item, true);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Generic;
            tooltipColor = null;
            scaling = null;
            return PungentMinion.BaseDamage(Main.LocalPlayer);
        }
    }
    public class PungentMinion : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LithosphericHeader>();
        public override int ToggleItemType => ModContent.ItemType<CrystalSkull>();
        public override bool MinionEffect => true;
        public static int BaseDamage (Player player) => 20;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasEffect<LithosphericEffect>())
                player.AddBuff(ModContent.BuffType<Buffs.Minions.CrystalSkullBuff>(), 5);
            else
                player.AddBuff(ModContent.BuffType<Buffs.Minions.PungentEyeballBuff>(), 5);
        }
    }
}