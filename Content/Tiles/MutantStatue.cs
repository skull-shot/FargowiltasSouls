using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Content.Tiles
{
    public class MutantStatue : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoAttach[Type] = true;

            

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.Direction = Terraria.Enums.TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = Terraria.Enums.TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Mutant Statue");
            AddMapEntry(new Color(144, 144, 144), name);
        }
        /*
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Placables.MutantStatue>());
        }
        */
    }
}