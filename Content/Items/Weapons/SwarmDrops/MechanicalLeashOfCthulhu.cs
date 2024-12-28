using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class MechanicalLeashOfCthulhu : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.damage = 155;
            Item.width = 30;
            Item.height = 10;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.knockBack = 6f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<MechFlail>();
            Item.shootSpeed = 50f;
            Item.UseSound = null;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LeashOfCthulhu>())
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerEye"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}