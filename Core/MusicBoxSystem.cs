using FargowiltasSouls.Content.Items.Placables.MusicBoxes;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace FargowiltasSouls.Core
{
    public class MusicBoxSystem
    {
        public abstract class MusicBoxItem : ModItem
        {
            
            public override void SetStaticDefaults()
            {
                ItemID.Sets.CanGetPrefixes[Type] = false;
                ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
                Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
                //base.SetStaticDefaults();
            }

            /*public override void ModifyTooltips(List<TooltipLine> list)
            {
                foreach (TooltipLine line2 in list)
                {
                    if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                    {
                        line2.OverrideColor = Main.DiscoColor;
                    }
                }
            }*/
        }

        public abstract class MusicBoxTile : ModTile
        {
            public override void SetStaticDefaults()
            {
                TileID.Sets.DisableSmartCursor[Type] = true;
                TileID.Sets.HasOutlines[Type] = true;
                Main.tileFrameImportant[Type] = true;
                Main.tileObsidianKill[Type] = true;
                TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
                TileObjectData.newTile.Origin = new Point16(0, 1);
                TileObjectData.newTile.LavaDeath = false;
                TileObjectData.newTile.DrawYOffset = 2;
                //TileLoader.GetItemDropFromTypeAndStyle(Type, 1);
                TileObjectData.addTile(Type);
                AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.MusicBox"));
            }
            public override void MouseOver(int i, int j)
            {
                Player player = Main.LocalPlayer;
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type);
            }

            public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
            {
                return true;
            }

            public override void KillMultiTile(int i, int j, int frameX, int frameY)
            {
                //Item.NewItem(Item.GetSource_None(), new Vector2(i * 16, j * 16), new Vector2(32, 32), (int)TileLoader.GetItemDropFromTypeAndStyle(Type));
                base.KillMultiTile(i, j, frameX, frameY);
            }

            public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
            {
                // This code spawns the music notes when the music box is open.
                if (Lighting.UpdateEveryFrame && new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) != 0)
                {
                    return;
                }

                Tile tile = Main.tile[i, j];

                if (!TileDrawing.IsVisible(tile) || tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
                {
                    return;
                }

                int MusicNote = Main.rand.Next(570, 573);
                Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
                Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
                NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
                NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
                switch (MusicNote)
                {
                    case 572:
                        SpawnPosition.X -= 8f;
                        break;
                    case 571:
                        SpawnPosition.X -= 4f;
                        break;
                }

                Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
            }
        }
    }
}
