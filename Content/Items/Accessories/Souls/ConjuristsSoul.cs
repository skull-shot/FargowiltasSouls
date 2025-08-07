using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    public class ConjuristsSoul : BaseSoul
    {
        public static readonly Color ItemColor = new(0, 255, 255);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().SummonSoul = true;
            player.GetDamage(DamageClass.Summon) += 0.22f;
            player.maxMinions += 3;
            player.maxTurrets += 1;
            player.whipRangeMultiplier += 0.15f;
            player.GetKnockback(DamageClass.Summon) += 3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SummonerEmblem)
            .AddIngredient(ItemID.PapyrusScarab)
            .AddIngredient(ItemID.PygmyNecklace)
            .AddRecipeGroup("FargowiltasSouls:AnySentryAccessory")

            .AddIngredient(ItemID.Smolstar) //blade staff
            .AddIngredient(ItemID.OpticStaff)
            .AddIngredient(ItemID.DeadlySphereStaff)
            .AddIngredient(ItemID.StormTigerStaff)
            .AddIngredient(ItemID.StaffoftheFrostHydra)
            .AddIngredient(ItemID.TempestStaff)
            .AddIngredient(ItemID.MaceWhip)
            .AddIngredient(ItemID.XenoStaff)
            .AddIngredient(ItemID.EmpressBlade) //terraprisma

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();


        }
    }
}
