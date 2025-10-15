using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    //[AutoloadEquip(EquipType.Shoes)] //TODO: enable this when sheeted
    public class ZephyrBoots : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //terraspark
            player.accRunSpeed = 6.75f;
            player.rocketBoots = player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
            player.moveSpeed += 0.08f;
            player.iceSkate = true;

            //lava wader
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 420;
            player.lavaRose = true;

            //fart balloon
            player.AddEffect<ZephyrJump>(Item);
            player.jumpBoost = true;
            player.noFallDmg = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TerrasparkBoots)
                .AddIngredient(ItemID.BalloonHorseshoeFart)
                .AddIngredient(ModContent.ItemType<EurusSock>())
                .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    public class ZephyrJump : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<ZephyrBoots>();
        public override bool ExtraJumpEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.GetJumpState(ExtraJump.FartInAJar).Enable();
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
            }
        }
    }
}
