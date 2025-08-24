using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Assets.UI
{
    // Exists to be displayed as an item icon in the Toggler UI when inflicted with Mutant's Presence.
    public class OncomingMutantItem : ModItem
    {
        public override string Texture => FargoAssets.GetAssetString("UI", "OncomingMutant");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MutantTrophy>();
        }
    }
}