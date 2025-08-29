using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class SublimationBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().Sublimation = true;
            if (npc.FargoSouls().PureGazeTime >= 3)
                npc.buffTime[buffIndex] += 1;
        }
    }
}