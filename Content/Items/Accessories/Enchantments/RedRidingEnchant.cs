using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

            Item.rare = ItemRarityID.Yellow;
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
                .AddIngredient(ItemID.DD2ExplosiveTrapT3Popper)
                .AddIngredient(ItemID.DD2BetsyBow)

                .AddTile<EnchantedTreeSheet>()
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
            Item firstAmmo = player.FindAmmo(AmmoID.Arrow);
            if (firstAmmo.ammo != AmmoID.Arrow)
                firstAmmo.SetDefaults(ItemID.VenomArrow);
            int arrowType = firstAmmo.shoot;
            int damage = firstAmmo.damage * (modPlayer.ForceEffect<RedRidingEnchant>() ? 5 : 3);
            int heatray = Projectile.NewProjectile(player.GetSource_Accessory(effectItem), player.Center, new Vector2(0, -6f), ProjectileID.HeatRay, 0, 0, Main.myPlayer);
            Main.projectile[heatray].tileCollide = false;
            //proj spawns arrows all around it until it dies

            Projectile.NewProjectile(player.GetSource_Accessory(effectItem), target.Center.X, player.Center.Y - 500, 0f, 0f, ModContent.ProjectileType<ArrowRain>(), damage, 0f, player.whoAmI, arrowType, target.whoAmI);
            modPlayer.RedRidingArrowCD = modPlayer.ForceEffect<RedRidingEnchant>() ? 240 : 360;
        }
    }
}
