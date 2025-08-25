using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    [AutoloadEquip(EquipType.Face)]
    [LegacyName("ChaliceoftheMoon")]
    public class VerdantDoomsayerMask : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<DiveEffect>(),
             AccessoryEffectLoader.GetEffect<BulbKeyEffect>()];

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 54;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 7);
            //Item.defense = 8;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            //magical bulb
            MagicalBulb.AddEffects(player, Item);
            //player.AddEffect<IvyVenomEffect>(Item);

            //lihzahrd treasure
            player.buffImmune[ModContent.BuffType<DaybrokenBuff>()] = true;
            player.buffImmune[ModContent.BuffType<FusedBuff>()] = true;
            //player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;
            fargoPlayer.LihzahrdTreasureBoxItem = Item;
            player.AddEffect<LihzahrdGroundPound>(Item);
            player.AddEffect<DiveEffect>(Item);
            player.AddEffect<LihzahrdBoulders>(Item);

            //celestial rune
            player.buffImmune[ModContent.BuffType<MarkedforDeathBuff>()] = true;
            player.AddEffect<CelestialRuneAttacks>(Item);
            //player.AddEffect<CelestialRuneOnhit>(Item);

            //chalice
            player.buffImmune[ModContent.BuffType<AtrophiedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<JammedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<ReverseManaFlowBuff>()] = true;
            player.buffImmune[ModContent.BuffType<AntisocialBuff>()] = true;
            fargoPlayer.MoonChalice = true;
            //player.AddEffect<CultistMinionEffect>(Item);

        }
        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<MagicalBulb>())
            .AddIngredient(ModContent.ItemType<LihzahrdTreasureBox>())
            .AddIngredient(ModContent.ItemType<CelestialRune>())
            .AddIngredient(ItemID.FragmentSolar, 1)
            .AddIngredient(ItemID.FragmentVortex, 1)
            .AddIngredient(ItemID.FragmentNebula, 1)
            .AddIngredient(ItemID.FragmentStardust, 1)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.LunarCraftingStation)

            .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            Player player = Main.LocalPlayer;
            damageClass = CelestialRune.GetRuneClassDamage(player);

            int damage = (int)(CelestialRuneAttacks.BaseDamage(Main.LocalPlayer) * player.ActualClassDamage(damageClass));
            if (damageClass == DamageClass.Summon)
                damage /= (int)player.ActualClassDamage(DamageClass.Summon);
            if (damageClass == DamageClass.Ranged || damageClass == DamageClass.Magic)
                damage *= 2;

            tooltipColor = null;
            scaling = null;
            return damage;
        }
    }
    /*
    public class CultistMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<ChaliceoftheMoon>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff<SouloftheMasochistBuff>())
                player.AddBuff(ModContent.BuffType<LunarCultistBuff>(), 2);
        }
    } 
    */
}
