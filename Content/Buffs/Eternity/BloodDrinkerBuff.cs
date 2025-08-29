using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class BloodDrinkerBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().BloodDrinker = true;
        }
    }
}