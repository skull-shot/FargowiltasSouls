using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.FrostMoon
{
    public class Nutcracker : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.Nutcracker, NPCID.NutcrackerSpinning);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            LocalizedText DeathText = Language.GetText("Mods.FargowiltasSouls.DeathMessage.Nutcracker");
            if (WorldSavingSystem.MasochistModeReal && Main.getGoodWorld && target.Male)
            {
                target.KillMe(PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(target.name)), 999999, 0);
            }
        }
    }
}
