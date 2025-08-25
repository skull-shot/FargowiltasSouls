using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Content.Projectiles.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Misc
{
    public class TophatSquirrelWeapon : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/Misc", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 77; // 22 * 7/2

            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 45;
            Item.useTime = 45;

            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.6f;

            Item.mana = 66;

            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<TopHatSquirrelProj>();
            Item.shootSpeed = 8f;

            Item.value = Item.sellPrice(0, 20);
        }

        //public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        //{
        //    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

        //    return false;
        //}

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<TopHatSquirrelCaught>(), 10)
            .AddIngredient(ItemID.ChlorophyteBar, 5)
            .AddIngredient(ItemID.SoulofFright, 3)
            .AddIngredient(ItemID.SoulofSight, 3)
            .AddIngredient(ItemID.SoulofMight, 3)
            .AddIngredient(ItemID.SoulofLight, 3)
            .AddIngredient(ItemID.SoulofNight, 3)
            .AddTile(TileID.MythrilAnvil)


            .Register();
        }
    }
}
