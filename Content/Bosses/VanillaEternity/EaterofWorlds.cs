using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Corruption;
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

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class EaterofWorlds : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);

        int MassDefenseTimer;
        bool UseMassDefense;

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.CursedInferno] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
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

            target.AddBuff(ModContent.BuffType<RottingBuff>(), 600);
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
        public NPC? NPC = null;
        public Player? Target => Main.player[NPC.target];
        public bool DroppedSummon;
        public static int HaveSpawnDR;
        public int NoSelfDestructTimer = 15;
        public static int SpecialCountdownTimer;

        public int SpawnTimer = 0;

        // attack stuff
        public int Attack;
        public int OldAttack;

        public int Timer;

        public static int UndergroundLength => 400;
        public int ExtraAI0;

        public static int ComboTimer = 0;
        public static float ComboHP => 0.8f;

        public bool Telegraph = false;
        public bool ContactDamage = true;

        public enum Attacks
        {
            DefaultMovement,
            BurrowSpit,
            BurrowBelow,
            BurrowSide,
            Combo
        }
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write(ExtraAI0);
            binaryWriter.Write(Attack);
            binaryWriter.Write(OldAttack);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            ExtraAI0 = binaryReader.ReadInt32();
            Attack = binaryReader.ReadInt32();
            OldAttack = binaryReader.ReadInt32();
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
            if (Telegraph || !ContactDamage)
                return false;
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.eaterBoss = npc.whoAmI;
            FargoSoulsGlobalNPC.boss = npc.whoAmI;

            if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && npc.Distance(Main.LocalPlayer.Center) < 2000)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);
            }

            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 6000)
            {
                // check for other eater heads. if all eater heads have no target or are out of range...
                if (Main.npc.All(otherNpc => !(otherNpc.active && otherNpc.type == npc.type && (!otherNpc.HasValidTarget || otherNpc.Distance(Main.player[otherNpc.target].Center) > 6000))))
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

            if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) > 2500)
            {
                Vector2 dir = npc.DirectionTo(Main.player[npc.target].Center);
                if (npc.velocity.Length() < 25)
                    npc.velocity += dir * 1f;
                npc.velocity = npc.velocity.RotateTowards(dir.ToRotation(), 0.1f);
            }

            //if (eaterResist > 0 && npc.whoAmI == NPC.FindFirstNPC(npc.type)) eaterResist--;

            int firstEater = NPC.FindFirstNPC(npc.type);

            if (npc.whoAmI == firstEater)
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

                if (fraction < ComboHP && NPC.CountNPCS(NPCID.EaterofWorldsHead) > 3)
                {
                    if (ComboTimer == 0)
                    {
                        ComboTimer = -1;
                    }
                    else if (ComboTimer > 0)
                        ComboTimer--;
                }
                else
                {
                    ComboTimer = 0;
                }
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
            if (Telegraph)
            {
                if (Timer % 5 == 0)
                    SoundEngine.PlaySound(SoundID.WormDig with { Pitch = -0.2f }, NPC.Center);
                Vector2 vel = NPC.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.27f) * Main.rand.NextFloat(0.4f, 0.6f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CorruptionThorns, vel.X, vel.Y);

                Lighting.AddLight(NPC.Center - Vector2.UnitY * 16, TorchID.Corrupt);
            }
            Telegraph = false;
            ContactDamage = true;

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
                case Attacks.BurrowSpit:
                    BurrowSpit();
                    break;

                case Attacks.BurrowBelow:
                    BurrowBelow();
                    break;

                case Attacks.BurrowSide:
                    BurrowSide();
                    break;

                case Attacks.Combo:
                    ComboAttack();
                    break;

                default:
                    ret = true;
                    break;
            }

            //drop summon
            //if (npc.HasPlayerTarget && !DroppedSummon)
            //{
            //    Player player = Main.player[npc.target];

            //    //eater meme
            //    if (!player.dead && player.FargoSouls().FreeEaterSummon)
            //    {
            //        player.FargoSouls().FreeEaterSummon = false;

            //        if (!NPC.downedBoss2 && FargoSoulsUtil.HostCheck && ModContent.TryFind("Fargowiltas", "WormyFood", out ModItem modItem))
            //            Item.NewItem(npc.GetSource_Loot(), player.Hitbox, modItem.Type);

            //        DroppedSummon = true;
            //        SpecialCountdownTimer = 0;
            //        HaveSpawnDR = 180;
            //        npc.velocity.Y += 6;
            //    }
            //}
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
            int maxDistX = 400;
            targetPos.X = MathHelper.Clamp(targetPos.X, Target.Center.X - maxDistX, Target.Center.X + maxDistX);
            targetPos.X += Math.Abs(Target.Center.X - NPC.Center.X) * 0.4f; // inch slightly towards player
            targetPos.Y += UndergroundLength;
            Movement(targetPos, 1f);

            Timer++;
            int waitTime;
            int heads = CountHeads();
            if (WorldSavingSystem.MasochistModeReal)
            {
                if (heads > 3)
                    waitTime = 70 + (heads - 3) * 3;
                else if (heads == 3)
                    waitTime = 70;
                else if (heads == 2)
                    waitTime = 50;
                else
                    waitTime = 35;
            }
            else
            {
                if (heads > 3)
                    waitTime = 90 + (heads - 3) * 6;
                else if (heads == 3)
                    waitTime = 90;
                else if (heads == 2)
                    waitTime = 75;
                else
                    waitTime = 60;
            }
            if (Timer > waitTime && (heads < 3 || Main.rand.NextBool(10)))
            {
                List<Attacks> attacks = [Attacks.BurrowSpit, Attacks.BurrowBelow, Attacks.BurrowSide];

                int segments = CountSegments();
                if (segments <= 5)
                    attacks.Remove(Attacks.BurrowSpit);

                if (attacks.Count > 2)
                    attacks.Remove((Attacks)OldAttack);

                Attack = (int)Main.rand.NextFromCollection(attacks);

                if (ComboTimer == -1)
                {
                    Attack = (int)Attacks.Combo;
                }
                OldAttack = Attack;
                NPC.netUpdate = true;
                Timer = 0;
                return;
            }
        }
        public void BurrowSpit()
        {
            int windup = 20;

            Vector2 targetPos = Target.Center + Vector2.UnitX * Target.HorizontalDirectionTo(NPC.Center) * 450 - Vector2.UnitY * 400;
            targetPos = FindGround(targetPos.ToTileCoordinates()).ToWorldCoordinates();
            targetPos = UnevenGroundFix(targetPos, Target);

            if (Timer >= 0) // first part
            {
                Timer++;


                targetPos.Y += UndergroundLength;
                float xDif = targetPos.X - NPC.Center.X;

                Movement(targetPos, 2.5f);
                if (Timer > windup && Math.Abs(xDif) < 80)
                {
                    Timer = -1;
                    NPC.velocity *= 0.5f;
                }
            }
            if (Timer < 0) // second part
            {
                Timer--;

                bool collision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
                collision = collision || NPC.Center.Y > targetPos.Y || (!Collision.CanHitLine(NPC.Center, 1, 1, Target.Center, 1, 1) && NPC.Center.Y > Target.Center.Y);

                if (collision && Timer > -50)
                {
                    NPC.velocity.X *= 0.96f;
                    const float accelUp = 1f;
                    if (NPC.velocity.Y > -13)
                    {
                        NPC.velocity.Y -= accelUp;
                    }
                }
                else
                {
                    if (Timer > -1000)
                    {
                        Timer = -1000;
                    }
                    NPC.velocity *= 0.967f;

                    if (Timer > -1000 - 12 && Timer < -1000 - 1)
                    {
                        Vector2 dir = -Vector2.UnitX * NPC.HorizontalDirectionTo(Target.Center) + Vector2.UnitY * 0.35f;
                        NPC.velocity = NPC.velocity.RotateTowards(dir.ToRotation(), 0.1f);
                    }

                    if (Timer > -1000 - 40 && Timer < -1000 - 15)
                    {
                        Vector2 dir = Vector2.UnitX * NPC.HorizontalDirectionTo(Target.Center) + Vector2.UnitY * 0.35f;
                        NPC.velocity = NPC.velocity.RotateTowards(dir.ToRotation(), 0.13f);
                    }
                    if (Timer < -1000 - 40)
                        NPC.velocity = NPC.velocity.RotateTowards(NPC.DirectionTo(Target.Center).ToRotation(), 0.014f);

                    int frequency;
                    int heads = CountHeads();
                    if (heads > 4)
                        heads = 4;
                    if (heads <= 1)
                        frequency = 20;
                    else if (heads == 2)
                        frequency = 30;
                    else
                        frequency = 20 + heads * 5;
                    if (Timer < -1000 - frequency && Timer % frequency == 0)
                    {
                        // fireball
                        Vector2 dir = (NPC.rotation - MathHelper.PiOver2).ToRotationVector2();

                        if (heads == 1)
                            dir = dir.RotatedByRandom(MathHelper.PiOver2 * 0.22f);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * 10,
                                ProjectileID.CursedFlameHostile, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                        }
                    }

                    if (Timer < -1000 - 140)
                    {
                        EndAttack();
                        return;
                    }
                }
            }
        }
        public void BurrowBelow()
        {
            int windup = 40;
            Vector2 targetPos = new(Target.Center.X + Target.velocity.X * 80, Target.Center.Y - 400);
            targetPos = FindGround(targetPos.ToTileCoordinates()).ToWorldCoordinates();
            targetPos = UnevenGroundFix(targetPos, Target);
            if (Timer >= 0) // first part
            {

                Timer++;


                float xDif = targetPos.X - NPC.Center.X;


                targetPos.Y += UndergroundLength;
                Movement(targetPos, 2.5f);
                if (Math.Abs(xDif) < 5)
                {
                    Timer = -1;
                    NPC.velocity *= 0f;
                }
            }
            if (Timer < 0) // second part
            {
                Timer--;
                NPC.velocity.X *= 0.98f;
                NPC.velocity.X += NPC.HorizontalDirectionTo(Target.Center) * 0.025f;

                if (Timer > -28 || (Timer > -50 && NPC.Center.Y > targetPos.Y))
                {
                    ContactDamage = false;
                    if (Timer % 2 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                    }
                    const float accelUp = 0.8f;
                    if (NPC.velocity.Y > -25)
                    {
                        NPC.velocity.Y -= accelUp;
                    }
                }
                else
                {
                    if (Timer > -1000)
                    {
                        Timer = -1000;
                        NPC.position -= NPC.velocity / 2;
                        NPC.velocity *= 0.75f;
                        Telegraph = true;
                    }
                    if (Timer <= -1000 && Timer >= -1000 - windup) // windup
                    {
                        NPC.position -= NPC.velocity;
                        Telegraph = true;
                    }
                    else // post windup
                    {
                        if (Timer == -1000 - windup - 2)
                        {
                            int heads = CountHeads();
                            if (heads > 4)
                                heads = 4;
                            if (WorldSavingSystem.MasochistModeReal && heads > 1)
                                heads--;
                            if (heads <= 4)
                            {
                                int projCount;
                                float width;
                                if (heads == 1)
                                {
                                    projCount = 6;
                                    width = 0.32f;
                                }
                                else if (heads == 2)
                                {
                                    projCount = 4;
                                    width = 0.18f;
                                }
                                else if (heads == 3)
                                {
                                    projCount = 2;
                                    width = 0.13f;
                                }
                                else
                                {
                                    projCount = 0;
                                    width = 0.13f;
                                }
                                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    for (float i = 0; i < projCount; i++)
                                    {
                                        float offset = (i - (projCount / 2)) / (projCount / 2); // from -1 to 1
                                        Vector2 angle = -Vector2.UnitY.RotatedBy((offset + Main.rand.NextFloat(-0.12f, 0.12f)) * MathHelper.PiOver2 * width);

                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, angle * Main.rand.NextFloat(14, 18),
                                            ModContent.ProjectileType<CorruptionSludge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                    }
                                }

                            }
                        }

                        NPC.velocity.Y += 0.3f;
                        NPC.velocity.X += NPC.HorizontalDirectionTo(Target.Center) * 0.1f;
                        bool collision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
                        collision = collision || NPC.Center.Y > Target.Center.Y;
                        if (collision && Timer < -1000 - windup - 50)
                        {
                            EndAttack();
                            return;
                        }
                    }

                }
            }
        }
        public void BurrowSide()
        {
            int windup = 40;
            Vector2 targetPos = Target.Center + Vector2.UnitX * Target.HorizontalDirectionTo(NPC.Center) * (880 + ExtraAI0) - Vector2.UnitY * 400;
            targetPos = FindGround(targetPos.ToTileCoordinates()).ToWorldCoordinates();
            targetPos = UnevenGroundFix(targetPos, Target);

            if (Timer >= 0) // first part
            {
                if (Timer == 0)
                {
                    ExtraAI0 = Main.rand.Next(-100, 100);
                }
                Timer++;

                //float dir = -Target.HorizontalDirectionTo(targetPos);

                //Telegraph = true;
                //Vector2 telegPos = targetPos + dir * Vector2.UnitX * 200;
                //TelegraphPosition = telegPos;
                //TelegraphRotation = new Vector2(dir * 4, -6).ToRotation();


                targetPos.Y += UndergroundLength;

                float xDif = targetPos.X - NPC.Center.X;

                Movement(targetPos, 2.5f);
                if (Math.Abs(xDif) < 5)
                {
                    Timer = -1;
                    NPC.velocity *= 0f;
                }
            }
            if (Timer < 0) // second part
            {
                Timer--;


                if (Timer > -24 || (Timer > -50 && NPC.Center.Y > targetPos.Y))
                {
                    ContactDamage = false;
                    const float accelUp = 0.4f;
                    if (NPC.velocity.Y > -25)
                    {
                        float side = 0.4f;
                        float mult = 1f;
                        if (CountSegments() < 10)
                        {
                            mult = 2f;
                        }
                            
                        NPC.velocity.X += NPC.HorizontalDirectionTo(Target.Center) * accelUp * side * mult;
                        NPC.velocity.Y -= accelUp * mult;
                    }
                }
                else
                {
                    if (Timer > -1000)
                    {
                        Timer = -1000;
                        NPC.position -= NPC.velocity / 2;
                        Telegraph = true;
                    }
                    if (Timer <= -1000 && Timer >= -1000 - windup) // windup
                    {
                        NPC.position -= NPC.velocity;
                        Telegraph = true;
                    }
                    else // post windup
                    {
                        if (Timer == -1000 - windup - 3)
                        {
                            int heads = CountHeads();
                            if (heads > 4)
                                heads = 4;
                            if (WorldSavingSystem.MasochistModeReal && heads > 1)
                                heads--;
                            if (heads <= 4)
                            {
                                int projCount;
                                float width;
                                if (heads == 1)
                                {
                                    projCount = 8;
                                    width = 0.4f;
                                }
                                else if (heads == 2)
                                {
                                    projCount = 6;
                                    width = 0.3f;
                                }
                                else if (heads == 3)
                                {
                                    projCount = 4;
                                    width = 0.2f;
                                }
                                else
                                {
                                    projCount = 2;
                                    width = 0.12f;
                                }
                                    SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    for (float i = 0; i < projCount; i++)
                                    {
                                        float offset = (i - (projCount / 2)) / (projCount / 2); // from -1 to 1
                                        Vector2 angle = NPC.velocity.RotatedBy((offset + Main.rand.NextFloat(-0.12f, 0.12f)) * MathHelper.PiOver2 * width).SafeNormalize(-Vector2.UnitY);

                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, angle * Main.rand.NextFloat(16, 22),
                                            ModContent.ProjectileType<CorruptionSludge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        NPC.velocity.Y += 0.5f;
                        bool collision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
                        collision = collision || NPC.Center.Y > Target.Center.Y;
                        if (collision && Timer < -1000 - 50)
                        {
                            EndAttack();
                            return;
                        }
                    }
                }
            }
        }
        public void ComboAttack()
        {
            int windup = 0;

            int heads = 0;
            bool foundyou = false;
            int myID = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i == NPC.whoAmI)
                    foundyou = true;
                if (Main.npc[i].type == NPCID.EaterofWorldsHead)
                {
                    heads++;
                    if (!foundyou)
                        myID++;
                }
                    
            }
            int offsetIndex = myID - (heads / 2);
            // no 0 allowed
            if (offsetIndex == 0)
                offsetIndex = 1;
            int side = Math.Sign(offsetIndex);


            Vector2 targetPos = new(Target.Center.X + 450 * side + 60 * offsetIndex, Target.Center.Y);

            if (Timer >= 0) // first part
            {
                Timer++;

                // wait for all heads to sync
                if (Timer > 1)
                {
                    if (Main.npc.Any(n => n.TypeAlive(NPCID.EaterofWorldsHead) && n.GetGlobalNPC<EaterofWorldsHead>().Attack != (int)Attacks.Combo))
                    {
                        Timer = 1;
                    }
                }

                float xDif = targetPos.X - NPC.Center.X;

                targetPos.Y += UndergroundLength;
                Movement(targetPos, 4f);
                if (Timer >= 5 && Math.Abs(xDif) < 120)
                {
                    Timer = -1;
                    NPC.velocity *= 0f;
                }
            }
            if (Timer < 0) // second part
            {
                Timer--;

                NPC.velocity.X *= 0.98f;
                NPC.velocity.X += NPC.HorizontalDirectionTo(Target.Center) * 0.025f;

                if (Timer > -28 || (Timer > -50 && NPC.Center.Y > targetPos.Y))
                {
                    ContactDamage = false;
                    if (Timer % 2 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                    }
                    const float accelUp = 0.8f;
                    if (NPC.velocity.Y > -19)
                    {
                        NPC.velocity.Y -= accelUp;
                    }
                }
                else
                {
                    if (Timer > -1000 && NPC.velocity.Y > -5)
                    {
                        Timer = -1000;
                        ContactDamage = false;
                    }
                    if (Timer <= -1000 && Timer >= -1000 - windup) // windup
                    {
                        NPC.position -= NPC.velocity;
                        Telegraph = true;
                        ContactDamage = false;
                    }
                    else // post windup
                    {
                        int windupTime = 25;
                        int chargeTime = 35;
                        int endTime = 17;
                        if ((Timer <= -1000 && Timer >= -1000 - windupTime) || (Timer <= -1000 - windupTime - chargeTime && Timer >= -1000 - windupTime * 2 - chargeTime))
                        {
                            if (Timer >= -1000 - windupTime)
                                ContactDamage = false;
                            Vector2 dir = NPC.DirectionTo(Target.Center);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, dir * 3, 0.2f);
                            if (Timer == -1000 - windupTime || Timer == -1000 - windupTime - chargeTime - windupTime)
                            {
                                NPC.velocity = dir * 17;
                                SoundEngine.PlaySound(SoundID.ForceRoarPitched with { Volume = 0.25f }, NPC.Center);
                            }
                        }
                        else if ((Timer <= -1000 - windupTime && Timer >= -1000 - windupTime - chargeTime) || (Timer <= -1000 - windupTime * 2 - chargeTime && Timer >= -1000 - (windupTime + chargeTime) * 2))
                        {
                            NPC.velocity = (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 17;
                        }
                        else
                        {
                            if (NPC.velocity.Y < 12)
                                NPC.velocity.Y += 0.5f;
                        }
                            

                        bool collision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
                        collision = collision || NPC.Center.Y > Target.Center.Y;
                        if (collision && Timer < -1000 - windupTime * 2 - chargeTime * 2 - endTime)
                        {
                            ComboTimer = 60 * 26;
                            EndAttack();
                            return;
                        }
                    }
                }
            }
        }
        #region Help Methods
        public void EndAttack()
        {
            Timer = 0;
            Attack = (int)Attacks.DefaultMovement;
        }
        void Movement(Vector2 target, float speedMultiplier = 1f)
        {
            float accel = 0.4f * speedMultiplier;
            float decel = 0.7f * speedMultiplier;
            float resistance = NPC.velocity.Length() * accel / (35f * speedMultiplier);
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, target, NPC.velocity, accel - resistance, decel + resistance);
        }
        public int CountHeads()
        {
            return NPC.CountNPCS(NPCID.EaterofWorldsHead);
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

            if (Main.getGoodWorld)
            {
                target.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.EOW", target.name)), 999999, 0);
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

            if (Telegraph)
            {
                drawPos += Main.rand.NextVector2Circular(12, 6);
            }

            spriteBatch.Draw(bodytexture, drawPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }
    }

    public class EaterofWorldsSegment : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);
        public bool ContactDamage = true;
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
                ContactDamage = head.GetGlobalNPC<EaterofWorldsHead>().ContactDamage;
                npc.timeLeft = 60 * 60;
                EaterofWorldsHead headEternity = head.GetGlobalNPC<EaterofWorldsHead>();
                if (head.HasPlayerTarget)
                {
                    Player player = Main.player[head.target];
                    if (Collision.SolidCollision(head.position, head.width, head.height) && !Collision.SolidCollision(npc.Top - Vector2.UnitY * 12, 1, 1))
                        npc.position.Y += 4;
                }
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
            if (!ContactDamage)
                return false;
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

            if (WorldSavingSystem.MasochistModeReal && Main.getGoodWorld && FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 8; i++)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitY.RotatedBy(2 * Math.PI / 8 * i) * 2f, ProjectileID.CorruptSpray, 0, 0f, Main.myPlayer, 8f);
            }
        }
    }
}
