using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shield)]
    public class SupremeDeathbringerFairy : SoulsItem
    {
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<DebuffInstallKeyEffect>(),
             AccessoryEffectLoader.GetEffect<SpecialDashEffect>()];

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
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.SupremeDeathbringerFairy = true;

            //slimy shield
            player.buffImmune[BuffID.Slimed] = true;

            player.AddEffect<SlimeFallEffect>(Item);

            if (player.AddEffect<SlimyShieldEffect>(Item))
            {
                player.FargoSouls().SlimyShieldItem = Item;
            }

            //agitating lens
            player.buffImmune[ModContent.BuffType<BerserkedBuff>()] = true;
            //player.GetDamage(DamageClass.Generic) += 0.1f;
            player.AddEffect<AgitatingLensEffect>(Item);
            player.AddEffect<AgitatingLensInstall>(Item);
            player.AddEffect<DebuffInstallKeyEffect>(Item);

            //queen stinger
            player.buffImmune[ModContent.BuffType<InfestedBuff>()] = true;
            player.npcTypeNoAggro[210] = true;
            player.npcTypeNoAggro[211] = true;
            player.npcTypeNoAggro[42] = true;
            player.npcTypeNoAggro[231] = true;
            player.npcTypeNoAggro[232] = true;
            player.npcTypeNoAggro[233] = true;
            player.npcTypeNoAggro[234] = true;
            player.npcTypeNoAggro[235] = true;
            fargoPlayer.QueenStingerItem = Item;
            player.AddEffect<SpecialDashEffect>(Item);
            if (player.honey)
                player.GetArmorPenetration(DamageClass.Generic) += 5;

            //necromantic brew
            player.buffImmune[ModContent.BuffType<LethargicBuff>()] = true;
            fargoPlayer.NecromanticBrewItem = Item;
            player.AddEffect<NecroBrewSpin>(Item);
            player.AddEffect<SkeleMinionEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<SlimyShield>())
            .AddIngredient(ModContent.ItemType<AgitatingLens>())
            .AddIngredient(ModContent.ItemType<QueenStinger>())
            .AddIngredient(ModContent.ItemType<NecromanticBrew>())
            .AddIngredient(ItemID.HellstoneBar, 10)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 5)

            .AddTile(TileID.DemonAltar)

            .Register();
        }
    }
}