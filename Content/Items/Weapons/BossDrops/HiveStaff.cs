using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class HiveStaff : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/BossDrops", Name);
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TheSmallSting>();
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.mana = 10;
            Item.knockBack = 0.25f;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item78;
            Item.value = 50000;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<HiveSentry>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.FindSentryRestingSpot(type, out int XPosition, out int YPosition, out int YOffset);
            YOffset += 5;
            position = new Vector2(XPosition, YPosition - YOffset);
            int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (p.IsWithinBounds(Main.maxProjectiles))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }
}