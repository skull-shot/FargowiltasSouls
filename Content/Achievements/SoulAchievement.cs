using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class SoulAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 11;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Collector);

            AddItemCraftCondition("CraftSoulCondition",
                [
                ModContent.ItemType<ArchWizardsSoul>(),
                ModContent.ItemType<BerserkerSoul>(),
                ModContent.ItemType<ColossusSoul>(),
                ModContent.ItemType<ConjuristsSoul>(),
                ModContent.ItemType<DimensionSoul>(),
                ModContent.ItemType<FlightMasterySoul>(),
                ModContent.ItemType<MasochistSoul>(),
                ModContent.ItemType<SnipersSoul>(),
                ModContent.ItemType<SupersonicSoul>(),
                ModContent.ItemType<TerrariaSoul>(),
                ModContent.ItemType<TrawlerSoul>(),
                ModContent.ItemType<UniverseSoul>(),
                ModContent.ItemType<WorldShaperSoul>(),
                ]);
        }

        public override Position GetDefaultPosition() => new After("CHAMPION_OF_TERRARIA");

        public override IEnumerable<Position> GetModdedConstraints()
        {
            yield return new After(ModContent.GetInstance<ForceAchievement>());
        }
    }
}
