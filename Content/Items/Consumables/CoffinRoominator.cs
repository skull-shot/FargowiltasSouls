using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Tiles;
using FargowiltasSouls.Content.WorldGeneration;

namespace FargowiltasSouls.Content.Items.Consumables
{
    public class CoffinRoominator : SoulsItem
    {
        public override bool IsLoadingEnabled(Mod mod) => true;
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.UseSound = SoundID.Roar;
            Item.value = Item.sellPrice(0, 0, 0, 1);
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            //Point point = new((int)(Main.MouseWorld.X / 16), (int)(Main.MouseWorld.Y / 16));
            //WorldGen.KillTile(point.X, point.Y);
            //WorldGen.PlaceTile(point.X, point.Y, ModContent.TileType<CrackedSandstoneBricks>(), mute: true, forced: true);
            Point topCenter = Main.MouseWorld.ToTileCoordinates();
            CoffinArena.Place(topCenter);
            CoffinArena.PlacePassage(topCenter);
            return true;
        }
    }
}
