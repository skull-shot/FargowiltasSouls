using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Content.Tiles
{
    public class MutantStatueGift : ModTile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Tiles", "MutantStatueGift");
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

            
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Direction = Terraria.Enums.TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = Terraria.Enums.TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Mutant Statue");
            AddMapEntry(new Color(144, 144, 144), name);

        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Masochist>();
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }
        public override bool RightClick(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            //account for possibly clicking on any part of the multi-tile, negate it to have coords of top left corner
            j -= tile.TileFrameY / 18;
            //add offset to get middle bottom tile
            j += 2;
            int x = (i - (tile.TileFrameX / 18)) + (tile.TileFrameX >= 54 ? 5 : 2);
            
            
            WorldGen.KillTile(x, j, noItem: true);

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();

                packet.Write((byte)FargowiltasSouls.PacketID.DropMutantGift);
                packet.Write(x);
                packet.Write(j);
                packet.Send();
            }

            return true;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            //account for possibly clicking on any part of the multi-tile, negate it to have coords of top left corner
            i -= tile.TileFrameX / 18;
            j -= tile.TileFrameY / 18;
            //add offset to get middle bottom tile
            i += 1;
            j += 2;
            

            WorldGen.PlaceTile(i, j, ModContent.TileType<MutantStatue>(), false, false, -1, 0);
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<Items.Masochist>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }
        //public override bool CanDrop(int i, int j) => false;

    }
}