using FargowiltasSouls.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Neck)]
    public class SnipersSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 12));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }


        public static readonly Color ItemColor = new(188, 253, 68);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //reduce ammo consume
            player.FargoSouls().RangedSoul = true;
            player.GetDamage(DamageClass.Ranged) += 0.22f;
            player.GetCritChance(DamageClass.Ranged) += 10;
            player.GetArmorPenetration(DamageClass.Generic) += 5;

            //add new effects
            player.magicQuiver = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            //.AddIngredient(ItemID.RangerEmblem)
            .AddRecipeGroup("FargowiltasSouls:AnyQuiver")
            .AddRecipeGroup("FargowiltasSouls:AnySniperScope")
            .AddRecipeGroup("FargowiltasSouls:AnySharktoothNecklace")
            //prehm
            .AddIngredient(ItemID.Blowpipe)
            .AddIngredient(ItemID.Boomstick)
            .AddIngredient(ItemID.BeesKnees)
            //.AddIngredient(ItemID.HellwingBow)
            .AddIngredient(ItemID.PhoenixBlaster)
            //hm
            .AddIngredient(ItemID.DaedalusStormbow)
            .AddIngredient(ItemID.Megashark)
            .AddIngredient(ItemID.PiranhaGun)
            //.AddIngredient(ItemID.ElfMelter)
            .AddIngredient(ItemID.Tsunami)
            //.AddIngredient(ItemID.Xenopopper)

            //old recipe
            //.AddIngredient(ItemID.DartPistol)
            //.AddIngredient(ItemID.Megashark)
            //.AddIngredient(ItemID.PulseBow)
            //.AddIngredient(ItemID.NailGun)
            //.AddIngredient(ItemID.PiranhaGun)
            //.AddIngredient(ItemID.SniperRifle)
            //.AddIngredient(ItemID.Tsunami)
            //.AddIngredient(ItemID.StakeLauncher)
            //.AddIngredient(ItemID.ElfMelter)
            //.AddIngredient(ItemID.Xenopopper)
            //.AddIngredient(ItemID.Celeb2)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();

            //alt recipe with abom energy instead of weapons
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyQuiver")
            .AddRecipeGroup("FargowiltasSouls:AnySniperScope")
            .AddRecipeGroup("FargowiltasSouls:AnySharktoothNecklace")
            .AddIngredient(ItemID.PiranhaGun)
            .AddIngredient(ItemID.Tsunami)
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();
        }
    }
}
