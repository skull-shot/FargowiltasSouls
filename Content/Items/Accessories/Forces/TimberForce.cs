using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class TimberForce : BaseForce
    {

        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<WoodEnchant>(),
                ModContent.ItemType<BorealWoodEnchant>(),
                ModContent.ItemType<RichMahoganyEnchant>(),
                ModContent.ItemType<EbonwoodEnchant>(),
                ModContent.ItemType<ShadewoodEnchant>(),
                ModContent.ItemType<PalmWoodEnchant>(),
                ModContent.ItemType<PearlwoodEnchant>()
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            SetActive(player);
            player.AddEffect<TimberEffect>(Item);
            // wood 
            WoodEnchant.WoodEffect(player, Item);
            // boreal
            player.AddEffect<BorealEffect>(Item);
            // mahog
            player.AddEffect<MahoganyEffect>(Item);
            // ebon
            player.AddEffect<EbonwoodEffect>(Item);
            // shade
            player.AddEffect<ShadewoodEffect>(Item);
            // palmwood
            player.AddEffect<PalmwoodEffect>(Item);
            // pearlwood
            player.AddEffect<PearlwoodEffect>(Item);
            player.AddEffect<PearlwoodManaEffect>(Item);
            //player.AddEffect<PearlwoodRainbowEffect>(Item);
        }

        public override void UpdateVanity(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
        }

        public override void UpdateInventory(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
        public class TimberEffect : AccessoryEffect
        {
            public override Header ToggleHeader => null;
            //public override int ToggleItemType => ModContent.ItemType<TimberForce>();
        }
    }
}
