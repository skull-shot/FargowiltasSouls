using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.JungleMimic;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class JungleMimicSummonBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Minions", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<JungleMimicSummon>()] > 0)
            {
                player.buffTime[buffIndex] = 2;
            }
        }
    }
}