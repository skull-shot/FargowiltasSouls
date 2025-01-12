using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;



namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class RefractorBlaster : SoulsItem
    {

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LaserRifle);
            Item.knockBack = 2.5f;
            Item.scale = 0.75f;
            Item.damage = 30;
            Item.useTime = 2;
            Item.useAnimation = 24;
            Item.shootSpeed = 15f;
            Item.value = 100000;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<PrimeLaser>();
            Item.UseSound = SoundID.Item12 with { Volume = 0.5f };
            Item.mana = 15;
            
            
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 1f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            const int NumProjectiles = 1;
            
            for (int i = 0; i < NumProjectiles; i++)
            {
                Vector2 newVelocity = velocity.RotatedBy(MathHelper.ToRadians(28 + Main.rand.NextFloat(1f, 35f)));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI, 0, 1);
            }
            for (int i = 0; i < NumProjectiles; i++)
            {
                Vector2 newVelocity = velocity.RotatedBy(MathHelper.ToRadians(-28 + Main.rand.NextFloat(-1f, -35f)));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI, 0, 1);
            }

            return false; // Return false because we don't want tModLoader to shoot projectile
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 20f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}
