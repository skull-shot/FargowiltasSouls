using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class DefenselessBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Defenseless");
            // Description.SetDefault("Your guard is completely broken");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "毫无防御");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你的防御完全崩溃了");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().Defenseless = true;
        }
    }
}