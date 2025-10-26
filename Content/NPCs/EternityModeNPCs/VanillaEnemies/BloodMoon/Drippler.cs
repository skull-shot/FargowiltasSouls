using System;
using System.Collections.Generic;
using System.Linq;
using Fargowiltas.Content.Projectiles;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Night;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.BloodMoon
{
    public class Drippler : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Drippler);
        public int EatenDemonEyes;
        public NPC? CurrentScrumptiousDemonEye;
        private static List<int> DemonEyes => [NPCID.DemonEye, NPCID.DemonEye2, NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship,NPCID.CataractEye, NPCID.CataractEye2, NPCID.SleepyEye,
            NPCID.SleepyEye2, NPCID.DialatedEye, NPCID.DialatedEye2, NPCID.GreenEye, NPCID.GreenEye2, NPCID.PurpleEye, NPCID.PurpleEye2];
        public override bool SafePreAI(NPC npc)
        {
            if (EatenDemonEyes < 3)
            {
                foreach (NPC n in Main.npc.Where(x => x.active && DemonEyes.Contains(x.type)))
                {
                    // eat
                    if (Collision.CheckAABBvAABBCollision(n.position, new(n.width, n.height), npc.position, new(npc.width, npc.height)))
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                        npc.scale += 0.5f;
                        npc.width = (int)(28 * npc.scale);
                        npc.height = (int)(30 * npc.scale);
                        npc.Center = npc.Center - 2 * npc.scale * Vector2.UnitY;
                        n.active = false;
                        int healVal = (int)Math.Round((double)(n.life / 2));
                        npc.lifeMax += healVal;
                        if (FargoSoulsUtil.HostCheck)
                        {
                            npc.HealEffect(healVal);
                            npc.life += healVal;
                        }
                        EatenDemonEyes++;
                    }

                    // hover towards nearby alive demon eyes
                    if (n.Distance(npc.Center) < 16*8 && Collision.CanHitLine(npc.Center, 0, 0, n.Center, 0, 0) && CurrentScrumptiousDemonEye == null)
                    {
                        CurrentScrumptiousDemonEye = n;
                    }
                    if (CurrentScrumptiousDemonEye != null)
                    {
                        npc.velocity = FargoSoulsUtil.SmartAccel(npc.Center, n.Center, npc.velocity, 0.1f, 0.1f);
                        return false;
                    }
                    if (CurrentScrumptiousDemonEye != null && !CurrentScrumptiousDemonEye.active)
                    {
                        CurrentScrumptiousDemonEye = null;
                    }
                }
            }
            return base.SafePreAI(npc);
        }
        public override void AI(NPC npc)
        {
            base.AI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);
            //target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 300);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < EatenDemonEyes; i++)
                {
                    int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.DemonEye);
                    if (n != Main.maxNPCs)
                    {
                        Main.npc[n].life = Main.npc[n].lifeMax / 2;
                        Main.npc[n].FargoSouls().CanHordeSplit = false;
                        Main.npc[n].velocity = Main.rand.NextVector2Circular(8, 8);
                    }
                }
            }
        }
    }
}
