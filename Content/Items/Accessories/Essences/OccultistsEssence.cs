using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Essences
{
    public class OccultistsEssence : BaseEssence
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(0, 255, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.18f;
            player.whipRangeMultiplier += 0.1f;
            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BabyBirdStaff)
                .AddIngredient(ItemID.SlimeStaff)
                .AddIngredient(ItemID.VampireFrogStaff)
                .AddIngredient(ItemID.HoundiusShootius)
                .AddIngredient(ItemID.HornetStaff)
                .AddIngredient(ItemID.ThornWhip) //snapthorn 
                .AddIngredient(ItemID.ImpStaff)
                .AddIngredient(ItemID.SummonerEmblem)
                .AddIngredient(ItemID.HallowedBar, 5)

                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
