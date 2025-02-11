using FargowiltasSouls.Content.Tiles.MusicBoxes;
using FargowiltasSouls.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class ChampionMusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions"), ModContent.ItemType<ChampionMusicBox>(), ModContent.TileType<ChampionMusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<ChampionMusicBoxSheet>(), 0);
        }
    }
}

