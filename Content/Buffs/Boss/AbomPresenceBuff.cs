using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class AbomPresenceBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Boss", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().noDodge = true;
            player.FargoSouls().noSupersonic = true;
            player.moonLeech = true;
            player.bleed = true;
        }
    }
}
