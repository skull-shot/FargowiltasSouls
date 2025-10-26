using FargowiltasSouls.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    public class ConjuristsSoul : BaseSoul
    {
        public static readonly Color ItemColor = new(0, 255, 255);
        protected override Color? nameColor => ItemColor;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 17));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.FargoSouls();
            modPlayer.SummonSoul = true;
            player.GetDamage(DamageClass.Summon) += 0.22f;
            if (modPlayer.MinionSlotsNonstack < 3 && modPlayer.MinionSlotsNonstack >= 0)
                modPlayer.MinionSlotsNonstack = 3;
            if (modPlayer.SentrySlotsNonstack < 1)
                modPlayer.SentrySlotsNonstack = 1;
            player.whipRangeMultiplier += 0.15f;
            player.GetKnockback(DamageClass.Summon) += 3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            //.AddIngredient(ItemID.SummonerEmblem)
            .AddIngredient(ItemID.PapyrusScarab)
            .AddIngredient(ItemID.PygmyNecklace)
            .AddRecipeGroup("FargowiltasSouls:AnySentryAccessory")
            //prehm weps
            .AddIngredient(ItemID.BabyBirdStaff) // Finch Staff
            .AddIngredient(ItemID.BlandWhip) // Leather Whip, replace with Evil Whip in 1.4.5
            .AddIngredient(ItemID.VampireFrogStaff)
            .AddIngredient(ItemID.HoundiusShootius)
            .AddIngredient(ItemID.ImpStaff)
            //hm weps
            .AddIngredient(ItemID.CoolWhip)
            .AddIngredient(ItemID.OpticStaff)
            //.AddIngredient(ItemID.DeadlySphereStaff)
            .AddIngredient(ItemID.StormTigerStaff) // Desert Tiger
            //.AddIngredient(ItemID.MaceWhip) // Morningstar
            .AddIngredient(ItemID.EmpressBlade) // Terraprisma

            //old recipe
            //.AddIngredient(ItemID.Smolstar) //blade staff
            //.AddIngredient(ItemID.OpticStaff)
            //.AddIngredient(ItemID.DeadlySphereStaff)
            //.AddIngredient(ItemID.StormTigerStaff)
            //.AddIngredient(ItemID.StaffoftheFrostHydra)
            //.AddIngredient(ItemID.TempestStaff)
            //.AddIngredient(ItemID.MaceWhip)
            //.AddIngredient(ItemID.XenoStaff)
            //.AddIngredient(ItemID.EmpressBlade) //terraprisma

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();

            //alt recipe with abom energy instead of weapons
            CreateRecipe()
            .AddIngredient(ItemID.PapyrusScarab)
            .AddIngredient(ItemID.PygmyNecklace)
            .AddRecipeGroup("FargowiltasSouls:AnySentryAccessory")
            .AddIngredient(ItemID.StormTigerStaff) // Desert Tiger
            .AddIngredient(ItemID.EmpressBlade) // Terraprisma
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();
        }
    }
}
