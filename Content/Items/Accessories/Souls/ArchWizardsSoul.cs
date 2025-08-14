using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    public class ArchWizardsSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public static readonly Color ItemColor = new(255, 83, 255);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().MagicSoul = true;
            player.GetDamage(DamageClass.Magic) += .22f;
            player.GetCritChance(DamageClass.Magic) += 10;
            player.statManaMax2 += 100;
            player.manaCost -= 0.2f;
            //accessorys
            player.manaFlower = true;
            //add mana cloak
            player.manaMagnet = true;
            player.magicCuffs = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.SorcererEmblem)
            .AddIngredient(ItemID.CelestialEmblem)
            .AddRecipeGroup("FargowiltasSouls:AnyMagicCuffs")
            .AddRecipeGroup("FargowiltasSouls:AnyManaFlower")
            //prehm weps
            .AddRecipeGroup("FargowiltasSouls:AnyGemStaff")
            .AddRecipeGroup("FargowiltasSouls:VilethorneOrCrimsonRod")
            .AddIngredient(ItemID.WeatherPain)
            .AddIngredient(ItemID.AquaScepter)
            .AddIngredient(ItemID.DemonScythe)
            //hm weps
            .AddIngredient(ItemID.SharpTears) //Blood Thorn
            .AddIngredient(ItemID.CrystalSerpent)
            .AddIngredient(ItemID.VenomStaff)
            .AddIngredient(ItemID.RainbowGun)
            .AddIngredient(ItemID.FairyQueenMagicItem) // Nightglow
            .AddIngredient(ItemID.LaserMachinegun)


            //old recipe
            //.AddIngredient(ItemID.MedusaHead)
            //.AddIngredient(ItemID.SharpTears)
            //.AddIngredient(ItemID.MagnetSphere)
            //.AddIngredient(ItemID.RainbowGun)
            //.AddIngredient(ItemID.ApprenticeStaffT3)
            //.AddIngredient(ItemID.SparkleGuitar)
            //.AddIngredient(ItemID.RazorbladeTyphoon)
            ////.AddIngredient(ItemID.BlizzardStaff);
            //.AddIngredient(ItemID.LaserMachinegun)
            //.AddIngredient(ItemID.LastPrism)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();


        }
    }
}
