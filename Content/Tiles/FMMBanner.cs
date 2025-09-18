using FargowiltasSouls.Content.Items.Placables;
using FargowiltasSouls.Content.NPCs.Critters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Content.Tiles
{
    public class FMMBanner : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Banners, 0));
            //TileObjectData.newTile.Height = 3;
            //TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            //TileObjectData.newTile.StyleHorizontal = true;
            //TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.Platform, TileObjectData.newTile.Width, 0);
            //TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Banner");
            AddMapEntry(new Color(13, 88, 130), name);

            //name.AddTranslation((int)GameCulture.CultureName.Chinese, "旗帜");
        }
        public override bool CanDrop(int i, int j)
        {
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            Tile tile = Main.tile[i, j];
            TileObjectData data = TileObjectData.GetTileData(tile);
            int topLeftX = i - tile.TileFrameX / 18 % data.Width;
            int topLeftY = j - tile.TileFrameY / 18 % data.Height;
            if (WorldGen.IsBelowANonHammeredPlatform(topLeftX, topLeftY))
            {
                offsetY -= 8;
            }
            base.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref tileFrameX, ref tileFrameY);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int style = frameX / 18;
            int item = /*style == 0 ?*/ ModContent.ItemType<TophatSquirrelBanner>();// : ModContent.ItemType<FezSquirrelBanner>();
            Item.NewItem(new Terraria.DataStructures.EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, item);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer) return;

            Player player = Main.LocalPlayer;
            int style = Main.tile[i, j].TileFrameX / 18;
            int npcType = /*style = 0 ?*/ ModContent.NPCType<TophatSquirrelCritter>();// : ModContent.NPCType<FezSquirrel>();

            Main.SceneMetrics.NPCBannerBuff[npcType] = true;
            Main.SceneMetrics.hasBanner = true;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1) spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }
}