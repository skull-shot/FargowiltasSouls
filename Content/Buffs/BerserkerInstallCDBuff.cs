using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class BerserkerInstallCDBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}