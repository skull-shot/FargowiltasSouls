using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Weapons.FinalUpgrades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class StyxGazerAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 14;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Collector);

            AddItemCraftCondition(ModContent.ItemType<StyxGazer>());
        }
    }
}
