using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class AmbrosiaBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Souls", Name);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.08f;
            player.GetDamage(DamageClass.Generic) += 0.2f;
            //player.GetAttackSpeed(DamageClass.Melee) += 0.22f;
            player.FargoSouls().Ambrosia = true;
            //player.FargoSouls().MinionCrits = true;
        }
        
    }
}