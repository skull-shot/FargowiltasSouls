using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class DaybrokenBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }
        public override string Texture => "FargowiltasSouls/Content/Buffs/Souls/SolarFlareBuff";
        public override void Update(Player player, ref int buffIndex)
        {
            //do NOT make this slow the player down. i will KILL you.
            player.FargoSouls().Daybroken = true;
        }
    }
}