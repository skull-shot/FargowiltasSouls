using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class PerfectParryAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 17;

        public CustomFlagCondition Condition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Challenger);

            Condition = AddCondition("PerfectParryAchievementCondition");
        }

        public override Position GetDefaultPosition() => new Before("EYE_ON_YOU");
    }
}
