using FargowiltasSouls.Content.Tiles.MusicBoxes;
using FargowiltasSouls.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class BaronP1MusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Baron"), ModContent.ItemType<BaronP1MusicBox>(),ModContent.TileType<BaronP1MusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<BaronP1MusicBoxSheet>(), 0);
        }
    }
}
