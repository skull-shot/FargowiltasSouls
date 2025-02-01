using FargowiltasSouls.Content.Tiles.MusicBoxes;
using FargowiltasSouls.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class DeviMusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(
                    Mod,
                    MusicLoader.GetMusicSlot(musicMod, (musicMod.Version >= Version.Parse("0.1.4")) ? "Assets/Music/Strawberry_Sparkly_Sunrise" : "Assets/Music/LexusCyanixs"),
                    ModContent.ItemType<DeviMusicBox>(),
                    ModContent.TileType<Tiles.MusicBoxes.DeviMusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<DeviMusicBoxSheet>(), 0);
        }
    }
}
