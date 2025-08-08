using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class DimensionSoul : FlightMasteryWings
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();


            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 30));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
        public override int NumFrames => 30;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.defense = 15;
            Item.value = 5000000;
            Item.rare = -12;
            Item.expert = true;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item6;
            Item.useTime = Item.useAnimation = 90;
        }

        public override bool? UseItem(Player player) => true;

        public override void UseItemFrame(Player player)
        {
            if (player.itemTime == player.itemTimeMax / 2)
            {
                player.Spawn(PlayerSpawnContext.RecallFromItem);

                for (int d = 0; d < 70; d++)
                    Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 1.5f);
            }
        }

        public override void UpdateInventory(Player player)
        {
            //cell phone
            player.accWatch = 3;
            player.accDepthMeter = 1;
            player.accCompass = 1;
            player.accFishFinder = true;
            player.accDreamCatcher = true;
            player.accOreFinder = true;
            player.accStopwatch = true;
            player.accCritterGuide = true;
            player.accJarOfSouls = true;
            player.accThirdEye = true;
            player.accCalendar = true;
            player.accWeatherRadio = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            ColossusSoul.AddEffects(player, Item, 100, 0.2f, 10);
            SupersonicSoul.AddEffects(player, Item, hideVisual);
            FlightMasterySoul.AddEffects(player, Item);
            TrawlerSoul.AddEffects(player, Item, hideVisual);
            WorldShaperSoul.AddEffects(player, Item, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient<ColossusSoul>()
            .AddIngredient<SupersonicSoul>()
            .AddIngredient<FlightMasterySoul>()
            .AddIngredient<TrawlerSoul>()
            .AddIngredient<WorldShaperSoul>()
            .AddIngredient<AbomEnergy>(10)

            .AddTile<CrucibleCosmosSheet>()

            .Register();
        }
    }
}
