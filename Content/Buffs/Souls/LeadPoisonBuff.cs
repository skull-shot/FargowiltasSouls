using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class LeadPoisonBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().LeadPoison = true;
            if (npc.buffTime[buffIndex] <= 45)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC spread = Main.npc[i];

                    if (i != npc.whoAmI && !(spread.HasBuff<LeadPoisonBuff>() || spread.HasBuff<LeadPoisonSpreadBuff>()) && spread != null && spread.active && !spread.townNPC && !spread.friendly && spread.lifeMax > 5 && Vector2.Distance(npc.Center, spread.Center) < 50)
                    {
                        spread.AddBuff(ModContent.BuffType<LeadPoisonSpreadBuff>(), 90);
                    }
                }
            }
        }
    }
}