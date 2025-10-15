using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class MagicalCurseBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs", "MagicalCleanseCDBuff");

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().MagicalCurse = true;
        }
    }
}