using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    [AutoloadEquip(EquipType.Wings)]
    public class LifeForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<CactusEnchant>(),
                ModContent.ItemType<PumpkinEnchant>(),
                ModContent.ItemType<BeeEnchant>(),
                ModContent.ItemType<SpiderEnchant>(),
                ModContent.ItemType<TurtleEnchant>(),
                ModContent.ItemType<BeetleEnchant>()
            ];
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(1000);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            SetActive(player);
            player.AddEffect<LifeForceEffect>(Item);
            modPlayer.LifeForceActive = true;
            // Cactus Enchant
            player.AddEffect<CactusEffect>(Item);
            // Pumpkin Enchant
            if (!player.HasEffect<LifeForceEffect>())
                player.AddEffect<PumpkinEffect>(Item);
            // Bee Enchant
            player.AddEffect<BeeEffect>(Item);
            player.strongBees = true;
            // Spider Enchant
            player.AddEffect<SpiderEffect>(Item);
            // Turtle Enchant
            player.AddEffect<TurtleEffect>(Item);
            // Beetle Enchant

            if (player.HasEffect<LifeForceEffect>())
                player.AddEffect<BeetleEffect>(Item);
            else
                BeetleEnchant.AddEffects(player, Item);

            //hover
            if (player.controlDown && player.controlJump && !player.mount.Active)
            {
                player.position.Y -= player.velocity.Y;
                if (player.velocity.Y > 0.1f)
                    player.velocity.Y = 0.1f;
                else if (player.velocity.Y < -0.1f)
                    player.velocity.Y = -0.1f;
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {

            player.wingsLogic = ArmorIDs.Wing.LongTrailRainbowWings;
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.175f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
            if (player.controlUp)
            {
                ascentWhenFalling *= 4f;
                ascentWhenRising *= 4f;
                constantAscend *= 4f;
            }
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 10f;
            acceleration = 0.7f;
        }
    }
    public class LifeForceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        //public override int ToggleItemType => ModContent.ItemType<LifeForce>();
    }
}
