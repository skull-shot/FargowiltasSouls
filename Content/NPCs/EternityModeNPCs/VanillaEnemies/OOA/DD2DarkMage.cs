using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.OOA;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Mono.CompilerServices.SymbolWriter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2DarkMage : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DD2DarkMageT3);

        public override void AI(NPC npc)
        {
            base.AI(npc);
            
            if (npc.Distance(Main.LocalPlayer.Center) > 3000 && !DD2Event.Ongoing)
            {
                npc.active = false;
            }

            int radius = npc.type == NPCID.DD2DarkMageT1 ? 600 : 900;

            EModeGlobalNPC.Aura(npc, radius, ModContent.BuffType<LethargicBuff>(), false, 254);
            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != npc.type && n.Distance(npc.Center) < radius))
            {
                n.Eternity().PaladinsShield = true;
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(n.position, n.width, n.height, DustID.CrystalPulse, 0f, -3f, 0, new Color(), 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                }
            }
            
        }
    }

    public class DD2DarkMageT1 : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DD2DarkMageT1);

        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }

        public int State = -1;
        public int PreviousState = 0;
        public int Timer = 0;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(State);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.Write7BitEncodedInt(PreviousState);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            State = binaryReader.Read7BitEncodedInt();
            Timer = binaryReader.Read7BitEncodedInt();
            PreviousState = binaryReader.Read7BitEncodedInt();
        }

        public enum States
        {
            Spawning = -1,
            Idle,
            SigilSpin,
            BookSlam,
            SplitShot
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.mageBoss = npc.whoAmI;

            npc.TargetClosest();
            if (npc.HasValidTarget && Main.player[npc.target].Center.Distance(npc.Center) > 2000)
            {
                return base.SafePreAI(npc);
            }

            Timer++;
            switch ((States)State)
            {
                case States.Spawning:
                    if (Timer >= 60)
                    {
                        ResetToIdle(npc);
                        Timer = 30;
                    }
                    return base.SafePreAI(npc);
                case States.Idle:
                    if (Timer >= 60)
                        ChooseAttack(npc);
                    break;
                case States.SigilSpin:
                    SigilSpin(npc);
                    break;
                case States.BookSlam:
                    BookSlam(npc);
                    break;
                case States.SplitShot:
                    SplitShot(npc);
                    break;
            }
            Movement(npc);
            return false;
        }

        #region States
        public void SigilSpin(NPC npc)
        {
            if (FargoSoulsUtil.HostCheck && Timer == 1 && npc.HasValidTarget)
            {
                List<int> order = new List<int> { 1, 2, 3, 4, 5, 6 };
                for (int i = 0; i < 6; i++)
                {
                    int index = Main.rand.Next(0, order.Count);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<DarkRune>(), npc.damage / 3, 1f, ai0: npc.target, ai1: i * MathHelper.TwoPi / 6, ai2: order.ElementAt(index));
                    order.RemoveAt(index);
                }
            }
            if (Timer > 600)
                ResetToIdle(npc);
        }
        public void BookSlam(NPC npc)
        {
            if (FargoSoulsUtil.HostCheck && Timer % 45 == 1 && Timer < 45 * 10 && npc.HasValidTarget)
            {
                Player player = Main.player[npc.target];
                for (int i = 0; i < 1; i++)
                {
                    Vector2 pos = player.Center + new Vector2(Main.rand.Next(-250, 250), -300);
                    npc.netUpdate = true;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<DarkTomeHostile>(), npc.damage / 3, 1f, ai2: npc.target);
                }
            }
            if (Timer > 500)
                ResetToIdle(npc);
        }
        public void SplitShot(NPC npc)
        {
            if (Timer % 35 == 1 && Timer < 5 * 35 && FargoSoulsUtil.HostCheck && npc.HasValidTarget)
            {
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, 4 * Vector2.UnitX.RotatedBy((Main.player[npc.target].Center - npc.Center).ToRotation()), ModContent.ProjectileType<DarkBolt>(), npc.damage / 4, 0.3f, ai0: npc.target);
                SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, npc.Center);
            }
            if (Timer > 300)
                ResetToIdle(npc);
        }
        #endregion
        #region Help Methods
        public void ResetState(NPC npc)
        {
            Timer = 0;
            NetSync(npc);
            npc.netUpdate = true;
        }

        public void ResetToIdle(NPC npc)
        {
            PreviousState = State;
            State = (int)States.Idle;
            ResetState(npc);
        }

        public void ChooseAttack(NPC npc)
        {
            ResetState(npc);
            List<States> states = [];
            for (int i = 1; i <= 3; i++)
            {
                if (PreviousState != i)
                {
                    states.Add((States)i);
                }
            }
            State = (int) Main.rand.NextFromCollection(states);
            NetSync(npc);
        }
        public void Movement(NPC npc)
        {
            npc.velocity = Vector2.UnitY;
            Vector2 vel = Vector2.Zero;
            int crystal = NPC.FindFirstNPC(NPCID.DD2EterniaCrystal);
            if (crystal != -1)
            {
                if (Main.npc[crystal].Center.Distance(npc.Center) < 500)
                    return;
                float dir = npc.HorizontalDirectionTo(Main.npc[crystal].Center);
                npc.spriteDirection = (int)dir;
                npc.velocity += Vector2.UnitX * dir;
            }
            else if (npc.HasValidTarget)
            {
                if (Main.player[npc.target].Center.Distance(npc.Center) < 500 || Math.Abs(Main.player[npc.target].Center.X - npc.Center.X) < 5)
                    return;
                float dir = npc.HorizontalDirectionTo(Main.player[npc.target].Center);
                npc.spriteDirection = (int)dir;
                npc.velocity += Vector2.UnitX * dir;
            }
        }
        #endregion

        public override void OnKill(NPC npc)
        {
            // instantly end T1 OOA
            if (DD2Event.Ongoing && DD2Event.OngoingDifficulty == 1 && NPC.waveNumber == 5)
            {
                NPC.waveKills = 140;
                DD2Event.CheckProgress(npc.type);
            }

            base.OnKill(npc);
        }
    }
}
