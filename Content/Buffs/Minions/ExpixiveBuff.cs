using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Terraria;
using Terraria.ModLoader;


namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class ExpixiveBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Minions", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<KamikazePixie>()] > 0) modPlayer.PixieMinion = true;
            if (!modPlayer.PixieMinion)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}