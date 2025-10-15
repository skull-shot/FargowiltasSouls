using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using FargowiltasSouls.Assets.Textures;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class ElectricWhip : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/BossDrops", Name);
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<DestroyerGun>();
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<ElectricWhipProj>(), 69, 2, 10, 35);
            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
