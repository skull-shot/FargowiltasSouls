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
            player.statManaMax2 += 200;
            //accessorys
            player.manaFlower = true;
            //add mana cloak
            player.manaMagnet = true;
            player.magicCuffs = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(null, "ApprenticesEssence")
            .AddIngredient(ItemID.ManaCloak)
            .AddIngredient(ItemID.MagnetFlower)
            .AddIngredient(ItemID.ArcaneFlower)

            .AddIngredient(ItemID.CelestialCuffs)
            .AddIngredient(ItemID.CelestialEmblem)
            .AddIngredient(ItemID.MedusaHead)
            .AddIngredient(ItemID.SharpTears)
            .AddIngredient(ItemID.MagnetSphere)
            .AddIngredient(ItemID.RainbowGun)

            .AddIngredient(ItemID.ApprenticeStaffT3)
            .AddIngredient(ItemID.SparkleGuitar)
            .AddIngredient(ItemID.RazorbladeTyphoon)

            //.AddIngredient(ItemID.BlizzardStaff);
            .AddIngredient(ItemID.LaserMachinegun)
            .AddIngredient(ItemID.LastPrism)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();


        }
    }
}
