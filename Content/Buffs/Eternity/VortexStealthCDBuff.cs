using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class VortexStealthCDBuff : ModBuff
    {   
        //if changing this, remember to update localization and vice versa
        public const int STEALTH_UPTIME = 600;
        public const int STEALTH_DOWNTIME = 900;

        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] < STEALTH_DOWNTIME)
                player.vortexStealthActive = false;
        }
    }
}