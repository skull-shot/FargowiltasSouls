using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class GladiatorBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            float stats = 0.08f;
            if (player.FargoSouls().ForceEffect<GladiatorEnchant>())
                stats = 0.1f;
            player.GetDamage(DamageClass.Generic) += stats;
            player.endurance += stats;
            player.noKnockback = true;
        }
    }
}