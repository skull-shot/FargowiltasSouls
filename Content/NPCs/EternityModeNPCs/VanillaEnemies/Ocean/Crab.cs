using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Ocean
{
    public class Crab : Shooters
    {
        public Crab() : base(60 * 5, ModContent.ProjectileType<BubbleHostile>(), 14, 1, DustID.Water, 500, 60) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Crab);

        public override bool SafePreAI(NPC npc)
        {
            if (AttackTimer == AttackThreshold - Telegraph)
            {
                npc.velocity.X = -npc.direction * 5;
                npc.velocity.Y = -4;
            }
            if (AttackTimer > AttackThreshold - Telegraph && AttackTimer < AttackThreshold)
            {
                AttackTimer++;

                if (AttackTimer > AttackThreshold - Telegraph * 0.3f && AttackTimer % 8 == 0)
                {
                    if (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < Distance
                        && (!NeedLineOfSight || NeedLineOfSight && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        && FargoSoulsUtil.HostCheck)
                    {
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Speed * npc.SafeDirectionTo(Main.player[npc.target].Center).RotatedByRandom(MathHelper.PiOver2 * 0.3f), ProjectileType, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, DamageMultiplier), 0, Main.myPlayer);
                        if (p.IsWithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[p].friendly = false;
                            Main.projectile[p].hostile = true;
                            Main.projectile[p].netUpdate = true;
                        }
                    }
                }
                return false;
            }
            return base.SafePreAI(npc);
        }
    }
}
