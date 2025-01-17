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
    public class EridanusMusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(
                    Mod,
                    MusicLoader.GetMusicSlot(musicMod, "Assets/Music/PlatinumStar"),
                    ModContent.ItemType<EridanusMusicBox>(),
                    ModContent.TileType<EridanusMusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<EridanusMusicBoxSheet>(), 0);
        }
    }
}
