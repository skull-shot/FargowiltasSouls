using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class DarkTome : SoulsItem
    {
        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 32;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 50;
            Item.rare = ItemRarityID.Orange;
            Item.width = 36;
            Item.height = 40;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<Tome>();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            // Alt fire should cost nothing
            if (player.altFunctionUse == 2)
            {
                mult = 0;
            }
        }

        public override bool CanUseItem(Player player)
        {
            int bookCount = player.ownedProjectileCounts[Item.shoot];
            if (player.altFunctionUse == 2)
            {
                if (bookCount > 0)
                {
                    Item.UseSound = SoundID.Item9;
                    return base.CanUseItem(player);
                }
                return false;
            }
            else if (bookCount < 3)
            {
                Item.UseSound = SoundID.Item8;
                return base.CanUseItem(player);
            }
            return false;
        }

        public override bool CanShoot(Player player)
        {
            return base.CanShoot(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }
            if (player.ownedProjectileCounts[Item.shoot] >= 3)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.active && p.type == type)
                    {
                        p.timeLeft = 20;
                        break;
                    }
                }
            }
            float rot = (Main.MouseWorld - player.Center).ToRotation();
            Vector2 vel = new Vector2(5f, 0f).RotatedBy(rot);
            Projectile.NewProjectile(source, player.Center, vel, Item.shoot, Item.damage, 0, player.whoAmI, 0, 7);
            return false;
        }
    }
}
