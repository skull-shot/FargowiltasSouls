using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class MagicalCleanseCDBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] > 2 && player.FargoSouls().WeaponUseTimer <= 0)
                player.buffTime[buffIndex] -= 1;
        }
    }
}