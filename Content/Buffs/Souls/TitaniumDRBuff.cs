using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class TitaniumDRBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.TitaniumDRBuff = true;
            
            //kill all shards before running out
            if (player.buffTime[buffIndex] == 2 && player.HasBuff<TitaniumCDBuff>())
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.type == ProjectileID.TitaniumStormShard && proj.owner == player.whoAmI)
                    {
                        proj.Kill();
                    }
                }
            }
            
        }
    }
}
