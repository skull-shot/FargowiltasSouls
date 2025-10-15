using Fargowiltas.Content.Items.Tiles;
using Fargowiltas.Content.Items.Vanity;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.PlayerDrawLayers;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Content.Rarities;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Eternal
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("MutantMask")]
    public class EternalFlame : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Armor/Eternal", Name);
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ModContent.RarityType<EternitySoulRarity>();
            Item.value = Item.sellPrice(0, 50);
            Item.defense = 50;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.50f;
            player.GetCritChance(DamageClass.Generic) += 20;

            player.maxMinions += 10;
            player.maxTurrets += 10;

            player.manaCost -= 0.25f;
            player.ammoCost75 = true;

            
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<EternalCore>() && legs.type == ModContent.ItemType<EternalLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = GetSetBonusString();
            MutantSetBonus(player, Item);
        }

        public static string GetSetBonusString()
        {
            return Language.GetTextValue($"Mods.FargowiltasSouls.SetBonus.Mutant");
        }

        public static void MutantSetBonus(Player player, Item item)
        {
            player.AddEffect<MasoAbom>(item);
            player.AddEffect<MasoRing>(item);
            player.AddBuff(ModContent.BuffType<MutantPowerBuff>(), 2);

            player.FargoSouls().MutantSetBonusItem = item;
            player.FargoSouls().GodEaterImbue = true;
            player.FargoSouls().AttackSpeed += .2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "MutantMask"))
            .AddIngredient<AbomEnergy>(10)
            .AddIngredient<EternalEnergy>(10)
            .AddTile<CrucibleCosmosSheet>()

            .Register();
        }
    }
    public class MasoAbom : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<MutantArmorHeader>();
        public override int ToggleItemType => ModContent.ItemType<EternalFlame>();
        //public override bool MinionEffect => true; no, abom is stronger than minos
        public override void PostUpdateEquips(Player player)
        {
            player.FargoSouls().AbomMinion = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AbomMinion>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), player.Center, Vector2.Zero, ModContent.ProjectileType<AbomMinion>(), 900, 10f, player.whoAmI, -1);
        }
    }
    public class MasoRing : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<MutantArmorHeader>();
        public override int ToggleItemType => ModContent.ItemType<EternalFlame>();
        public override void PostUpdateEquips(Player player)
        {
            player.FargoSouls().PhantasmalRing = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PhantasmalRing>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), player.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmalRing>(), 1700, 0f, player.whoAmI);
        }

    }
}