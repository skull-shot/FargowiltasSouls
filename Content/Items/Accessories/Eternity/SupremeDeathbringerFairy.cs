using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class SupremeDeathbringerFairy : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<SupremeDashEffect>()];

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 4);
            //Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.SupremeDeathbringerFairy = true;

            //slimy shield
            player.buffImmune[BuffID.Slimed] = true;

            player.AddEffect<SlimeFallEffect>(Item);
            player.AddEffect<PlatformFallthroughEffect>(Item);

            //if (player.AddEffect<SlimyShieldEffect>(Item))
            //{
            //    player.FargoSouls().SlimyShieldItem = Item;
            //}

            //agitating lens
            player.buffImmune[ModContent.BuffType<BerserkedBuff>()] = true;
            //player.GetDamage(DamageClass.Generic) += 0.1f;
            player.AddEffect<AgitatingLensEffect>(Item);
            player.AddEffect<AgitatingLensInstall>(Item);
            //player.AddEffect<DebuffInstallKeyEffect>(Item);

            //queen stinger
            player.buffImmune[ModContent.BuffType<InfestedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<SwarmingBuff>()] = true;
            player.npcTypeNoAggro[210] = true;
            player.npcTypeNoAggro[211] = true;
            player.npcTypeNoAggro[42] = true;
            player.npcTypeNoAggro[231] = true;
            player.npcTypeNoAggro[232] = true;
            player.npcTypeNoAggro[233] = true;
            player.npcTypeNoAggro[234] = true;
            player.npcTypeNoAggro[235] = true;
            fargoPlayer.QueenStingerItem = Item;
            player.AddEffect<SupremeDashEffect>(Item);
            //if (player.honey)
                //player.GetArmorPenetration(DamageClass.Generic) += 5;

            //necromantic brew
            player.buffImmune[ModContent.BuffType<LethargicBuff>()] = true;
            fargoPlayer.NecromanticBrewItem = Item;
            player.AddEffect<NecroBrewSpin>(Item);
            //player.AddEffect<SkeleMinionEffect>(Item);

            // deerclawps
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.AddEffect<DeerclawpsDashDR>(Item);
            player.AddEffect<DeerclawpsEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<SlimyShield>())
            .AddIngredient(ModContent.ItemType<AgitatingLens>())
            .AddIngredient(ModContent.ItemType<QueenStinger>())
            .AddIngredient(ModContent.ItemType<Deerclawps>())
            .AddIngredient(ModContent.ItemType<NecromanticBrew>())
            .AddIngredient(ItemID.HellstoneBar, 10)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 5)

            .AddTile(TileID.DemonAltar)

            .Register();
        }

        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Generic;
            tooltipColor = null;
            scaling = null;
            return SupremeDashEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class SupremeDashEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override int ToggleItemType => ModContent.ItemType<SupremeDeathbringerFairy>();
        public static int BaseDamage(Player player) => FargoSoulsUtil.HighestDamageTypeScaling(player, 22); //all sdf projectiles inherit from this damage
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (stunned)
                return;
            player.FargoSouls().SpecialDashKey(1);
        }
    }
}