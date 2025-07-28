using FargowiltasSouls.Assets.Textures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Tiles.Relics
{
    [LegacyName("ShadowChampionRelic")]
    public class DeathChampionRelic : BaseRelic
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Tiles/Relics", "DeathChampionRelic");
        protected override int ItemType => ModContent.ItemType<Items.Placables.Relics.DeathChampionRelic>();
    }
}
