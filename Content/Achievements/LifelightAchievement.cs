using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.Lifelight;
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
    public class LifelightAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 5;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Slayer);

            AddNPCKilledCondition("LifelightKillCondition", ModContent.NPCType<Lifelight>());
        }

        public override Position GetDefaultPosition() => new Before("THE_GREAT_SOUTHERN_PLANTKILL");
    }
}
