using FargowiltasSouls.Assets.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Tiles
{
    public class CactusMimicBannerSheet : ModBannerTile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Tiles", "CactusMimicBannerSheet");
    }
}
