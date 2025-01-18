using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Essences
{
    public class SharpshootersEssence : BaseEssence
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(188, 253, 68);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.18f;
            player.GetCritChance(DamageClass.Ranged) += 8;
            player.FargoSouls().RangedEssence = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PainterPaintballGun)
                .AddIngredient(ItemID.SnowballCannon)
                .AddIngredient(ItemID.BloodRainBow)
                .AddIngredient(ItemID.Harpoon)
                .AddIngredient(ItemID.PhoenixBlaster)
                .AddIngredient(ItemID.BeesKnees)
                .AddIngredient(ItemID.HellwingBow)
                .AddIngredient(ItemID.RangerEmblem)
                .AddIngredient(ItemID.HallowedBar, 5)

                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
