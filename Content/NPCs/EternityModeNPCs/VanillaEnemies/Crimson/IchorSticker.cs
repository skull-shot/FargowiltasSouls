using System;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Crimson
{
    public class IchorSticker : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.IchorSticker);
        private int SpitTimer = 300;
        private float realrotation;
        private int hitcooldown = 0;
        public override void AI(NPC npc)
        {
            base.AI(npc);
            if (npc.HasPlayerTarget)
                SpitTimer--;
            if (hitcooldown > 0)
                hitcooldown--;
            if (SpitTimer <= 0)
            {
                npc.frameCounter++;
                npc.velocity *= 0.9f;
                RotateTowards(npc, Main.player[npc.target].Center, 5);
            }
            else realrotation = npc.rotation + MathHelper.PiOver2;
            if (SpitTimer <= -60 && SpitTimer % -30 == 0)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    int count = Main.rand.Next(4, 7);
                    Vector2 rotation = Vector2.UnitX.RotatedBy(npc.rotation + MathHelper.PiOver2);
                    for (int i = 0; i < count; i++)
                    {
                        Vector2 vel = rotation * 12 + new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2));
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ProjectileID.GoldenShowerHostile, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0, Main.myPlayer);
                    }
                    npc.velocity -= rotation * 5;
                }
            }
            if (SpitTimer <= -100)
                SpitTimer = 300;

            //kill vanilla projectile behavior
            npc.ai[3] = 0;
        }
        public override void OnHitByAnything(NPC npc, Player player, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByAnything(npc, player, hit, damageDone);
            if (hitcooldown == 0)
            {
                if (SpitTimer > 30)
                    SpitTimer -= 30;
                else if (SpitTimer > 0)
                    SpitTimer = 0;
                hitcooldown = 10;
            }
        }
        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            for (int i = 0; i < 5; i++)
            {
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Top, new Vector2(Main.rand.NextFloat(-3, 4), Main.rand.NextFloat(-4, -6)), ProjectileID.GoldenShowerHostile, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0, Main.myPlayer);
            }
            //FargoSoulsUtil.XWay(5, npc.GetSource_FromThis(), npc.Center, ProjectileID.GoldenShowerHostile, 4, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 2);
        }
        private void RotateTowards(NPC npc, Vector2 target, float speed)
        {
            Vector2 LV = realrotation.ToRotationVector2();
            Vector2 PV = npc.SafeDirectionTo(target);
            float anglediff = FargoSoulsUtil.RotationDifference(LV, PV);
            if (!(anglediff > 0 || anglediff < 0))
                return;
            //change rotation towards target
            realrotation = realrotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
            npc.rotation = realrotation - MathHelper.PiOver2;
        }
    }
}
