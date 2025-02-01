using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class NinjaEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(48, 49, 52);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<NinjaEffect>(Item);
            player.AddEffect<NinjaDamageEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaHood)
                .AddIngredient(ItemID.NinjaShirt)
                .AddIngredient(ItemID.NinjaPants)
                .AddIngredient(ItemID.Gi)
                .AddIngredient(ItemID.Shuriken, 100)
                .AddIngredient(ItemID.ThrowingKnife, 100)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class NinjaEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<NinjaEnchant>();
    }
    public class NinjaDamageEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
    }
}
