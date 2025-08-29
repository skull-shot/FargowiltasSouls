using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Terraria;
using Terraria.ModLoader;


namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class BrainMinionBuff : ModBuff
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
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BrainMinion>()] > 0) modPlayer.BrainMinion = true;
            if (!modPlayer.BrainMinion)
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