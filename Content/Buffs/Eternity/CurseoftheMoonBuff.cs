using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class CurseoftheMoonBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense -= 20;
            //player.GetDamage(DamageClass.Generic) -= 0.2f;
            //player.GetCritChance(DamageClass.Generic) -= 20;
            player.FargoSouls().CurseoftheMoon = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().CurseoftheMoon = true;
        }
    }
}