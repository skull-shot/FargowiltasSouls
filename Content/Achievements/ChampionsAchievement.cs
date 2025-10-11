using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Bosses.Lifelight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class ChampionsAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 6;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Slayer);

            AddManyNPCKilledCondition(
                [
                ModContent.NPCType<TimberChampionHead>(),
                ModContent.NPCType<TerraChampion>(),
                ModContent.NPCType<EarthChampion>(),
                ModContent.NPCType<LifeChampion>(),
                ModContent.NPCType<ShadowChampion>(),
                ModContent.NPCType<WillChampion>(),
                ModContent.NPCType<SpiritChampion>(),
                ModContent.NPCType<NatureChampion>(),
                ModContent.NPCType<CosmosChampion>(),
                ]);
        }

        public override Position GetDefaultPosition() => new After("CHAMPION_OF_TERRARIA");

        public override IEnumerable<Position> GetModdedConstraints()
        {
            yield return new After(ModContent.GetInstance<SoulAchievement>());
        }
    }
}
