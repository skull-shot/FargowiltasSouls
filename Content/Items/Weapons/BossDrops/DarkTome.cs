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
        public int timer;
        public bool lastItemUse;
        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 28;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.rare = ItemRarityID.Orange;
            Item.width = 36;
            Item.height = 40;
            Item.useTime = 55;
            Item.useAnimation = 55;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<Tome>();
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.channel = true;

            Item.value = Item.sellPrice(0, 2, 50, 0);
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

        public override bool CanUseItem(Player player)
        {
            bool bookCount = player.ownedProjectileCounts[Item.shoot] < 3;
            Item.UseSound = SoundID.Item8 with { Volume = bookCount ? 1f : 0.5f };
            return base.CanUseItem(player) && timer == 0;
        }

        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 3 && base.CanShoot(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rot = (Main.MouseWorld - player.Center).ToRotation();
            Vector2 vel = new Vector2(5f, 0f).RotatedBy(rot);
            Projectile.NewProjectile(source, player.Center, vel, Item.shoot, damage, knockback, player.whoAmI, 0, 7);
            return false;
        }
    }
}
