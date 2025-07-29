using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Martians
{
    public class ForceBubble : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ForceBubble);
        private int hitcooldown = 0;
        public override void AI(NPC npc)
        {
            base.AI(npc);
            if (hitcooldown > 0)
                hitcooldown--;
        }
        public override void OnHitByAnything(NPC npc, Player player, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByAnything(npc, player, hit, damageDone);

            if (FargoSoulsUtil.HostCheck && hitcooldown == 0)
            {
                int Damage = FargoSoulsUtil.ScaledProjectileDamage(28);

                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 10f * Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi), ProjectileID.MartianTurretBolt, Damage, 0f, Main.myPlayer);

                if (Main.rand.NextBool(3))
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 10f * npc.SafeDirectionTo(player.Center), ProjectileID.MartianTurretBolt, Damage, 0f, Main.myPlayer);
                hitcooldown = 8;
            }
        }
    }
}
