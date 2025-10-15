using FargowiltasSouls.Assets.Textures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Relics
{
    [LegacyName("ShadowChampionRelic")]
    public class DeathChampionRelic : BaseRelic
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Placables/Relics", "DeathChampionRelic");
        protected override int TileType => ModContent.TileType<Tiles.Relics.DeathChampionRelic>();
    }
}
