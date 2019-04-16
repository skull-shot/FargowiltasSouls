using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class OceanicMaul : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Oceanic Maul");
            Description.SetDefault("Defensive stats and max life are savaged, no lifesteal, no dodging");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>(mod).OceanicMaul = true;
            player.bleed = true;
            player.onFrostBurn = true;
            player.moonLeech = true;
        }
    }
}