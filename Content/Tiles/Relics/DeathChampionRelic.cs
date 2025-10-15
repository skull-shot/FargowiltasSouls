using FargowiltasSouls.Assets.Textures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Tiles.Relics
{
    [LegacyName("ShadowChampionRelic")]
    public class DeathChampionRelic : BaseRelic
    {
        protected override int ItemType => ModContent.ItemType<Items.Placables.Relics.DeathChampionRelic>();
    }
}
