using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.BloodMoon;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.BloodMoon
{
    public class BloodZombie : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BloodZombie);
        public int TrailTimer;
        public int TrailCount;
        public override void AI(NPC npc)
        {
            base.AI(npc);
            TrailTimer++;
            if (npc.velocity.Y == 0 && npc.HasValidTarget && FargoSoulsUtil.HostCheck && TrailTimer % 20 == 0)
            {
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Bottom - new Vector2(0, 2), Vector2.Zero, ModContent.ProjectileType<BloodTrail>(), 0, 0, Main.myPlayer, npc.whoAmI, TrailCount);
                TrailCount++;
            }
            if (TrailCount >= 3)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].ai[1] == TrailCount - 5 && Main.projectile[i].type == ModContent.ProjectileType<BloodTrail>() && Main.projectile[i].ai[0] == npc.whoAmI)
                        Main.projectile[i].Kill();
                }
            }
            //EModeGlobalNPC.Aura(npc, 300, BuffID.Bleeding, false, 5);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);
            //target.AddBuff(ModContent.BuffType<BloodthirstyBuff>(), 240);
            //target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 600);
        }
    }
}
