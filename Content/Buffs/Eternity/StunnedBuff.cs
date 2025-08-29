using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Eternity
{
    public class StunnedBuff : ModBuff
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Buffs/Eternity", Name);
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Incapacitate();
            player.FargoSouls().Stunned = true;

            if (player.whoAmI == Main.myPlayer && player.buffTime[buffIndex] % 60 == 55)
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Debuffs/DizzyBird"));
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.boss)
            {
                npc.velocity.X *= 0;
                npc.velocity.Y *= 0;
                npc.frameCounter = 0;
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 3;
        }
    }
}