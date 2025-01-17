using FargowiltasSouls.Content.Tiles.MusicBoxes;
using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class StoriaMusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(
                    Mod,
                    MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Storia"),
                    ModContent.ItemType<StoriaMusicBox>(),
                    ModContent.TileType<StoriaMusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<StoriaMusicBoxSheet>(), 0);
        }
    }
}
