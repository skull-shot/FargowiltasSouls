using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class AntisocialBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().Asocial = true;

            player.GetDamage(DamageClass.Summon) /= 1.5f;

            if (player.HeldItem.DamageType.CountsAsClass(DamageClass.SummonMeleeSpeed))
                player.FargoSouls().AttackSpeed /= 1.5f;
        }
    }
}