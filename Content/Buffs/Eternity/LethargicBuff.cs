using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class LethargicBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //all item speed reduced to 75%
            player.FargoSouls().AttackSpeed -= .25f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.boss)
                return;

            NPC realNPC = FargoSoulsUtil.NPCExists(npc.realLife);
            if (realNPC != null && realNPC.boss)
                return;

            npc.FargoSouls().Lethargic = true;
        }
    }
}