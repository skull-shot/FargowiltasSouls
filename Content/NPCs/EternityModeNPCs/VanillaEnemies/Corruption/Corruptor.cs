using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Corruption
{
    public class Corruptor : Shooters
    {
        public Corruptor() : base(140, ProjectileID.EyeFire, 5f, 1, DustID.CursedTorch, 400, 60) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Corruptor);

        public override bool SafePreAI(NPC npc)
        {
            if (AttackTimer > AttackThreshold - Telegraph && AttackTimer < AttackThreshold)
            {
                if (AttackTimer > AttackThreshold - Telegraph * 0.3f && AttackTimer % 6 == 0)
                {
                    if (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < Distance
                        && (!NeedLineOfSight || NeedLineOfSight && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        && FargoSoulsUtil.HostCheck)
                    {
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Speed * npc.SafeDirectionTo(Main.player[npc.target].Center), ProjectileType, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, DamageMultiplier), 0, Main.myPlayer);
                        if (p.IsWithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[p].friendly = false;
                            Main.projectile[p].hostile = true;
                            Main.projectile[p].netUpdate = true;
                        }
                    }
                }
            }
            return base.SafePreAI(npc);
        }
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Weak, 600);
            target.AddBuff(ModContent.BuffType<RottingBuff>(), 900);
        }
    }
}
