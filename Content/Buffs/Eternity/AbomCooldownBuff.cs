
using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class AbomCooldownBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.FargoSouls().AbominableWandRevived)
                player.buffTime[buffIndex] = 2;
        }
    }
}