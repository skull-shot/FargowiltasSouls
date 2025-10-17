using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Corruption;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses.EaterOfWorlds;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses.KingSlime;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Humanizer;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static FargowiltasSouls.Content.Bosses.MutantBoss.MutantBoss;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class EaterofWorlds : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);

        int MassDefenseTimer;
        bool UseMassDefense;

        // when segments become head, these variables exist to make them retain their velocity
        static Dictionary<int, int> oldType = []; // indexed by whoami, since the npc loses this instance when it becomes head
        static Dictionary<int, Vector2> oldVelocity = [];
        Vector2 oldPos = Vector2.Zero;

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.CursedInferno] = true;
        }

        public static void CheckReset()
        {
            if (!NPC.AnyNPCs(NPCID.EaterofWorldsHead))
            {
                oldType = [];
                oldVelocity = [];
                EaterofWorldsHead.Attack = 0;
                EaterofWorldsHead.OldAttack = 0;
                EaterofWorldsHead.Timer = 0;
                EaterofWorldsHead.AvailableAttacks = [];
            }
        }

        public override bool SafePreAI(NPC npc)
        {
            if (oldType.TryGetValue(npc.whoAmI, out int value) && npc.type != value && npc.type == NPCID.EaterofWorldsHead)
            {
                npc.velocity = oldVelocity[npc.whoAmI];
                if (npc.TryGetGlobalNPC<EaterofWorldsHead>(out var head)) // skip the spawn timer, go straight to attack AI
                {
                    head.SpawnTimer = 60;
                }
                npc.netUpdate = true;
            }

            oldType[npc.whoAmI] = npc.type;
            oldVelocity[npc.whoAmI] = npc.Center - oldPos;
            oldPos = npc.Center;

            if (--MassDefenseTimer < 0)
            {
                MassDefenseTimer = 15;

                //only apply to head and the segment immediately before it
                if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody && FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.EaterofWorldsHead) != null)
                {
                    npc.defense = npc.defDefense;
                    UseMassDefense = false;

                    int totalCount = Main.npc.Count(n => n.active && (n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsTail));
                    int headCount = NPC.CountNPCS(NPCID.EaterofWorldsHead);
                    if (totalCount > 12 && headCount < totalCount / 5 + 1)
                    {
                        UseMassDefense = true;
                        npc.defense += 2;

                        if (npc.life < npc.lifeMax / 2)
                            npc.life += 1;
                    }
                }
            }

            return base.SafePreAI(npc);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (UseMassDefense)
                // TODO: maybe use defense for this?
                modifiers.FinalDamage *= 0.8f;

            base.ModifyIncomingHit(npc, ref modifiers);
        }


        public override bool CheckDead(NPC npc)
        {

            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && i != npc.whoAmI && (Main.npc[i].type == NPCID.EaterofWorldsHead || Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail))
                    count++;
            }

            if (count > 2)
                return false;

            return base.CheckDead(npc);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.lifeRegen >= 0)
                return;
            npc.lifeRegen /= 2;
            damage /= 2;
            if (UseMassDefense)
            {
                damage /= 2;
                npc.lifeRegen /= 2;
            }
        }

        public override void SafeModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            base.SafeModifyHitByItem(npc, player, item, ref modifiers);

            if (EaterofWorldsHead.HaveSpawnDR > 0)
                modifiers.FinalDamage /= 10;
        }

        public override void SafeModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.SafeModifyHitByProjectile(npc, projectile, ref modifiers);

            if (EaterofWorldsHead.HaveSpawnDR > 0)
                modifiers.FinalDamage /= projectile.numHits + 1;

            //if (projectile.FargoSouls().IsAHeldProj)
            //    modifiers.FinalDamage *= 0.8f;
        }

        public override void SafeOnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.SafeOnHitByProjectile(npc, projectile, hit, damageDone);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<RottingBuff>(), 60 * 3);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class EaterofWorldsHead : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.EaterofWorldsHead);

        // default
        public NPC? NPC = null; // who am i
        public int MyIndex; // which head am i
        public int HeadCount; // how many head
        public bool firstEater;
        public Player? Target => Main.player[NPC.target];
        public bool DroppedSummon;
        public static int HaveSpawnDR;
        public int NoSelfDestructTimer = 15;
        public static int SpecialCountdownTimer;

        public static int UndergroundLength = 600;

        public int SpawnTimer = 0;

        // attack stuff
        public static int Attack;
        public static int OldAttack;
        public static int Timer;
        public static List<Attacks> AvailableAttacks = [];
        public int AttackTimer;

        public int UTurnTotalSpacingDistance;
        public int UTurnIndividualSpacingPosition;
        public int UTurnStoredTargetX;

        public enum Attacks
        {
            DefaultMovement,
            Spit,
            JumpCrash,
            Sailing,
            Fireballs
        }
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            int firstEater = NPC.FindFirstNPC(npc.type);
            if (npc.whoAmI == firstEater)
            {
                binaryWriter.Write(Attack);
                binaryWriter.Write(OldAttack);
            }
            binaryWriter.Write(SpawnTimer);
            binaryWriter.Write(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            int firstEater = NPC.FindFirstNPC(npc.type);
            if (npc.whoAmI == firstEater)
            {
                Attack = binaryReader.ReadInt32();
                OldAttack = binaryReader.ReadInt32();
            }
            SpawnTimer = binaryReader.ReadInt32();
            AttackTimer = binaryReader.ReadInt32();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.damage = (int)(npc.damage * 4.0 / 3.0);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (Main.getGoodWorld)
                cooldownSlot = ImmunityCooldownID.Bosses;
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.eaterBoss = npc.whoAmI;
            FargoSoulsGlobalNPC.boss = npc.whoAmI;

            //if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && npc.Distance(Main.LocalPlayer.Center) < 2000)
            //{
                //Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);
            //}

            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3500)
            {
                npc.TargetClosest(false);
                if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3500)
                {
                    // check for other eater heads. if all eater heads have no target or are out of range...
                    if (!Main.npc.Any(otherNpc => otherNpc.active && otherNpc.type == npc.type && otherNpc.HasValidTarget && otherNpc.Distance(Main.player[otherNpc.target].Center) < 3500))
                    {
                        // then try to sync despawn
                        npc.velocity.Y += 0.25f;
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;
                    }
                    else if (npc.timeLeft < 30) //otherwise, ensure dont despawn
                    {
                        npc.timeLeft = 30;
                    }
                }
            }

            firstEater = NPC.FindFirstNPC(npc.type) == npc.whoAmI;

            if (firstEater)
            {
                // synced timer
                SpecialCountdownTimer++;
                if (HaveSpawnDR > 0)
                    HaveSpawnDR--;

                int num = 2;
                int num2 = NPC.GetEaterOfWorldsSegmentsCount() + num;
                int hp = 0;
                int max = npc.lifeMax * num2;
                for (int i = 0; i < 200; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.active && nPC2.type >= NPCID.EaterofWorldsHead && nPC2.type <= NPCID.EaterofWorldsTail)
                    {
                        hp += nPC2.life;
                    }
                }
                float fraction = (float)hp / max;
            }
            if (NoSelfDestructTimer <= 0)
            {
                if (FargoSoulsUtil.HostCheck && SpecialCountdownTimer % 6 == 3) //chose this number at random to avoid edge case
                {
                    //die if segment behind me is invalid
                    int ai0 = (int)npc.ai[0];
                    if (!(ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].ai[1] == npc.whoAmI
                        && (Main.npc[ai0].type == NPCID.EaterofWorldsBody || Main.npc[ai0].type == NPCID.EaterofWorldsTail)))
                    {
                        //Main.NewText("ai0 npc invalid");
                        npc.life = 0;
                        npc.HitEffect();
                        npc.checkDead();
                        npc.active = false;
                        npc.netUpdate = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        return false;
                    }
                }
            }
            else
            {
                NoSelfDestructTimer--;
            }
            
            
            bool ret = false;

            NPC = npc;
            int index = 0;
            foreach (NPC head in Main.ActiveNPCs)
            {
                if (head.TypeAlive(NPCID.EaterofWorldsHead))
                {
                    if (head.whoAmI == npc.whoAmI)
                        break;
                    index++;
                }
            }
            MyIndex = index;
            HeadCount = NPC.CountNPCS(NPCID.EaterofWorldsHead);

            if (!NPC.HasPlayerTarget)
                return true;


            switch ((Attacks)Attack)
            {
                case Attacks.DefaultMovement:
                    if (SpawnTimer < 30)
                    {
                        ret = true;
                        SpawnTimer++;
                    }
                    else
                        DefaultMovement();
                    break;

                case Attacks.Spit:
                    Spit();
                    break;

                case Attacks.JumpCrash:
                    JumpCrash();
                    break;

                case Attacks.Sailing:
                    Sailing();
                    break;

                case Attacks.Fireballs:
                    Fireballs();
                    break;


                default:
                    ret = true;
                    break;
            }

            EModeUtils.DropSummon(npc, ItemID.WormFood, NPC.downedBoss2, ref DroppedSummon);

            if (!ret) // default stuff that has to happen anyway
            {
                if (NPC.velocity != Vector2.Zero)
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }

            return ret;
        }
        public void DefaultMovement()
        {
            Vector2 targetPos = FindGround(NPC.Center.ToTileCoordinates()).ToWorldCoordinates();
            targetPos = UnevenGroundFix(targetPos, Target);
            int maxDistX = 150;
            targetPos.X = MathHelper.Clamp(targetPos.X, Target.Center.X - maxDistX, Target.Center.X + maxDistX);
            targetPos.X += Math.Abs(Target.Center.X - NPC.Center.X) * 0.4f; // inch slightly towards player
            targetPos.Y += UndergroundLength;
            Movement(targetPos, 0.5f, 0.2f, 0.2f, 1f);

            AttackTimer = 0;

            if (firstEater)
            {
                int waitTime;
                int heads = HeadCount;
                if (WorldSavingSystem.MasochistModeReal)
                {
                    waitTime = 25;
                }
                else
                {
                    waitTime = 25;
                }

                Timer++;
                if (Timer > waitTime)
                {
                    List<Attacks> GetAttacks()
                    {
                        var attacks = new List<Attacks>(AvailableAttacks);
                        // remove bad combos
                        if (HeadCount < 4)
                            attacks.Remove(Attacks.Sailing);

                        if (attacks.Count == 0)
                        {
                            AvailableAttacks = [Attacks.Spit, Attacks.JumpCrash, Attacks.Sailing, Attacks.Fireballs];
                            AvailableAttacks.Remove((Attacks)OldAttack);
                            attacks = GetAttacks();
                        }
                        return attacks;
                    }

                    if (FargoSoulsUtil.HostCheck)
                    {
                        List<Attacks> attacks = GetAttacks();
                        Attack = (int)Main.rand.NextFromCollection(attacks);
                        AvailableAttacks.Remove((Attacks)Attack);
                        OldAttack = Attack;
                    }

                    NPC.netUpdate = true;
                    Timer = 0;
                    return;
                }
            }
        }
        public void Spit()
        {
            float prepTime = 85;
            float upTime = 36;
            float leanTime = 15;
            float endlag = 60;
            if (Timer < prepTime)
            {
                if (Timer > prepTime - 10)
                    NPC.velocity *= 0.8f;
                else
                {
                    int side = MyIndex % 2 == 0 ? 1 : -1;
                    NPC mainEater = Main.npc[NPC.FindFirstNPC(NPCID.EaterofWorldsHead)];
                    side *= Target.HorizontalDirectionTo(mainEater.Center).NonZeroSign();
                    Vector2 desiredPos = Target.Center + Vector2.UnitY * UndergroundLength + Vector2.UnitX * side * 800;
                    if (NPC.Distance(desiredPos) > 120)
                        Movement(desiredPos, 3f, 3f, 3f, 5f);
                    else
                        NPC.velocity *= 0.9f;
                }
            }
            else if (Timer < prepTime + upTime)
            {
                if (NPC.velocity.Y > -12 && NPC.Center.Y > Target.Bottom.Y)
                    NPC.velocity.Y -= 1f;

                if (Timer > prepTime + upTime - leanTime)
                {
                    NPC.velocity.X += NPC.HorizontalDirectionTo(Target.Center) * 0.4f;
                }
                else
                {
                    NPC.velocity.X *= 0.8f;
                    if (Math.Abs(NPC.Center.X - Target.Center.X) < 700)
                        NPC.position.X += Target.HorizontalDirectionTo(NPC.Center) * 12f;
                }
                    
                    
            }
            else if (Timer == prepTime + upTime)
            {
                int projCount = WorldSavingSystem.MasochistModeReal ? 20 : 16;
                projCount /= HeadCount;
                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    for (float i = 0; i < projCount; i++)
                    {
                        float offset = (i - (projCount / 2)) / (projCount / 2); // from -1 to 1
                        Vector2 angle = NPC.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.25f).SafeNormalize(-Vector2.UnitY);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, angle * Main.rand.NextFloat(16, 25),
                            ModContent.ProjectileType<CorruptionSludge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                    }
                }
            }
            else
            {
                Vector2 desiredPos = Target.Center + Vector2.UnitY * UndergroundLength + Vector2.UnitX * Target.HorizontalDirectionTo(NPC.Center) * 350;
                Movement(desiredPos);
            }

            if (firstEater)
            {
                Timer++;
                if (Timer > prepTime + upTime + endlag)
                {
                    EndAttack();
                    return;
                }
            }
        }
        public void JumpCrash()
        {
            float prepTime = 80;
            float upTime = 30;
            float endlag = 100 + HeadCount * 50;
            float offsetTimer = Timer - MyIndex * 50;
            if (offsetTimer < prepTime)
            {

                if (offsetTimer > prepTime - 10)
                    NPC.velocity *= 0.9f;
                else
                {
                    int side = MyIndex % 2 == 0 ? 1 : -1;
                    Vector2 desiredPos = Target.Center + Vector2.UnitY * UndergroundLength + Vector2.UnitX * side * 1000;
                    Movement(desiredPos, 1.5f, 1.5f, 1.5f, 1.5f);
                }

                if (AttackTimer != -1)
                {
                    AttackTimer = -1;
                    NPC.netUpdate = true;
                }
            }
            else if (offsetTimer < prepTime + upTime)
            {
                if (NPC.velocity.Y > -15)
                    NPC.velocity.Y -= 1f;
                if (Math.Abs(NPC.Center.X - Target.Center.X) < 500)
                    NPC.velocity.X -= NPC.HorizontalDirectionTo(Target.Center) * 1f;
            }
            else if (AttackTimer < 0)
            {
                Vector2 desiredPos = Target.Center - Vector2.UnitY * 400;
                Movement(desiredPos, 1.5f, 1.5f, 1.5f, 1.5f);

                if (Math.Abs(NPC.Center.X - Target.Center.X) < 150 && NPC.Center.Y < Target.Top.Y)
                {
                    AttackTimer = 1;
                    NPC.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                }

            }
            else
            {
                if (AttackTimer == 1)
                {
                    if (NPC.Center.Y > Target.Bottom.Y && Collision.SolidCollision(NPC.position, NPC.width, NPC.height, true))
                    {
                        AttackTimer = 0;
                        NPC.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            int amt = WorldSavingSystem.MasochistModeReal ? 2 : 1;
                            if (HeadCount < 4)
                                amt += 4 - HeadCount;
                            for (int i = -amt; i <= amt; i++)
                            {
                                Vector2 vel = -Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i * 0.08f).RotatedByRandom(MathHelper.PiOver2 * 0.04f);
                                vel *= Main.rand.NextFloat(12f, 17f);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel,
                                    ModContent.ProjectileType<CorruptionSludge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                            }
                        }
                    }
                }
                if (NPC.Center.Y < Target.Bottom.Y)
                {
                    NPC.velocity.Y += 1f;
                    NPC.velocity.X *= 0.95f;
                }
                else
                {
                    Vector2 desiredPos = Target.Center + Vector2.UnitY * UndergroundLength + Vector2.UnitX * Target.HorizontalDirectionTo(NPC.Center) * 350;
                    Movement(desiredPos);
                }
            }

            if (firstEater)
            {
                Timer++;
                if (Timer > prepTime + upTime + endlag)
                {
                    EndAttack();
                    return;
                }
            }
        }
        public void Sailing()
        {
            if (firstEater)
            {
                if (Timer == 0)
                {
                    //SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                    UTurnTotalSpacingDistance = HeadCount / 2;
                    if (WorldSavingSystem.MasochistModeReal)
                        UTurnTotalSpacingDistance /= 2;

                    int headCounter = 0; //determine position of this head in the group
                    bool actuallyDoTheThing = true;
                    for (int i = 0; i < Main.maxNPCs; i++) //synchronize
                    {
                        if (Main.npc[i].active && Main.npc[i].type == NPC.type)
                        {
                            EaterofWorldsHead gNPC = Main.npc[i].GetGlobalNPC<EaterofWorldsHead>();
                            gNPC.AttackTimer = 0;
                            if (WorldSavingSystem.MasochistModeReal)
                                gNPC.AttackTimer += 60;
                            gNPC.UTurnTotalSpacingDistance = UTurnTotalSpacingDistance;
                            gNPC.UTurnIndividualSpacingPosition = headCounter;

                            Main.npc[i].netUpdate = true;
                            NetSync(Main.npc[i]);

                            headCounter *= -1; //alternate 0, 1, -1, 2, -2, 3, -3, etc.
                            if (headCounter >= 0)
                                headCounter++;
                        }
                    }
                }
                Timer++;
            }
            if (++AttackTimer < 120)
            {
                Vector2 target = Target.Center;
                if (UTurnTotalSpacingDistance != 0)
                    target.X += 900 / UTurnTotalSpacingDistance * UTurnIndividualSpacingPosition; //space out
                target.Y += 600f;

                float speedModifier = 0.6f;
                float speedCap = 24;
                if (NPC.Top.Y > Target.Bottom.Y + NPC.height)
                {
                    speedModifier *= 1.5f;
                    speedCap *= 1.5f;
                    NPC.position += (Target.position - Target.oldPosition) / 2;
                }

                if (NPC.Center.X < target.X)
                {
                    NPC.velocity.X += speedModifier;
                    if (NPC.velocity.X < 0)
                        NPC.velocity.X += speedModifier * 2;
                }
                else
                {
                    NPC.velocity.X -= speedModifier;
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X -= speedModifier * 2;
                }
                if (NPC.Center.Y < target.Y)
                {
                    NPC.velocity.Y += speedModifier;
                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y += speedModifier * 2;
                }
                else
                {
                    NPC.velocity.Y -= speedModifier;
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y -= speedModifier * 2;
                }

                if (Math.Abs(NPC.velocity.X) > speedCap)
                    NPC.velocity.X = speedCap * Math.Sign(NPC.velocity.X);
                if (Math.Abs(NPC.velocity.Y) > speedCap)
                    NPC.velocity.Y = speedCap * Math.Sign(NPC.velocity.Y);

                NPC.localAI[0] = 1f;

                if (Main.netMode == NetmodeID.Server && --NPC.netSpam < 0) //manual mp sync control
                {
                    NPC.netSpam = 5;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                }
            }
            else if (AttackTimer == 120) //fly up
            {
                if (firstEater)
                    SoundEngine.PlaySound(SoundID.Roar, Target.Center);
                NPC.velocity = Vector2.UnitY * -15f;
                UTurnStoredTargetX = (int)Target.Center.X; //store their initial location

                NPC.netUpdate = true;
            }
            else if (AttackTimer < 240) //cancel early and turn once we fly past player
            {
                if (NPC.Center.Y < Target.Center.Y - (WorldSavingSystem.MasochistModeReal ? 200 : 450))
                    AttackTimer = 239;
            }
            else if (AttackTimer == 240) //recalculate velocity to u-turn and dive back down in the same spacing over player
            {
                Vector2 target;
                target.X = Target.Center.X;
                if (UTurnTotalSpacingDistance != 0)
                    target.X += 900f / UTurnTotalSpacingDistance * UTurnIndividualSpacingPosition; //space out
                target.Y = NPC.Center.Y;

                float radius = Math.Abs(target.X - NPC.Center.X) / 2;
                float speed = MathHelper.Pi * radius / 30;
                if (speed < 8f)
                    speed = 8f;
                NPC.velocity = Vector2.Normalize(NPC.velocity) * speed;

                UTurnStoredTargetX = Math.Sign(Target.Center.X - UTurnStoredTargetX); //which side player moved to from original pos

                NPC.netUpdate = true;
            }
            else if (AttackTimer < 270) //u-turn
            {
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(6f) * UTurnStoredTargetX);
            }
            else if (AttackTimer == 270)
            {
                NPC.velocity = Vector2.Normalize(NPC.velocity) * 15f;
                NPC.netUpdate = true;
            }
            else if (AttackTimer > 300)
            {
                if (firstEater)
                {
                    EndAttack();
                    return;
                }
                /*
                SpecialAITimer = 0;
                SpecialCountdownTimer = 0;
                UTurnTotalSpacingDistance = 0;
                UTurnIndividualSpacingPosition = 0;
                Attack = (int)Attacks.Normal;
                */
                //for (int i = 0; i < Main.maxNPCs; i++)
                //{
                //    if (Main.npc[i].active)
                //    {
                //        if (Main.npc[i].type == npc.type)
                //        {
                //            Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurnTotalSpacingDistance = 0;
                //            Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurnIndividualSpacingPosition = 0;
                //            Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurn = false;
                //            Main.npc[i].netUpdate = true;
                //            if (Main.netMode == NetmodeID.Server)
                //                NetSync(npc);
                //        }
                //        else if (Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail)
                //        {
                //            Main.npc[i].netUpdate = true;
                //        }
                //    }
                //}

                NPC.netUpdate = true;
            }

            //NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            if (NPC.netUpdate)
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                    NetSync(NPC);
                }
                NPC.netUpdate = false;
            }
        }
        /*
        {
            int dashCount = 4;

            float dashPrep = 75;
            float dashTime = 60;
            float cycleTime = dashPrep + dashTime;
            float cycleTimer = Timer % cycleTime;
            NPC mainHead = Main.npc[NPC.FindFirstNPC(NPCID.EaterofWorldsHead)];
            if (cycleTimer < dashPrep || AttackTimer == 0)
            {
                if (cycleTimer <= 1)
                    AttackTimer = 1; // newly created heads go to prep mode until prompted
                Vector2 target = Target.Center;
                Vector2 dir = mainHead.DirectionTo(Target.Center);
                float offset = 700 - 480 * (cycleTimer / dashPrep);
                target -= dir * offset;
                float targetOffset = MyIndex * (WorldSavingSystem.MasochistModeReal ? 150 : 180);
                targetOffset *= MyIndex % 2 == 0 ? 1 : -1;
                targetOffset *= (int)(Timer / cycleTime) % 2 == 0 ? 1 : -1;
                target += dir.RotatedBy(MathHelper.PiOver2) * targetOffset;

                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Lerp(NPC.Center, target, 0.06f) - NPC.Center, 0.06f);
            }
            else
            {
                if (cycleTimer == dashPrep + 1 && firstEater)
                {
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                }
                if (cycleTimer < dashPrep + 20)
                {
                    NPC.velocity *= 0.9f;
                    NPC.velocity += NPC.velocity.SafeNormalize(Vector2.UnitX) * 2.8f;
                }
                else if (cycleTimer < dashPrep + 40)
                {
                    
                }
                else
                {
                    NPC.velocity *= 0.94f;
                }
            }

            if (firstEater)
            {
                Timer++;
                if (Timer >= cycleTime * dashCount - 2)
                {
                    EndAttack();
                    return;
                }
            }
        }
        */
        public void Fireballs()
        {
            float attackDuration = 60 * 9;



            if (Timer > attackDuration - 60)
            {
                NPC.velocity.X -= NPC.HorizontalDirectionTo(Target.Center) * 0.4f;
                NPC.velocity.Y += 0.12f;
                
            }
            else
            {
                float dashStart = attackDuration / 2 + 40;
                float dashWindup = dashStart - 30;
                float dashEnd = dashStart + 30;
                float dashEndlag = dashEnd + 20;
                if (Timer >= dashWindup && Timer < dashStart)
                {
                    if (Timer == dashWindup && firstEater)
                        SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                    NPC.velocity *= 0.94f;
                }
                else if (Timer >= dashStart && Timer < dashEnd)
                {
                    NPC.velocity += NPC.DirectionTo(Target.Center) * 0.8f;
                }
                else if (Timer >= dashEnd && Timer < dashEndlag)
                {
                    NPC.velocity *= 0.97f;
                }
                else
                {
                    Vector2 target = Target.Center;
                    Vector2 dir = NPC.DirectionTo(Target.Center);
                    float targetOffset = MyIndex - (HeadCount / 2f);
                    float sineTime = Timer + targetOffset * 30;
                    targetOffset = 160 * MathF.Sin(sineTime * MathF.Tau / 150f);
                    target += dir.RotatedBy(MathHelper.PiOver2) * targetOffset;
                    float speed = 0.5f + (0.02f * (MyIndex - (HeadCount / 2f)));
                    float turn = Timer >= dashEndlag ? 1.2f : 0.53f;
                    if (NPC.Distance(target) > 250)
                        Movement(target, speed, 0.4f, 1f, turn);

                    //AttackTimer++;
                    int x = WorldSavingSystem.MasochistModeReal ? 9 : 12;
                    if (Timer >= dashEndlag)
                        x -= 4;
                    int freq = x + HeadCount * x;
                    int offset = MyIndex * freq / HeadCount;
                    float telegraph = 25;
                    dir = NPC.velocity.SafeNormalize(dir);
                    float shotSpeed = 12;
                    if (WorldSavingSystem.MasochistModeReal)
                        shotSpeed = 15;
                    if (Timer % freq < offset && Timer % freq > ((offset - telegraph) % freq))
                    {
                        int d = Dust.NewDust(NPC.Center, 1, 1, DustID.CursedTorch);
                        Main.dust[d].velocity = dir.RotatedByRandom(MathHelper.PiOver2 * 0.1f) * Main.rand.NextFloat(shotSpeed - 2, shotSpeed + 2);
                    }
                    if (Timer % freq == offset && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * shotSpeed,
                                ProjectileID.CursedFlameHostile, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                        }
                    }
                }
            }



            if (firstEater)
            {
                Timer++;
                if (Timer > attackDuration)
                {
                    EndAttack();
                    return;
                }
            }
        }
        #region Help Methods
        public void EndAttack()
        {
            Timer = 0;
            Attack = (int)Attacks.DefaultMovement;
            NPC.netUpdate = true;
        }
        void Movement(Vector2 target, float speedMultiplier = 1f, float accelMultiplier = 1f, float decelMultiplier = 1f, float turnMultiplier = 1f)
        {
            float defaultSpeed = 16f * speedMultiplier;
            float accel = 0.3f * accelMultiplier;
            float decel = 0.3f * decelMultiplier;
            Vector2 desiredDir = NPC.DirectionTo(target);
            float diff = FargoSoulsUtil.RotationDifference(NPC.velocity, desiredDir);
            defaultSpeed = MathHelper.Lerp(defaultSpeed, 0, Math.Abs(diff) / MathHelper.Pi);
            if (NPC.velocity == Vector2.Zero)
                NPC.velocity = desiredDir * 0.01f;

            float speed = NPC.velocity.Length();
            if (speed < defaultSpeed)
            {
                speed += accel;
                if (speed > defaultSpeed)
                    speed = defaultSpeed;
            }
            else if (speed > defaultSpeed)
            {
                speed -= decel;
                if (speed < defaultSpeed)
                    speed = defaultSpeed;
            }

            NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitX) * speed;

            NPC.velocity = NPC.velocity.RotateTowards(desiredDir.ToRotation(), 0.05f * turnMultiplier);

            //NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, target, NPC.velocity, accel - resistance, decel + resistance);
        }
        public int CountSegments()
        {
            int c = 0;
            NPC me = NPC;
            for (int safety = 0; safety < Main.maxNPCs; safety++)
            {
                int ai0 = (int)me.ai[0];
                if (me.type == NPCID.EaterofWorldsTail || !ai0.IsWithinBounds(Main.maxNPCs))
                    return c;
                me = Main.npc[ai0];
                c++;
            }
            return c;
        }
        public NPC? GetTail()
        {
            int c = 0;
            NPC me = NPC;
            for (int safety = 0; safety < Main.maxNPCs; safety++)
            {
                int ai0 = (int)me.ai[0];
                if (me.type == NPCID.EaterofWorldsTail || !ai0.IsWithinBounds(Main.maxNPCs))
                    return me;
                me = Main.npc[ai0];
                c++;
            }
            return me;
        }
        public static Point FindGround(Point p)
        {
            if (p.X > 0 && p.Y > 0 && WorldGen.InWorld(p.X, p.Y, 2))
            {
                Point result = LumUtils.FindGroundVertical(p);
                if (result.X > 0 && result.Y > 0 && WorldGen.InWorld(result.X, result.Y, 2))
                    return result;
            }
            return p;
        }
        public static Vector2 UnevenGroundFix(Vector2 p, Player target)
        {
            for (int i = 0; i < 50; i++)
            {
                if (p.Y < target.Bottom.Y || Collision.CanHitLine(p, 1, 1, target.Center, 1, 1))
                    break;
                p.Y -= 16;
            }
            return p;
        }
        #endregion
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            LocalizedText DeathText = Language.GetText("Mods.FargowiltasSouls.DeathMessage.EOW");
            if (Main.getGoodWorld)
            {
                target.KillMe(PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(target.name)), 999999, 0);
            }
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadBossHeadSprite(recolor, 2);
            LoadGoreRange(recolor, 24, 29);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC == null || NPC.IsABestiaryIconDummy)
                return true;
            Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = new(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]);

            spriteBatch.Draw(bodytexture, drawPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }
    }

    public class EaterofWorldsSegment : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);
        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.damage *= 2;
        }
        public override bool SafePreAI(NPC npc)
        {
            //FargoSoulsUtil.PrintAI(npc);
            NPC head = null;
            int headID = (int)npc.ai[1];
            for (int i = 0; i < 200; i++)
            {
                if (headID.IsWithinBounds(Main.maxNPCs) && Main.npc[headID] is NPC ahead)
                {
                    if (ahead.type == NPCID.EaterofWorldsHead)
                    {
                        break;
                    }
                    headID = (int)ahead.ai[1];
                }
                else
                    break; // how
            }
            if (headID.IsWithinBounds(Main.maxNPCs))
                head = Main.npc[headID];
            if (head != null && head.Alive())
            {
                npc.timeLeft = 60 * 60;
                /*
                if (headEternity.Coiling && head.HasPlayerTarget)
                {
                    Player player = Main.player[head.target];
                    if (player.Distance(npc.Center) < EaterofWorldsHead.CoilRadius)
                    {
                        npc.Center = Vector2.Lerp(npc.Center, headEternity.CoilCenter + headEternity.CoilCenter.DirectionTo(npc.Center) * EaterofWorldsHead.CoilRadius, 0.75f);
                    }
                }
                */
            }
            return base.SafePreAI(npc);
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        public override bool CheckDead(NPC npc)
        {
            //no loot unless every other segment is dead (doesn't apply during swarms - if swarm, die and drop loot normally)
            if (!WorldSavingSystem.SwarmActive && Main.npc.Any(n => n.active && n.whoAmI != npc.whoAmI && (n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsTail)))
            {
                npc.active = false;
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                return false;
            }

            return base.CheckDead(npc);
        }
    }

    public class VileSpitEaterofWorlds : VileSpit
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.VileSpitEaterOfWorlds);

        public int SuicideCounter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.scale *= 2;

            if (WorldSavingSystem.MasochistModeReal)
                npc.dontTakeDamage = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++SuicideCounter > 600 || Main.npc.Any(n => n.TypeAlive(NPCID.EaterofWorldsHead) && n.TryGetGlobalNPC(out EaterofWorldsHead eowHead)))
                npc.SimpleStrikeNPC(int.MaxValue, 0, false, 0, null, false, 0, true);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            /*
            if (WorldSavingSystem.MasochistModeReal && Main.getGoodWorld && FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 8; i++)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitY.RotatedBy(2 * Math.PI / 8 * i) * 2f, ProjectileID.CorruptSpray, 0, 0f, Main.myPlayer, 8f);
            }
            */
        }
    }
}
