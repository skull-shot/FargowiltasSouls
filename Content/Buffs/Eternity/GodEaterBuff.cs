using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class GodEaterBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true; //ignore most debuff immunity
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //defense removed, endurance removed, colossal DOT (45 per second)
            player.FargoSouls().GodEater = true;
            player.FargoSouls().noDodge = true;
            player.FargoSouls().MutantPresence = true;
            player.moonLeech = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense = 0;
            npc.defDefense = 0;
            npc.FargoSouls().GodEater = true;
            npc.FargoSouls().BlackInferno = true;
        }
    }
}