﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    [LegacyName("LeashOfCthulhu")]
    public class Eyeleash : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/BossDrops", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.width = 30;
            Item.height = 10;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.knockBack = 4f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<EyeleashFlail>();
            Item.shootSpeed = 25f;
            Item.UseSound = null;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
            Item.channel = true;
        }
    }
}