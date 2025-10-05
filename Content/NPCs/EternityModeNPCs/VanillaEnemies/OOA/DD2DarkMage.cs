using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.OOA;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.CompilerServices.SymbolWriter;
using rail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2DarkMage : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2DarkMageT1,
            NPCID.DD2DarkMageT3
        );

        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }

        public int AnimState = -1;
        public int AnimStartTime = -1;

        public int State = -1;
        public int PreviousState = 0;
        public int Timer = 0;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(AnimState);
            binaryWriter.Write7BitEncodedInt(AnimStartTime);
            binaryWriter.Write7BitEncodedInt(State);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.Write7BitEncodedInt(PreviousState);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            AnimState = binaryReader.Read7BitEncodedInt();
            AnimStartTime = binaryReader.Read7BitEncodedInt();
            State = binaryReader.Read7BitEncodedInt();
            Timer = binaryReader.Read7BitEncodedInt();
            PreviousState = binaryReader.Read7BitEncodedInt();
        }

        public enum States
        {
            Spawning = -1,
            Idle,
            SummonSkeletons,
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
                case States.SummonSkeletons:
                    SummonSkeletons(npc);
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
        public void SummonSkeletons(NPC npc)
        {
            if (Timer == 1 && NPC.CountNPCS(NPCID.DD2SkeletonT1) >= 4)
                ResetToIdle(npc);
            if (Timer == 1 && NPC.CountNPCS(NPCID.DD2SkeletonT1) < 4)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, npc.Center);
                BeginAnimation(npc, (int)AnimStates.Conjuring);
                if (FargoSoulsUtil.HostCheck)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), new Vector2(npc.Bottom.X - i * 75, npc.Bottom.Y), NPCID.DD2SkeletonT1);
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), new Vector2(npc.Bottom.X + i * 75, npc.Bottom.Y), NPCID.DD2SkeletonT1);
                    }
                }
            }
            if (Timer > 180)
                ResetToIdle(npc);
        }

        public void SigilSpin(NPC npc)
        {
            if (Timer == 1)
            {
                BeginAnimation(npc, (int)AnimStates.Conjuring);
                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal with { Pitch = -1f, Volume = 2f }, npc.Center);
            }
            if (FargoSoulsUtil.HostCheck && Timer == 10 && npc.HasValidTarget)
            {
                List<int> order = new List<int> { 1, 2, 3, 4, 5, 6 };
                for (int i = 0; i < 6; i++)
                {
                    int index = Main.rand.Next(0, order.Count);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<DarkRune>(), npc.damage / 3, 1f, ai0: npc.target, ai1: i * MathHelper.TwoPi / 6, ai2: order.ElementAt(index));
                    order.RemoveAt(index);
                }
            }
            if (Timer > 610)
                ResetToIdle(npc);
        }
        public void BookSlam(NPC npc)
        {
            if (Timer == 1)
                BeginAnimation(npc, (int)AnimStates.Concentrating);
            new SparkParticle(npc.Center - 6 * npc.direction * Vector2.UnitX - 9 * Vector2.UnitY, -5 * Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi), Color.Pink, 0.2f, 8).Spawn();
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
            if (Timer % 35 == 5 && Timer < 5 * 35)
                BeginAnimation(npc, (int)AnimStates.Shooting);
            if (Timer % 35 == 0 && Timer < 6 * 35 && FargoSoulsUtil.HostCheck && npc.HasValidTarget)
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
            for (int i = 1; i <= 4; i++)
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
                npc.direction = (int)npc.HorizontalDirectionTo(Main.npc[crystal].Center);
                npc.spriteDirection = npc.direction;
                if (Main.npc[crystal].Center.Distance(npc.Center) < 500)
                    return;
                npc.velocity += Vector2.UnitX * npc.direction;
            }
            else if (npc.HasPlayerTarget)
            {
                npc.direction = (int)npc.HorizontalDirectionTo(Main.player[npc.target].Center);
                npc.spriteDirection = npc.direction;
                if (Main.player[npc.target].Center.Distance(npc.Center) < 500 || Math.Abs(Main.player[npc.target].Center.X - npc.Center.X) < 5)
                    return;
                npc.velocity += Vector2.UnitX * npc.direction;
            }
        }
        #endregion
        #region Animation
        public enum AnimStates
        {
            None = -1,
            Shooting,
            Conjuring,
            Concentrating
        }

        public void Animate(NPC npc)
        {
            if (AnimState == (int)AnimStates.None)
                return;

            switch ((AnimStates)AnimState)
            {
                case AnimStates.Shooting:
                    Animate_Shooting(npc);
                    break;
                case AnimStates.Conjuring:
                    Animate_Conjuring(npc);
                    break;
                case AnimStates.Concentrating:
                    Animate_Concentrating(npc);
                    break;
            }
        }

        public void BeginAnimation(NPC npc, int state)
        {
            AnimStartTime = Timer;
            AnimState = state;
            NetSync(npc);
            npc.netUpdate = true;
        }

        public void ResetAnimation(NPC npc)
        {
            AnimState = (int)AnimStates.None;
            AnimStartTime = -1;
            NetSync(npc);
            npc.netUpdate = true;
        }

        private void Animate_Shooting(NPC npc)
        {
            // 4-13
            int numFrames = 9;
            int timePerFrame = 6;
            npc.frame.X = 2;
            npc.frame.Y = Find_Frame(4, timePerFrame);
            if (Timer - AnimStartTime > numFrames * timePerFrame)
                ResetAnimation(npc);
        }

        private void Animate_Conjuring(NPC npc)
        {
            int numFrames = 26;
            int timePerFrame = 6;
            npc.frame.X = 2;
            npc.frame.Y = Find_Frame(14, timePerFrame);
            if (Timer - AnimStartTime > numFrames * timePerFrame)
                ResetAnimation(npc);
        }

        private void Animate_Concentrating(NPC npc)
        {
            int numFrames = 2;
            int timePerFrame = 8;
            npc.frame.X = 2;
            if (Math.Floor((double)Timer / timePerFrame) % 2 == 0)
                npc.frame.Y = 5;
            else
                npc.frame.Y = 6;

            if (Timer == 0)
                ResetAnimation(npc);
        }

        private int Find_Frame(int startFrame, int timePerFrame)
            => startFrame + (int)Math.Floor((decimal)(Timer - AnimStartTime) / timePerFrame);

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Animate(npc);
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
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
