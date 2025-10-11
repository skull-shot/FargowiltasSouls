using FargowiltasSouls.Assets.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class SuperAttackAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 18;

        public CustomFlagCondition Condition { get; private set; }

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(AchievementCategory.Challenger);

            Condition = AddCondition("SuperAttackAchievementCondition");
        }

        public override Position GetDefaultPosition() => new Before("EYE_ON_YOU");
    }
}
