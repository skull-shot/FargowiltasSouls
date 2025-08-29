using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class MutantRebirthBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs", "PlaceholderDebuff");

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;

            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}