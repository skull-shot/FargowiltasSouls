using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class AtrophiedBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

        }

        public override void Update(Player player, ref int buffIndex)
        {
            //melee silence hopefully plus damage reduced 99%, -all crit just in case
            player.FargoSouls().Atrophied = true;
            if (player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee) || player.HeldItem.DamageType.CountsAsClass(DamageClass.Throwing))
                player.FargoSouls().AttackSpeed /= 2;

            player.GetDamage(DamageClass.Melee) *= 0.6f;
        }
    }
}