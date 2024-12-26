using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shield)]
    public class LumpOfFlesh : SoulsItem
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
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.buffImmune[BuffID.Dazed] = true;
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.aggro -= 400;
            player.FargoSouls().SkullCharm = true;
            player.FargoSouls().LumpOfFlesh = true;
            /*if (!player.ZoneDungeon)
            {
                player.npcTypeNoAggro[NPCID.SkeletonSniper] = true;
                player.npcTypeNoAggro[NPCID.SkeletonCommando] = true;
                player.npcTypeNoAggro[NPCID.TacticalSkeleton] = true;
                player.npcTypeNoAggro[NPCID.DiabolistRed] = true;
                player.npcTypeNoAggro[NPCID.DiabolistWhite] = true;
                player.npcTypeNoAggro[NPCID.Necromancer] = true;
                player.npcTypeNoAggro[NPCID.NecromancerArmored] = true;
                player.npcTypeNoAggro[NPCID.RaggedCaster] = true;
                player.npcTypeNoAggro[NPCID.RaggedCasterOpenCoat] = true;
            }*/
            player.AddEffect<PungentEyeballCursor>(Item);
            player.FargoSouls().PungentEyeball = true;
            if (player.AddEffect<PungentMinion>(Item))
            {
                player.buffImmune[ModContent.BuffType<Buffs.Minions.CrystalSkullBuff>()] = true;
                player.AddBuff(ModContent.BuffType<Buffs.Minions.PungentEyeballBuff>(), 5);
            }

            player.buffImmune[ModContent.BuffType<AnticoagulationBuff>()] = true;
            player.noKnockback = true;
            player.AddEffect<DreadShellEffect>(Item);
            player.AddEffect<ParryEffect>(Item);

            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.AddEffect<DeerclawpsDive>(Item);
            player.AddEffect<DeerclawpsEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<PungentEyeball>())
            .AddIngredient(ModContent.ItemType<SkullCharm>())
            .AddIngredient(ModContent.ItemType<DreadShell>())
            .AddIngredient(ModContent.ItemType<Deerclawps>())
            .AddIngredient(ItemID.SpectreBar, 10)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)

            .Register();
        }
    }
}