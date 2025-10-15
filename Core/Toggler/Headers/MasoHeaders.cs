using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Armor.Eternal;
using FargowiltasSouls.Content.Items.Materials;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Toggler.Content
{
    public abstract class MasoHeader : Header
    {
        public override float Priority => 1;
        public override string SortCategory => "Maso";
    }
    public class DeviEnergyHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<DeviatingEnergy>();
        public override float Priority => 1.1f;
    }
    public class SupremeFairyHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<SupremeDeathbringerFairy>();
        public override float Priority => 1.2f;
    }
    public class BionomicHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<BionomicCluster>();
        public override float Priority => 1.3f;
    }
    public class LithosphericHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<LithosphericCluster>();
        public override float Priority => 1.4f;
    }
    public class DubiousHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<DubiousCircuitry>();
        public override float Priority => 1.5f;
    }
    public class PureHeartHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<PureHeart>();
        public override float Priority => 1.6f;
    }
    public class VerdantHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<VerdantDoomsayerMask>();
        public override float Priority => 1.7f;
    }
    public class HeartHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<HeartoftheMasochist>();
        public override float Priority => 1.8f;
    }
    public class MutantArmorHeader : MasoHeader
    {
        public override int Item => ModContent.ItemType<EternalFlame>();
        public override float Priority => 4f;
    }
}
