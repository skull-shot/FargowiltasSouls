using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AnglerEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(113, 125, 109);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 100000;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().FishSoul1 = true;
            player.fishingSkill += 10;

            //tackle bag
            player.accFishingLine = true;
            player.accTackleBox = true;
            player.accLavaFishing = true;

            //glowing fishing bobber
            player.accFishingBobber = true;
            player.overrideFishingBobber = 987;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AnglerHat)
                .AddIngredient(ItemID.AnglerVest)
                .AddIngredient(ItemID.AnglerPants)
                .AddIngredient(ItemID.LavaproofTackleBag)
                .AddIngredient(ItemID.FishingBobberGlowingStar)
                .AddIngredient(ItemID.FiberglassFishingPole)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
