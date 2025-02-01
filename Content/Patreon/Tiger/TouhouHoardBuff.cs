using Terraria;
using Terraria.ModLoader;
namespace FargowiltasSouls.Content.Patreon.Tiger
{
    public class TouhouHoardBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            PatreonPlayer patronPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CirnoMinion>()] > 0 ||
                player.ownedProjectileCounts[ModContent.ProjectileType<DaiyouseiMinion>()] > 0 ||
                player.ownedProjectileCounts[ModContent.ProjectileType<MystiaMinion>()] > 0 ||
                player.ownedProjectileCounts[ModContent.ProjectileType<RumiaMinion>()] > 0 ||
                player.ownedProjectileCounts[ModContent.ProjectileType<WriggleMinion>()] > 0)
            {
                patronPlayer.TouhouBuff = true;
            }

            if (!patronPlayer.TouhouBuff)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
