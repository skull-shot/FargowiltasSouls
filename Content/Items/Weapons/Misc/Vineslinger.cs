using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.JungleMimic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Misc
{
    public class Vineslinger : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/Misc", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 10;
            Item.value = Item.sellPrice(0, 8);
            Item.rare = ItemRarityID.LightRed;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.knockBack = 5.5f;
            Item.damage = 40;
            Item.scale = 1.1f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<VineslingerBall>();
            Item.shootSpeed = 38;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
        }
    }
}