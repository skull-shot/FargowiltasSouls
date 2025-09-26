using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class SwarmWeaponAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 15;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Collector);

            AddItemCraftCondition(
                [
                ModContent.ItemType<BigBrainBuster>(),
                ModContent.ItemType<Blender>(),
                ModContent.ItemType<DestructionCannon>(),
                ModContent.ItemType<DiffractorBlaster>(),
                ModContent.ItemType<DragonsDemise>(),
                ModContent.ItemType<EaterLauncher>(),
                ModContent.ItemType<GeminiGlaives>(),
                ModContent.ItemType<HellZone>(),
                ModContent.ItemType<Landslide>(),
                ModContent.ItemType<LeashofCthulhu>(),
                ModContent.ItemType<NukeFishron>(),
                ModContent.ItemType<OmniscienceStaff>(),
                ModContent.ItemType<Regurgitator>(),
                ModContent.ItemType<SlimeSlingingSlasher>(),
                ModContent.ItemType<TheBigSting>(),
                ModContent.ItemType<TheDestroyer>(),
                ModContent.ItemType<UmbraRegalia>(),
                ]);
        }
    }
}
