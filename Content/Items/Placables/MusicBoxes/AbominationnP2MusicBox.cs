using FargowiltasSouls.Content.Tiles.MusicBoxes;
using FargowiltasSouls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class AbominationnP2MusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(
                    Mod,
                    MusicLoader.GetMusicSlot(musicMod, musicMod.Version >= Version.Parse("0.1.5") ? "Assets/Music/Laevateinn_P2" : "Assets/Music/Stigma"),
                    ModContent.ItemType<AbominationnP2MusicBox>(),
                    ModContent.TileType<AbominationnP2MusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<AbominationnP2MusicBoxSheet>(), 0);
        }
    }
}
