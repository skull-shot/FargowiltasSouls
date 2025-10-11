using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using FargowiltasSouls.Content.Bosses.Lifelight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class CursedCoffinAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 4;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Slayer);

            AddNPCKilledCondition("CursedCoffinKillCondition", ModContent.NPCType<CursedCoffin>());
        }

        public override Position GetDefaultPosition() => new Before("WORM_FODDER");
    }
}
