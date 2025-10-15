using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Content.Tiles
{
    public class MainframePaintingSheet : ModTile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Tiles", Name);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            DustType = 7;
        }
    }
}