using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class GladiatorSpiritBuff : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/GladiatorBuff";
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.1f;
            player.noKnockback = true;
        }
    }
}