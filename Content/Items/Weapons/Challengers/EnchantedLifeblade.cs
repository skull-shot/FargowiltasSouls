using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.Weapons.ChallengerItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class EnchantedLifeblade : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/Challengers", Name);

        public override int NumFrames => 5;
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.BonusAttackSpeedMultiplier[Item.type] = 0.25f;
        }
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 80;
            Item.damage = 53;
            Item.knockBack = 3f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 40;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            //Item.channel = true;

            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.shoot = ModContent.ProjectileType<EnchantedLifebladeProjectile>();
            Item.shootSpeed = 30f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<LifelightBag>(2).AddTile(TileID.Solidifier).DisableDecraft().Register();
        }
    }
}