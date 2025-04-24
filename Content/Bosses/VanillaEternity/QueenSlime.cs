using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Core;
using System.Collections.Generic;
using Luminance.Common.Utilities;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class QueenSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenSlimeBoss);

        public int StompTimer;
        public int StompCounter;

        public float StompVelocityX;
        public float StompVelocityY;

        public int RainDirection;

        public bool DroppedSummon;

        private const float StompTravelTime = 60;
        private const float StompGravity = 1.6f;

        public List<States> AvailableStates = [];

        // phase 1 cycles between attacks randomly, without direct repetition
        // mixes in superslam on consistent 10 second cooldown
        public enum States
        {
            // phase 1
            Hops, // default state
            NormalSlam,
            MinionSlam,
            QuickHops,

            // both phases
            TripleSuperslam,

            // phase 2
            MinionChargeDirect,
            MinionChargeSide,
            Artillery,
            SpikeRain
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(StompTimer);
            binaryWriter.Write7BitEncodedInt(StompCounter);
            binaryWriter.Write(StompVelocityX);
            binaryWriter.Write(StompVelocityY);
            binaryWriter.Write(AI0);
            binaryWriter.Write(AI1);
            binaryWriter.Write(PreviousState);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            StompTimer = binaryReader.Read7BitEncodedInt();
            StompCounter = binaryReader.Read7BitEncodedInt();
            StompVelocityX = binaryReader.ReadSingle();
            StompVelocityY = binaryReader.ReadSingle();
            AI0 = binaryReader.ReadSingle();
            AI1 = binaryReader.ReadSingle();
            PreviousState = binaryReader.ReadSingle();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.05, MidpointRounding.ToEven);

            StompTimer = -360;
            NPC = npc;
        }
        public bool PhaseTwo => NPC.GetLifePercent() <= 0.5f;
        public NPC NPC = null;
        public Player Target => Main.player[NPC.target];
        public ref float State => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float NoContactDamage => ref NPC.ai[2];
        public ref float Split => ref NPC.ai[3];
        public ref float AI0 => ref NPC.localAI[0];
        public ref float AI1 => ref NPC.localAI[1];
        public ref float PreviousState => ref NPC.localAI[2];
        public ref float Squimgsh => ref NPC.localAI[3];

        public override bool SafePreAI(NPC npc)
        {
            NPC = npc;
            EModeGlobalNPC.queenSlimeBoss = npc.whoAmI;

            // despawn and targetting
            int num4 = 3000;
            if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > num4)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > num4)
                {
                    npc.EncourageDespawn(10);
                    return true;
                }
            }
            if (!NPC.HasValidTarget)
                return true;
            if (!PhaseTwo)
                NPC.noGravity = false;
            // ai here
            switch ((States)State)
            {
                // p1
                case States.Hops:
                    Hops();
                    break;
                case States.NormalSlam:
                    NormalSlam();
                    break;
                case States.MinionSlam:
                    MinionSlam();
                    break;
                case States.QuickHops:
                    QuickHops();
                    break;
                // both
                case States.TripleSuperslam:
                    Stompy();
                    break;
                // p2
                case States.MinionChargeDirect:
                    MinionChargeDirect();
                    break;
                case States.MinionChargeSide:
                    MinionChargeSide();
                    break;
                case States.Artillery:
                    Artillery();
                    break;
                case States.SpikeRain:
                    SpikeRain();
                    break;
            }
            if (StompTimer < 0)
                StompTimer++;

            EModeUtils.DropSummon(npc, "JellyCrystal", NPC.downedQueenSlime, ref DroppedSummon, Main.hardMode);
            return false;
        }
        #region States
        private void Hops()
        {
            int startup = 40;
            int hops = 1;
            if (Timer < startup)
            {
                Timer++;
                if (NPC.velocity.Y == 0)
                    NPC.velocity.X = 0;
                return;
            }
            int hopTimer = (int)Timer - startup;
            HopMovement(hopTimer, hops, ChooseAttackP1);
                

        }
        private void HopMovement(int hopTimer, int hops, Action endAction)
        {
            int endlag = 6;
            if (NPC.velocity.Y == 0)
            {
                if (hopTimer < hops) // start hop
                {
                    NPC.velocity.X = NPC.HorizontalDirectionTo(Target.Center) * 14;
                    int y = -12;
                    if (Target.Bottom.Y < NPC.Bottom.Y)
                        y = -12;
                    NPC.velocity.Y = y;
                    Timer++;
                }
                else
                {
                    Timer++;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    if (hopTimer > hops + endlag)
                    {
                        endAction.Invoke();
                        return;
                    }
                }
            }
            else
            {
                if (Target.Bottom.Y > NPC.Bottom.Y)
                    NPC.velocity.Y += 0.25f;
                float horDir = NPC.HorizontalDirectionTo(Target.Center);
                if (NPC.velocity.X.NonZeroSign() != horDir)
                    NPC.velocity.X += horDir * 0.5f;
            }
        }
        private void NormalSlam()
        {
            // go straight above player, then slam straight down 
            int slamPrepTime = WorldSavingSystem.MasochistModeReal ? 12 : 25;
            int endTime = WorldSavingSystem.MasochistModeReal ? 0 : 20;
            int abovePlayer = 250;
            if (Timer == 0) // moving to slam
            {
                NoContactDamage = 1;
                Vector2 destination = new(Target.Center.X, Target.Center.Y - abovePlayer);
                NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Bottom, destination, NPC.velocity, 3f, 3f);

                if (NPC.Bottom.Distance(destination) < 25f)
                {
                    Timer = 1;
                    NPC.netUpdate = true;
                }
            }
            else if (Timer < slamPrepTime) // preparing slam
            {
                if (Timer < slamPrepTime / 5)
                {
                    Vector2 destination = new(Target.Center.X, Target.Center.Y - abovePlayer);
                    NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Bottom, destination, NPC.velocity, 3f, 3f);
                }
                else
                {
                    NPC.velocity *= 0;
                }
                Timer++;
                if (Timer == slamPrepTime)
                {
                    NPC.velocity.Y = 18;
                    NPC.velocity.X *= 0;
                    NPC.noGravity = true;

                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int j = -1; j <= 1; j += 2)
                        {
                            int start = WorldSavingSystem.MasochistModeReal ? -3 : -1;
                            for (int i = start; i < 5; i++)
                            {
                                float angle = (i + 5) / 14f;
                                Vector2 dir = -Vector2.UnitY.RotatedBy(angle * MathHelper.PiOver2 * j);
                                dir.X *= 3f + (i * 0.6f);
                                dir.Y *= 2 - MathF.Pow(i * 0.3f, 2f);
                                dir *= 3;
                                dir += Main.rand.NextVector2Circular(0.5f, 0.5f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(dir) * 24, dir, ProjectileID.QueenSlimeGelAttack, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 1f, Main.myPlayer);
                            }
                        }
                        
                    }
                }
                    
            }
            else if (Timer == slamPrepTime) // slamming
            {
                NoContactDamage = 0;
                NPC.noGravity = true;

                for (int i = 0; i < 6; i++)
                {
                    int dust = Main.rand.NextFromCollection([DustID.BlueCrystalShard, DustID.PurpleCrystalShard]);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, dust);
                }
                if (NPC.velocity.Y == 0) // hit ground
                {
                    Timer = slamPrepTime + 1;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                Timer++;
                NPC.noGravity = false;
                if (Timer > slamPrepTime + endTime)
                    ResetToNeutral();
            }
        }
        private void MinionSlam()
        {
            // go straight above player, then slam straight down 
            int slamPrepTime = WorldSavingSystem.MasochistModeReal ? 12 : 25;
            int endTime = WorldSavingSystem.MasochistModeReal ? 0 : 20;
            int abovePlayer = 250;
            if (Timer == 0) // moving to slam
            {
                NoContactDamage = 1;
                Vector2 destination = new(Target.Center.X, Target.Center.Y - abovePlayer);
                NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Bottom, destination, NPC.velocity, 3f, 3f);

                if (NPC.Bottom.Distance(destination) < 25f)
                {
                    Timer = 1;
                    NPC.netUpdate = true;
                }
            }
            else if (Timer < slamPrepTime) // preparing slam
            {
                if (Timer < slamPrepTime / 5)
                {
                    Vector2 destination = new(Target.Center.X, Target.Center.Y - abovePlayer);
                    NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Bottom, destination, NPC.velocity, 3f, 3f);
                }
                else
                {
                    NPC.velocity *= 0;
                }
                Timer++;
                if (Timer == slamPrepTime)
                {
                    NPC.velocity.Y = 18;
                    NPC.velocity.X *= 0;
                    NPC.noGravity = true;
                }

            }
            else if (Timer == slamPrepTime) // slamming
            {
                NoContactDamage = 0;
                NPC.noGravity = true;

                for (int i = 0; i < 6; i++)
                {
                    int dust = Main.rand.NextFromCollection([DustID.BlueCrystalShard, DustID.PurpleCrystalShard]);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, dust);
                }
                if (NPC.velocity.Y == 0) // hit ground
                {
                    Timer = slamPrepTime + 1;
                    NPC.netUpdate = true;

                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 focus = Main.player[NPC.target].Center;
                        for (int i = 0; i < 50; i++)
                        {
                            Tile tile = Framing.GetTileSafely(focus);
                            if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                                break;
                            focus.Y += 16f;
                        }
                        focus.Y -= Player.defaultHeight / 2f;

                        for (int i = -5; i <= 5; i++)
                        {
                            Vector2 targetPos = focus;
                            targetPos.X += 330 * i;

                            float minionTravelTime = StompTravelTime + Main.rand.Next(30);
                            float minionGravity = 0.4f;
                            Vector2 vel = targetPos - NPC.Center;
                            vel.X /= minionTravelTime;
                            vel.Y = vel.Y / minionTravelTime - 0.5f * minionGravity * minionTravelTime;

                            FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<GelatinSlime>(), NPC.whoAmI, minionTravelTime, minionGravity, vel.X, vel.Y, target: NPC.target);
                        }
                    }
                }
            }
            else if (Timer < slamPrepTime + endTime)
            {
                Timer++;
                NPC.noGravity = false;
            }
            else
            {
                ResetToNeutral();
                //int hopTimer = (int)Timer - (slamPrepTime + endTime);
                //int hops = 2;
                //HopMovement(hopTimer, hops, ResetToNeutral);
            }
        }
        private void QuickHops()
        {
            ref float minionTimer = ref AI0;
            ref float minionDir = ref AI1;
            int startup = 15;
            int hops = 4;
            int endlag = 6;
            if (Timer < startup)
            {
                Timer++;
                if (NPC.velocity.Y == 0)
                    NPC.velocity.X = 0;
                return;
            }
            int hopTimer = (int)Timer - startup;
            if (NPC.velocity.Y == 0)
            {
                if (hopTimer < hops) // start hop
                {
                    NPC.velocity.X = NPC.HorizontalDirectionTo(Target.Center) * 9;
                    int y = -18;
                    if (Target.Bottom.Y < NPC.Bottom.Y)
                        y = -18;
                    NPC.velocity.Y = y;
                    if (Math.Abs(Target.Center.X - NPC.Center.X) < 240)
                        NPC.velocity.X *= -0.7f;
                    Timer++;

                    if (!WorldSavingSystem.MasochistModeReal)
                        minionTimer *= -1;
                    minionTimer = minionTimer.NonZeroSign();
                    minionDir = NPC.HorizontalDirectionTo(Target.Center);
                }
                else
                {
                    Timer++;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    if (hopTimer > hops + endlag)
                    {
                        ResetToNeutral();
                        return;
                    }
                }
            }
            else
            {
                if (minionTimer > 0) // every other jump
                {
                    int freq = 8;
                    if (minionTimer % freq == 0 && minionTimer <= 3 * freq)
                    {
                        SoundEngine.PlaySound(SoundID.Item155, NPC.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            int i = (int)(minionTimer / freq) - 2; // -1, 0, 1
                            int offset = i * 240 * (int)minionDir; // offset in x-position from player when diving
                            Vector2 dir = new(-minionDir * 700 + offset / 10, -300);
                            dir.Normalize();
                            Vector2 vel = 20f * dir;
                            FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<GelatinFlyer>(), NPC.whoAmI, target: NPC.target,
                                velocity: vel, ai0: offset, ai2: minionDir);
                        }
                            
                    }
                    minionTimer++;
                }
                
                NPC.velocity.Y += 0.4f;
                float horDir = NPC.HorizontalDirectionTo(Target.Center);
                if (NPC.velocity.X.NonZeroSign() != horDir)
                    NPC.velocity.X += horDir * 0.15f;
            }


        }
        private void Stompy()
        {
            NPC npc = NPC;
            if (StompTimer == 0) //ready to super stomp
            {
                StompTimer = 1;

                SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);

                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, NPCID.WallofFleshEye);

                npc.netUpdate = true;
                NetSync(npc);
                return;
            }
            else if (StompTimer > 0 && StompTimer < 30) //give time to react
            {
                StompTimer++;

                npc.rotation = 0;

                if (NPCInAnyTiles(npc))
                    npc.position.Y -= 16;

                return;
            }
            else if (StompTimer == 30)
            {
                if (!npc.HasValidTarget)
                    npc.TargetClosest(false);

                if (npc.HasValidTarget && StompCounter++ < 3)
                {
                    StompTimer++;

                    npc.ai[1] = 1f;

                    Vector2 target = Main.player[npc.target].Center;
                    for (int i = 0; i < 3; i++)
                    {
                        Tile tile = Framing.GetTileSafely(target);
                        if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                            break;
                        target.Y += 16f;
                    }
                    target.Y -= Player.defaultHeight;

                    Vector2 distance = target - npc.Bottom;
                    if (StompCounter == 1 || StompCounter == 2)
                        distance.X += 300f * Math.Sign(Main.player[npc.target].Center.X - npc.Center.X);
                    float time = StompTravelTime;
                    if (StompCounter < 0) //enraged
                        time /= 2;
                    distance.X /= time;
                    distance.Y = distance.Y / time - 0.5f * StompGravity * time;
                    StompVelocityX = distance.X;
                    StompVelocityY = distance.Y;

                    SoundEngine.PlaySound(SoundID.Item92, npc.Center);

                    npc.netUpdate = true;
                    NetSync(npc);
                    return;
                }
                else //done enough stomps
                {
                    StompCounter = 0;
                    StompTimer = -720;
                    if (PhaseTwo)
                        StompTimer *= 2;

                    npc.velocity.X = 0;

                    ResetToNeutral();
                    return;
                }
            }
            else if (StompTimer > 30)
            {
                npc.rotation = 0;
                npc.noTileCollide = true;

                float time = StompTravelTime;
                if (StompCounter < 0) //enraged
                    time /= 2;

                if (++StompTimer > time + 30)
                {
                    npc.noTileCollide = false;

                    //when landed on a surface
                    if (Grounded(npc) || StompTimer >= time * 2 + 25)
                    {
                        npc.velocity = Vector2.Zero;

                        if (WorldSavingSystem.MasochistModeReal)
                        {
                            StompTimer = 25;
                        }
                        else
                        {
                            StompTimer = /*NPC.AnyNPCs(ModContent.NPCType<GelatinSlime>()) ? 1 :*/ 15;
                        }

                        if (npc.DeathSound != null)
                            SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            int smashDamage = WorldSavingSystem.MasochistModeReal && Main.getGoodWorld
                                ? FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage) : 0;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.QueenSlimeSmash, smashDamage, 0f, Main.myPlayer);

                            for (int j = -1; j <= 1; j += 2) //spray spikes
                            {
                                Vector2 baseVel = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(10) * j));
                                const int max = 12;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 vel = Main.rand.NextFloat(5f, 15f) * j * baseVel.RotatedBy(MathHelper.PiOver4 * 0.8f / max * i * -j);
                                    vel *= WorldSavingSystem.MasochistModeReal ? 2f : 1.5f;
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ProjectileID.QueenSlimeMinionBlueSpike, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
                                }
                            }
                        }

                        return;
                    }
                }

                //damn queen slime ai glitching out and not fastfalling properly sometimes
                float correction = StompVelocityY - npc.velocity.Y;
                if (correction > StompGravity)
                    npc.position.Y += correction;

                npc.velocity.X = StompVelocityX;
                npc.velocity.Y = StompVelocityY;
                StompVelocityY += StompGravity;

                return;
            }
        }
        private void MinionChargeDirect()
        {

        }
        private void MinionChargeSide()
        {

        }
        private void Artillery()
        {
            int windup = 35;
            int attackLength = 60;
            int endlag = 30;
            float horDir = NPC.HorizontalDirectionTo(Target.Center);
            Vector2 desiredPos = Target.Center + Vector2.UnitX * -horDir * 490;
            Movement(desiredPos, 1f);

            Timer++;
            if (Timer < windup)
            {

            }
            else if (Timer < windup + attackLength)
            {
                float attackTimer = Timer - windup;
                float progress = attackTimer / attackLength;
                if (attackTimer % 5 == 0)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        float vel = 10f + 20f * MathF.Pow(progress, 0.8f);
                        float sqrt2 = MathF.Sqrt(2);
                        float verticalDir = MathHelper.Lerp(-0.6f, -0.9f, MathF.Pow(progress, 0.8f));
                        Vector2 dir = new(horDir / sqrt2, verticalDir / sqrt2);
                        Vector2 offset = Main.rand.NextVector2Circular(NPC.width / 8, NPC.height / 8);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset + dir * 24, dir * vel, ProjectileID.QueenSlimeGelAttack, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 1f, Main.myPlayer);
                    }
                }
            }
            else if (Timer > windup + attackLength + endlag)
            {
                ResetToNeutral();
            }
        }
        private void SpikeRain()
        {

        }
        #endregion
        #region Help Methods
        public void ResetState()
        {
            Timer = 0;
            AI0 = 0;
            AI1 = 0;
            NetSync(NPC);
            NPC.netUpdate = true;
        }
        public void ResetToNeutral()
        {
            if (!PhaseTwo)
            {
                PreviousState = State;
                State = (int)States.Hops;
                ResetState();
            }
            else
            {
                if (AvailableStates.Count == 0)
                {
                    List<States> states = [
                        //States.MinionChargeDirect, 
                        //States.MinionChargeSide, 
                        States.Artillery,
                        //States.SpikeRain
                        ];
                    AvailableStates.AddRange(states);
                }
                //AvailableStates.Remove((States)State);
                State = (int)Main.rand.NextFromCollection(AvailableStates);
                AvailableStates.Remove((States)State);
                NetSync(NPC);
                NPC.netUpdate = true;
                ResetState();
            }
            CheckStompy();
        }
        public void ChooseAttackP1()
        {
            ResetState();
            List<States> states = [States.NormalSlam, States.MinionSlam, States.QuickHops];
            states.Remove((States)PreviousState);
            State = (int)Main.rand.NextFromCollection(states);
            NetSync(NPC);

            // debug
            //State = (int)States.QuickHops;
            CheckStompy();
        }
        public void CheckStompy()
        {
            if (StompTimer < 0 && (!Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Target.Center, 1, 1) || NPC.Distance(Target.Center) > 1200))
            {
                StompCounter = -3; // enrage
                StompTimer = 0;
            }
                
            if (StompTimer >= 0)
            {
                State = (int)States.TripleSuperslam;
                NetSync(NPC);
            }
        }
        public void Movement(Vector2 targetPos, float speedModifier)
        {
            float accel = 1f * speedModifier;
            float decel = 1.5f * speedModifier;
            float resistance = NPC.velocity.Length() * accel / (22f * speedModifier);
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, targetPos, NPC.velocity, accel - resistance, decel + resistance);
        }
        private static bool Grounded(NPC npc) => npc.velocity.Y == 0 || NPCInAnyTiles(npc);
        private static bool NPCInAnyTiles(NPC npc)
        {
            //WHERE'S TJHE FKC IJNGI METHOD FOR HTIS? ITS NOT COLLISION.SOLKIDCOLLIOSOM ITS NOPT COLLISON.SOLDITILES I HATE 1.4 IHATE TMODLAOREI I HATE THIS FUSPTID FUCKIGN GNAME SOFU KIGN MCUCH FUCK FUCK FUCK
            bool isInTilesIncludingPlatforms = false;
            for (int x = 0; x < npc.width; x += 16)
            {
                for (float y = npc.height / 2; y < npc.height; y += 16)
                {
                    Tile tile = Framing.GetTileSafely((int)(npc.position.X + x) / 16, (int)(npc.position.Y + y) / 16);
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                    {
                        isInTilesIncludingPlatforms = true;
                        break;
                    }
                }
            }

            return isInTilesIncludingPlatforms;
        }
        #endregion

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (npc.lifeRegen >= 0)
                return;
            if (npc.life < npc.lifeMax / 2)
                modifiers.FinalDamage *= 0.8f;

            base.ModifyIncomingHit(npc, ref modifiers);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.lifeRegen < 0)
            {
                if (npc.life < npc.lifeMax / 2)
                {
                    npc.lifeRegen = (int)Math.Round(npc.lifeRegen * 0.8f);
                    damage = (int)(Math.Round(damage *0.8f));
                }
            }
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (NoContactDamage > 0)
                return false;
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 240);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadBossHeadSprite(recolor, 38);
            LoadGore(recolor, 1258);
            LoadGore(recolor, 1259);
            LoadExtra(recolor, 177);
            LoadExtra(recolor, 180);
            LoadExtra(recolor, 185);
            LoadExtra(recolor, 186);
            LoadProjectile(recolor, ProjectileID.QueenSlimeGelAttack);
            LoadProjectile(recolor, ProjectileID.QueenSlimeMinionPinkBall);
            LoadProjectile(recolor, ProjectileID.QueenSlimeMinionBlueSpike);

        }

        private static MiscShaderData queenSlimeBuffer;

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool resprite = WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors;
            if (!resprite)
                return base.PreDraw(npc, spriteBatch, screenPos, drawColor);

            queenSlimeBuffer = GameShaders.Misc["QueenSlime"];
            GameShaders.Misc["QueenSlime"] = GameShaders.Misc["FargowiltasSouls:QueenSlime"];
            
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
         
            bool resprite = WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors;
            if (!resprite)
            {
                return;
            }
            
            GameShaders.Misc["QueenSlime"] = queenSlimeBuffer;
        }
    }

    public class QueenSlimeMinion : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.QueenSlimeMinionBlue,
            NPCID.QueenSlimeMinionPink,
            NPCID.QueenSlimeMinionPurple
        );

        public bool TimeToFly;
        public bool Landed;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            bitWriter.WriteBit(TimeToFly);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            TimeToFly = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (WorldSavingSystem.MasochistModeReal)
                npc.knockBackResist = 0;
        }

        public override void OnFirstTick(NPC npc)
        {
            if (NPC.AnyNPCs(NPCID.QueenSlimeBoss))
            {
                npc.life = 0;
                npc.active = false;
                npc.timeLeft = 0;
                return;
            }
        }
        public override void AI(NPC npc)
        {
            if (NPC.AnyNPCs(NPCID.QueenSlimeBoss))
            {
                npc.life = 0;
                npc.active = false;
                npc.timeLeft = 0;
                return;
            }
            base.AI(npc);

            if (WorldSavingSystem.MasochistModeReal)
            {
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss))
                {
                    Vector2 target = Main.player[Main.npc[EModeGlobalNPC.queenSlimeBoss].target].Top;
                    if (TimeToFly)
                    {

                        npc.velocity = Math.Min(npc.velocity.Length(), 20f) * npc.SafeDirectionTo(target);
                        npc.position += 8f * npc.SafeDirectionTo(target);

                        if (npc.Distance(target) < 300f)
                        {
                            TimeToFly = false;
                            NetSync(npc);

                            npc.velocity += 8f * npc.SafeDirectionTo(target).RotatedByRandom(MathHelper.PiOver4);
                            npc.netUpdate = true;
                        }
                    }
                    else if (npc.Distance(target) > 900f)
                    {
                        TimeToFly = true;
                        NetSync(npc);
                    }
                }
                else
                {
                    TimeToFly = false;
                }

                npc.noTileCollide = TimeToFly;
            }
            else
            {
                npc.localAI[0] = 30f; //prevent firing

                if (npc.type == NPCID.QueenSlimeMinionPurple)
                {
                    npc.position -= npc.velocity * 0.5f;
                }
                else
                {
                    if (!Landed) //tl;dr dont fall on the player
                    {
                        if (npc.velocity.Y == 0)
                            Landed = true;

                        Player p = FargoSoulsUtil.PlayerExists(Player.FindClosest(npc.Center, 0, 0));
                        if (p != null)
                        {
                            const float minDist = 16 * 8;
                            float dist = npc.Center.X - p.Center.X;
                            if (Math.Abs(dist) < minDist)
                            {
                                npc.velocity.X *= 0.95f;
                                npc.velocity.X += (minDist - Math.Abs(dist)) * Math.Sign(dist) * 0.05f;
                            }
                        }
                    }
                }
            }

            //if (npc.velocity.Y != 0)
            //    npc.localAI[0] = 25f;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 180);
            target.AddBuff(ModContent.BuffType<SmiteBuff>(), 360);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadGore(recolor, 1260);
        }
    }
   
}
