using FargowiltasSouls.Assets.Textures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Assets.UI
{
    // Exists to be displayed as an item icon in the Toggler UI when inflicted with Mutant's Presence.
    public class OncomingMutantItem : ModItem
    {
        public override string Texture => FargoAssets.GetAssetString("UI", "OncomingMutant");
    }
}