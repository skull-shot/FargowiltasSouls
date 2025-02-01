using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class RefractorBlaster2 : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Diffractor Blaster");
            // Tooltip.SetDefault("'The reward for a mighty rematch...'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗星炮");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'由一个被击败的敌人的武器改装而来..'");
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LaserRifle);
            Item.width = 98;
            Item.height = 38;
            Item.damage = 770;
            Item.channel = true;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.reuseDelay = 20;
            Item.UseSound = null;
            Item.shootSpeed = 15f;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<RefractorBlaster2Held>();
            Item.noUseGraphic = true;
            Item.mana = 18;
            Item.knockBack = 0.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "RefractorBlaster")
            .AddIngredient(null, "AbomEnergy", 10)
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerPrime"))
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}