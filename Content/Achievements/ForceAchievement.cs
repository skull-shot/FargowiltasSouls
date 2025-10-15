using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class ForceAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 10;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Collector);

            AddItemCraftCondition("CraftForceCondition",
                [
                ModContent.ItemType<TimberForce>(),
                ModContent.ItemType<TerraForce>(),
                ModContent.ItemType<EarthForce>(),
                ModContent.ItemType<LifeForce>(),
                ModContent.ItemType<DeathForce>(),
                ModContent.ItemType<WillForce>(),
                ModContent.ItemType<NatureForce>(),
                ModContent.ItemType<SpiritForce>(),
                ModContent.ItemType<CosmoForce>(),
                ]);
        }

        public override Position GetDefaultPosition() => new After("CHAMPION_OF_TERRARIA");
    }
}

