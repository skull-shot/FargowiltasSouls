using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class RottingBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //inflicts DOT (8 per second).
            player.FargoSouls().Rotting = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().Rotting = true;
        }
    }
}