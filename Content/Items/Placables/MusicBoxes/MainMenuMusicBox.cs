using FargowiltasSouls.Content.Tiles.MusicBoxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Placables.MusicBoxes
{
    public class MainMenuMusicBox : MusicBoxSystem.MusicBoxItem
    {
        public override void SetStaticDefaults()
        {
            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Nein"), ModContent.ItemType<MainMenuMusicBox>(), ModContent.TileType<MainMenuMusicBoxSheet>());
            }
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<MainMenuMusicBoxSheet>(), 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Masochist>(), 1)
                .AddIngredient(ItemID.MusicBox, 1)
                .Register();
        }
    }
}
