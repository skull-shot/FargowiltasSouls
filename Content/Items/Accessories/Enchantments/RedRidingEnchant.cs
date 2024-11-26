using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class RedRidingEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(192, 27, 60);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Cyan;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<RedRidingEffect>(Item);
            player.AddEffect<RedRidingHuntressEffect>(Item);
            player.AddEffect<HuntressEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.HuntressAltHead)
            .AddIngredient(ItemID.HuntressAltShirt)
            .AddIngredient(ItemID.HuntressAltPants)
            .AddIngredient(null, "HuntressEnchant")
            .AddIngredient(ItemID.Marrow)
            .AddIngredient(ItemID.DD2BetsyBow)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class RedRidingHuntressEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
    }
    public class RedRidingEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<WillHeader>();
        public override bool ExtraAttackEffect => true;
        public override int ToggleItemType => ModContent.ItemType<RedRidingEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.RedRidingArrowCD > 0)
            {
                modPlayer.RedRidingArrowCD--;
            }
        }
        public static void SpawnArrowRain(Player player, NPC target)
        {
            if (!player.HasEffectEnchant<RedRidingEffect>())
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            Item effectItem = player.EffectItem<RedRidingEffect>();
            Item firstAmmo = PickAmmo(player);
            int arrowType = firstAmmo.shoot;
            int damage = firstAmmo.damage * (modPlayer.ForceEffect<RedRidingEnchant>() ? 7 : 5);
            //int damage = FargoSoulsUtil.HighestDamageTypeScaling(player, (int)(firstAmmo.damage * 5f));
            int heatray = Projectile.NewProjectile(player.GetSource_Accessory(effectItem), player.Center, new Vector2(0, -6f), ProjectileID.HeatRay, 0, 0, Main.myPlayer);
            Main.projectile[heatray].tileCollide = false;
            //proj spawns arrows all around it until it dies
            Projectile.NewProjectile(player.GetSource_Accessory(effectItem), target.Center.X, player.Center.Y - 500, 0f, 0f, ModContent.ProjectileType<ArrowRain>(), FargoSoulsUtil.HighestDamageTypeScaling(player, damage), 0f, player.whoAmI, arrowType, target.whoAmI);

            modPlayer.RedRidingArrowCD = modPlayer.ForceEffect<RedRidingEnchant>() ? 240 : 360;
        }
        private static Item PickAmmo(Player player)
        {
            Item item = new();
            bool flag = false;
            for (int i = 54; i < 58; i++)
            {
                if (player.inventory[i].ammo == AmmoID.Arrow && player.inventory[i].stack > 0)
                {
                    item = player.inventory[i];
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo == AmmoID.Arrow && player.inventory[j].stack > 0)
                    {
                        item = player.inventory[j];
                        break;
                    }
                }
            }

            if (item.ammo != AmmoID.Arrow)
            {
                item.SetDefaults(ItemID.VenomArrow);
            }

            return item;
        }
    }
}
