using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class HallowIlluminatedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            player.FargoSouls().Illuminated = true;

            if (player.Center.Y / 16 <= Main.worldSurface) //above ground, purge debuff immediately
            {
                if (player.buffTime[buffIndex] > 2)
                    player.buffTime[buffIndex] = 2;
            }
        }
    }
}