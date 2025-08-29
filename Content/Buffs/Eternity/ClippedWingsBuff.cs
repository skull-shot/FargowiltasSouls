using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class ClippedWingsBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.wingTime = 0;
            player.wingTimeMax = 0;
            player.rocketTime = 0;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.boss)
            {
                if (npc.buffTime[buffIndex] > 1)
                    npc.buffTime[buffIndex] = 1;
                return;
            }

            npc.position -= npc.velocity / 2;
            if (npc.velocity.Y < 0)
                npc.velocity.Y = 0;
        }
    }
}