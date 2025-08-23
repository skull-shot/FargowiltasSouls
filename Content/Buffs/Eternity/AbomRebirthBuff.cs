using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class AbomRebirthBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Abominable Rebirth");
            // Description.SetDefault("You cannot die unless struck");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().AbomRebirth = true;
        }
    }
}