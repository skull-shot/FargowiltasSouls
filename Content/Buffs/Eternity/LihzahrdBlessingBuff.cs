using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class LihzahrdBlessingBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void Update(Player player, ref int buffIndex)
        {
            if (Framing.GetTileSafely(player.Center).WallType == WallID.LihzahrdBrickUnsafe)
            {
                player.sunflower = true;
                player.ZonePeaceCandle = true;
                player.calmed = true;
            }
        }
    }
}