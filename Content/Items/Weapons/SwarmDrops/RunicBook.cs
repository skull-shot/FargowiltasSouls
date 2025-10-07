using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class RunicBook : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/BossDrops", "DarkTome");
        public int timer;
        public bool lastItemUse;
        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 200;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.rare = ItemRarityID.Orange;
            Item.width = 36;
            Item.height = 40;
            Item.useTime = 55;
            Item.useAnimation = 55;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.DD2_DarkMageAttack;
            Item.shoot = ModContent.ProjectileType<RunicSigil>();
            Item.shootSpeed = 10f;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.channel = true;

            Item.value = Item.sellPrice(0, 2, 50, 0);
        }

        public override bool CanUseItem(Player player)
        {
            bool sigilCount = player.ownedProjectileCounts[Item.shoot] < 3;
            Item.UseSound = SoundID.Item8 with { Volume = sigilCount ? 1f : 0.5f };
            return base.CanUseItem(player) && timer == 0;
        }

        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 3 && base.CanShoot(player);
        }

        public override void HoldItem(Player player)
        {
            if (timer > 0)
            {
                timer--;
            }
            if (!player.controlUseItem && lastItemUse && timer == 0)
            {
                timer = Item.useTime + 5;
            }
            lastItemUse = player.controlUseItem;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "DarkTome")
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerDarkMage"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}
