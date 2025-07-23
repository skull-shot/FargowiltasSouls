using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class LeadPoisonSpreadBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }
        public override string Texture => "FargowiltasSouls/Content/Buffs/Souls/LeadPoisonBuff";
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().LeadPoison = true;
        }
    }
}