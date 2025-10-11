using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Achievements
{
    public class EnchantmentAchievement : ModAchievement
    {
        public override string TextureName => FargoAssets.GetAssetString("Content/Achievements", "SoulsAchievements");

        public override int Index => 9;

        public override void SetStaticDefaults()
        {
            Achievement.SetCategory(Terraria.Achievements.AchievementCategory.Collector);

            // infinite torture nexus.
            AddItemCraftCondition("CraftEnchantmentCondition", 
                [
                ModContent.ItemType<AdamantiteEnchant>(),
                ModContent.ItemType<AncientCobaltEnchant>(),
                ModContent.ItemType<AncientHallowEnchant>(),
                ModContent.ItemType<AncientShadowEnchant>(),
                ModContent.ItemType<AnglerEnchant>(),
                ModContent.ItemType<ApprenticeEnchant>(),
                ModContent.ItemType<AshWoodEnchant>(),
                ModContent.ItemType<BeeEnchant>(),
                ModContent.ItemType<BeetleEnchant>(),
                ModContent.ItemType<BorealWoodEnchant>(),
                ModContent.ItemType<CactusEnchant>(),
                ModContent.ItemType<ChlorophyteEnchant>(),
                ModContent.ItemType<CobaltEnchant>(),
                ModContent.ItemType<CopperEnchant>(),
                ModContent.ItemType<CrimsonEnchant>(),
                ModContent.ItemType<CrystalAssassinEnchant>(),
                ModContent.ItemType<DarkArtistEnchant>(),
                ModContent.ItemType<EbonwoodEnchant>(),
                ModContent.ItemType<ForbiddenEnchant>(),
                ModContent.ItemType<FossilEnchant>(),
                ModContent.ItemType<FrostEnchant>(),
                ModContent.ItemType<GladiatorEnchant>(),
                ModContent.ItemType<GoldEnchant>(),
                ModContent.ItemType<HallowEnchant>(),
                ModContent.ItemType<HuntressEnchant>(),
                ModContent.ItemType<IronEnchant>(),
                ModContent.ItemType<JungleEnchant>(),
                ModContent.ItemType<LeadEnchant>(),
                ModContent.ItemType<MeteorEnchant>(),
                ModContent.ItemType<MinerEnchant>(),
                ModContent.ItemType<MoltenEnchant>(),
                ModContent.ItemType<MonkEnchant>(),
                ModContent.ItemType<MythrilEnchant>(),
                ModContent.ItemType<NebulaEnchant>(),
                ModContent.ItemType<NecroEnchant>(),
                ModContent.ItemType<NinjaEnchant>(),
                ModContent.ItemType<ObsidianEnchant>(),
                ModContent.ItemType<OrichalcumEnchant>(),
                ModContent.ItemType<PalladiumEnchant>(),
                ModContent.ItemType<PalmWoodEnchant>(),
                ModContent.ItemType<PearlwoodEnchant>(),
                ModContent.ItemType<PlatinumEnchant>(),
                ModContent.ItemType<PumpkinEnchant>(),
                ModContent.ItemType<RainEnchant>(),
                ModContent.ItemType<RedRidingEnchant>(),
                ModContent.ItemType<RichMahoganyEnchant>(),
                ModContent.ItemType<ShadewoodEnchant>(),
                ModContent.ItemType<ShadowEnchant>(),
                ModContent.ItemType<ShinobiEnchant>(),
                ModContent.ItemType<ShroomiteEnchant>(),
                ModContent.ItemType<SilverEnchant>(),
                ModContent.ItemType<SnowEnchant>(),
                ModContent.ItemType<SolarEnchant>(),
                ModContent.ItemType<SpectreEnchant>(),
                ModContent.ItemType<SpiderEnchant>(),
                ModContent.ItemType<SpookyEnchant>(),
                ModContent.ItemType<SquireEnchant>(),
                ModContent.ItemType<StardustEnchant>(),
                ModContent.ItemType<TikiEnchant>(),
                ModContent.ItemType<TinEnchant>(),
                ModContent.ItemType<TitaniumEnchant>(),
                ModContent.ItemType<TungstenEnchant>(),
                ModContent.ItemType<TurtleEnchant>(),
                ModContent.ItemType<ValhallaKnightEnchant>(),
                ModContent.ItemType<VortexEnchant>(),
                ModContent.ItemType<WizardEnchant>(),
                ModContent.ItemType<WoodEnchant>(),
                ModContent.ItemType<ShadowEnchant>(),
                ]);
        }

        public override Position GetDefaultPosition() => new Before("EYE_ON_YOU");
    }
}
