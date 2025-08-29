using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class FlamesoftheUniverseBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //activates various vanilla debuffs
            player.FargoSouls().FlamesoftheUniverse = true;
            player.ichor = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().FlamesoftheUniverse = true;
            npc.ichor = true;
        }
    }
}