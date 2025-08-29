using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class SuperBleedBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs", "PlaceholderBuff");
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().SBleed = true;
        }
    }
}