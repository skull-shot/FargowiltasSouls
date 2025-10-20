using System;
using System.Linq;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class Ghost : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Ghost);
        public override void SetDefaults(NPC npc)
        {
            npc.knockBackResist = 0f;
            //if (Main.hardMode)
                //npc.lifeMax = (int)(npc.lifeMax * 1.25f);
        }
        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            //if (Main.rand.NextBool(5) && npc.FargoSouls().CanHordeSplit)
                //EModeGlobalNPC.Horde(npc, 3);
        }
        public bool MadeDustRingForTrans;
        public override void AI(NPC npc)
        {
            base.AI(npc);
            //EModeGlobalNPC.Aura(npc, 100, BuffID.Cursed, false, 20);
            npc.dontTakeDamage = false;
            Player target = Main.player[npc.target];
            if (Collision.CanHitLine(npc.Center, 0, 0, target.Center, 0, 0) && Math.Abs(npc.Center.X - target.Center.X) <= 16*40 && Math.Abs(npc.Center.Y - target.Center.Y) <= 16*20)
            {
                if (npc.HasPlayerTarget && target.Alive() && target.direction == Math.Sign(npc.Center.X - target.Center.X) && npc.Distance(target.Center) >= 16*3)
                {
                    if (!MadeDustRingForTrans)
                    {
                        FargoSoulsUtil.DustRing(npc.Center + npc.velocity, 12, DustID.PortalBolt, 4, scale: 1.5f);
                        MadeDustRingForTrans = true;
                    }
                    npc.dontTakeDamage = true;
                    npc.position -= npc.velocity / 2; //halved speed
                    npc.Opacity = MathHelper.Lerp(npc.Opacity, 0.4f, 0.1f);
                }
                else
                {
                    if (MadeDustRingForTrans)
                    {
                        FargoSoulsUtil.DustRing(npc.Center + npc.velocity, 12, DustID.PortalBolt, 8, scale: 1.5f);
                        MadeDustRingForTrans = false;
                        SoundEngine.PlaySound(SoundID.NPCHit36 with {Pitch = 0.5f, MaxInstances = 1}, npc.Center);
                    }
                    npc.Opacity = MathHelper.Lerp(npc.Opacity, 1f, 0.1f);
                    npc.position += npc.velocity; //doubled speed
                }
            }
        }
        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            //if (Main.rand.NextBool(3))
                //modifiers.SetMaxDamage(1);
        }
    }
}
