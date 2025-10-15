using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    [LegacyName("FleshCannon")]
    public class Regurgitator : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/SwarmDrops", Name);
        public int counter;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.mana = 6;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item12;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RegurgitatorHungry>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            const int FACTOR = 10; // (Make sure this is even)

            counter++;
            if (player.ownedProjectileCounts[type] < 1 && counter % (FACTOR / 2) == 0)
            {
                Projectile.NewProjectile(source, position, velocity * 2f, type, damage, knockback, player.whoAmI, 0f, damage);
                SoundEngine.PlaySound(SoundID.NPCDeath13, position);
            }

            float rotation = MathHelper.ToRadians(7) * (float)Math.Sin((counter + 0.2) * Math.PI / (FACTOR / 2));
            Projectile.NewProjectile(source, position, velocity.RotatedBy(rotation) * 0.4f, ModContent.ProjectileType<RegurgitatorLaser>(), damage, knockback, player.whoAmI, ai0: 0);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(-rotation) * 0.4f, ModContent.ProjectileType<RegurgitatorLaser>(), damage, knockback, player.whoAmI, ai0: 1);

            if (counter >= FACTOR) //reset
                counter = 0;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "FleshHand")
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerWall"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}