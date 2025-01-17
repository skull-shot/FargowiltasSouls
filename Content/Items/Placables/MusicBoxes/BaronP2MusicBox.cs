using FargowiltasSouls.Content.Tiles.MusicBoxes;
using FargowiltasSouls.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class BaronP2MusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(
                    Mod,
                    MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Baron2"),
                    ModContent.ItemType<BaronP2MusicBox>(),
                    ModContent.TileType<BaronP2MusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<BaronP2MusicBoxSheet>(), 0);
        }
    }
}
