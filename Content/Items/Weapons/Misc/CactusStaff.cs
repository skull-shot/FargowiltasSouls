using FargowiltasSouls.Assets.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using FargowiltasSouls.Content.Projectiles.Weapons;
namespace FargowiltasSouls.Content.Items.Weapons.Misc
{
    public class CactusStaff : ModItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/Misc", Name);
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.UseSound = SoundID.Item8;
            Item.knockBack = 3;
            Item.damage = 16;
            Item.shootSpeed = 10;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<CactusBall>();
            Item.mana = 14;
            Item.autoReuse = true;
            base.SetDefaults();
        }
    }
}
