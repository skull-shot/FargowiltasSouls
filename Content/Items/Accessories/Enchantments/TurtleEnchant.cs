using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class TurtleEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(248, 156, 92);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public override void UpdateVanity(Player player)
        {
            player.FargoSouls().CactusImmune = true;
        }
        public override void UpdateInventory(Player player)
        {
            player.FargoSouls().CactusImmune = true;
        }
        public static void AddEffects(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            modPlayer.CactusImmune = true;
            player.AddEffect<CactusEffect>(item);
            player.AddEffect<TurtleEffect>(item);

            player.turtleThorns = true;
            player.thorns = 1f;


            if (player.HasEffect<TurtleEffect>() && !player.HasBuff(ModContent.BuffType<BrokenShellBuff>()) && !player.controlRight && !player.controlLeft && player.velocity.Y == 0 && !player.controlUseItem && player.whoAmI == Main.myPlayer && !modPlayer.noDodge)
            {
                modPlayer.TurtleCounter++;

                if (modPlayer.TurtleCounter > 20)
                {
                    player.AddBuff(ModContent.BuffType<ShellHideBuff>(), 2);
                }
            }
            else
            {
                modPlayer.TurtleCounter = 0;
            }

            if (modPlayer.TurtleShellHP < 20 && !player.HasBuff(ModContent.BuffType<BrokenShellBuff>()) && !modPlayer.ShellHide && modPlayer.ForceEffect<TurtleEnchant>())
            {
                modPlayer.turtleRecoverCD--;
                if (modPlayer.turtleRecoverCD <= 0)
                {
                    modPlayer.turtleRecoverCD = 240;

                    modPlayer.TurtleShellHP++;
                }
            }

            //Main.NewText($"shell HP: {TurtleShellHP}, counter: {TurtleCounter}, recovery: {turtleRecoverCD}");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TurtleHelmet)
            .AddIngredient(ItemID.TurtleScaleMail)
            .AddIngredient(ItemID.TurtleLeggings)
            .AddIngredient(null, "CactusEnchant")
            .AddIngredient(ItemID.ChlorophytePartisan)
            .AddIngredient(ItemID.Yelets)

            //chloro saber
            //
            //jungle turtle
            //.AddIngredient(ItemID.Seaweed);
            //.AddIngredient(ItemID.LizardEgg);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class TurtleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<TurtleEnchant>();

    }
}
