using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class SilverBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override string Texture => "Terraria/Images/Buff_" + BuffID.ParryDamageBuff;
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 1f;
        }
    }
}