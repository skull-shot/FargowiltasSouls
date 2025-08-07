using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class TikiEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(86, 165, 43);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.whipRangeMultiplier += player.FargoSouls().ForceEffect<TikiEnchant>() ? 0.4f : 0.2f;
            player.AddEffect<TikiEffect>(item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TikiMask)
            .AddIngredient(ItemID.TikiShirt)
            .AddIngredient(ItemID.TikiPants)
            //leaf wings
            .AddIngredient(ItemID.Blowgun)
            //toxic flask
            .AddIngredient(ItemID.PygmyStaff)
            .AddIngredient(ItemID.TikiTotem)
            //kaledoscope
            //.AddIngredient(ItemID.TikiTotem);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class TikiEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
        public override int ToggleItemType => ModContent.ItemType<TikiEnchant>();
    }
}
