using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class AeolusBootsAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 16;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Collector);

            AddItemCraftCondition(ModContent.ItemType<AeolusBoots>());
        }

        public override Position GetDefaultPosition() => new After("GET_TERRASPARK_BOOTS");
    }
}
