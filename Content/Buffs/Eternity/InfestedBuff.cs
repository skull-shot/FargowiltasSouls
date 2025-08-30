using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class InfestedBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer p = player.FargoSouls();

            //weak DOT that grows exponentially stronger
            if (p.FirstInfection)
            {
                p.MaxInfestTime = player.buffTime[buffIndex];
                p.FirstInfection = false;
            }

            p.Infested = true;
        }

        /*public override bool ReApply(Player player, int time, int buffIndex)
        {
            return true;
        }*/

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().Infested = true;
        }
    }
}