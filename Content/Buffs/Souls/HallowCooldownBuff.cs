using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class HallowCooldownBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}