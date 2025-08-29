using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class MushroomPowerBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            float stats = 0.12f;
            if (player.FargoSouls().ForceEffect<ShroomiteEnchant>())
                stats = 0.25f;
            player.GetDamage(DamageClass.Generic) += stats;
            player.GetCritChance(DamageClass.Generic) += stats * 100;
        }
    }
}
