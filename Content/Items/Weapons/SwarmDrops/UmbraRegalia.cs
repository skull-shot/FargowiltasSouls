using Fargowiltas.Content.Items.Summons.SwarmSummons.Energizers;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Rarities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class UmbraRegalia : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/SwarmDrops", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.mana = 0;
            Item.damage = 480;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 25);
            Item.rare = ModContent.RarityType<AbominableRarity>();
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<UmbraRegaliaProj>();
            Item.shootSpeed = 4f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(p => p.TypeAlive<UmbraRegaliaProj>() && p.owner == player.whoAmI && p.ai[0] < 2);

        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            //if (player.altFunctionUse == 2) damage *= 4;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ai0 = 1;
            if (player.altFunctionUse == 2)
                ai0 = 0;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai0);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<PrismaRegalia>()
            .AddIngredient<EnergizerEmpress>()
            .AddIngredient<AbomEnergy>(10)

            .AddTile<CrucibleCosmosSheet>()

            .Register();
        }
    }
}
