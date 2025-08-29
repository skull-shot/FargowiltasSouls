using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class GladiatorBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}