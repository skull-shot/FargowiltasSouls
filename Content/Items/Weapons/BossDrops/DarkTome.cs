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
        public override string Texture => "Terraria/Images/Item_531";
        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 50;
            Item.rare = ItemRarityID.Orange;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 25;
            Item.useAnimation = 25;
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
            if (player.altFunctionUse == 2)
            {
                mult = 0;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.Item9;
            }
            else
            {
                Item.UseSound = SoundID.Item8;
            }
            return base.CanUseItem(player);
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
            Projectile.NewProjectile(source, player.Center, vel, Item.shoot, Item.damage, 0, player.whoAmI, 0, 5);
            return false;
        }
    }
}
