using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class WillForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<GoldEnchant>(),
                ModContent.ItemType<PlatinumEnchant>(),
                ModContent.ItemType<GladiatorEnchant>(),
                ModContent.ItemType<RedRidingEnchant>(),
                ModContent.ItemType<ValhallaKnightEnchant>()
            ];
        }
        public override void UpdateInventory(Player player)
        {
            player.AddEffect<GoldToPiggy>(Item);
        }
        public override void UpdateVanity(Player player)
        {
            player.AddEffect<GoldToPiggy>(Item);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            player.AddEffect<WillEffect>(Item);

            // gladi
            player.AddEffect<GladiatorBanner>(Item);
            // gold
            player.AddEffect<GoldToPiggy>(Item);
            player.AddEffect<GoldEffect>(Item);
            player.AddEffect<GoldKeyEffect>(Item);
            // platinum
            modPlayer.PlatinumEffect = Item;
            // red riding
            player.AddEffect<HuntressEffect>(Item);
            player.AddEffect<RedRidingHuntressEffect>(Item);
            // valhalla
            player.FargoSouls().ValhallaEnchantActive = true;
            player.AddEffect<ValhallaDash>(Item);
            SquireEnchant.SquireEffect(player, Item);

            if (!player.HasEffect<WillEffect>())
            {
                player.AddEffect<GladiatorSpears>(Item);
                player.AddEffect<GoldEffect>(Item);
                player.AddEffect<GoldKeyEffect>(Item);
                player.AddEffect<RedRidingEffect>(Item);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
    public class WillEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        //public override int ToggleItemType => ModContent.ItemType<WillForce>();
       
    }
}
