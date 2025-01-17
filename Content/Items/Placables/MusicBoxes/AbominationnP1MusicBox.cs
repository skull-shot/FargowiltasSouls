using FargowiltasSouls.Content.Tiles.MusicBoxes;
using FargowiltasSouls.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class AbominationnP1MusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(
                    Mod,
                    MusicLoader.GetMusicSlot(musicMod, musicMod.Version >= Version.Parse("0.1.5") ? "Assets/Music/Laevateinn_P1" : "Assets/Music/Stigma"),
                    ModContent.ItemType<AbominationnP1MusicBox>(),
                    ModContent.TileType<AbominationnP1MusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<AbominationnP1MusicBoxSheet>(), 0);
        }
    }
}
