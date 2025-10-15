using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses.EmpressOfLight;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class EmpressofLight : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.HallowBoss);

        public int AttackTimer;
        public int AttackCounter;
        public int P2SwordsAttackCounter;
        public int DashCounter;

        public bool DroppedSummon;

        private float startRotation;
        public Vector2 targetPos;

        private static int SwordWallCap => WorldSavingSystem.MasochistModeReal ? 4 : 3;
        public bool DoParallelSwordWalls => P2SwordsAttackCounter % SwordWallCap > 0;
        public bool Ritual = false;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write7BitEncodedInt(AttackCounter);
            binaryWriter.Write7BitEncodedInt(P2SwordsAttackCounter);
            binaryWriter.Write7BitEncodedInt(DashCounter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
            AttackCounter = binaryReader.Read7BitEncodedInt();
            P2SwordsAttackCounter = binaryReader.Read7BitEncodedInt();
            DashCounter = binaryReader.Read7BitEncodedInt();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.3, MidpointRounding.ToEven);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            //only have contact damage when dashing
            if ((npc.ai[0] == 8 || npc.ai[0] == 9) && npc.ai[1] >= 40)
                return base.CanHitPlayer(npc, target, ref CooldownSlot);

            return false;
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.empressBoss = npc.whoAmI;
            Ritual = false;

            if (Main.LocalPlayer.active && Main.dayTime && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                Main.LocalPlayer.FargoSouls().noDodge = true;

            bool useP2Attacks = npc.ai[3] != 0 || WorldSavingSystem.MasochistModeReal;
            switch ((int)npc.ai[0])
            {
                //0 spawn

                case 1: //move over player
                    if (npc.ai[1] == 0)
                    {
                        AttackTimer = 0;
                        AttackCounter++;
                        NetSync(npc);
                    }
                    break;

                case 2: //homing bolts, ends at ai1=130
                    if (useP2Attacks && npc.ai[1] > 80 && !WorldSavingSystem.MasochistModeReal)
                        npc.ai[1] -= 0.5f; //p2, more delay before next attack
                    break;

                case 4: //pseudorandom swords following you, ends at ai1=100
                    {
                        if (npc.ai[1] == 0)
                        {
                            AttackTimer = 0;
                            NetSync(npc);

                            TryRandom(npc);
                        }

                        const int specialAttackThreshold = 97;
                        if (npc.ai[1] > specialAttackThreshold && useP2Attacks) //p2 only sword circle
                        {
                            SwordCircle(npc, specialAttackThreshold);
                        }
                    }
                    break;

                case 5: //stupid long trail circle ring
                    if (AttackTimer < 2)
                    {
                        if (npc.ai[1] > 1f)
                            npc.ai[1] -= 0.5f; //progress slower

                        if (npc.ai[1] == 30) //repeat attack
                        {
                            npc.ai[1] = 0f;
                            AttackTimer++;
                        }
                    }

                    //p2, do sword rings
                    if (useP2Attacks && AttackTimer >= 2)
                    {
                        if (npc.ai[1] == 20)
                            startRotation = Main.rand.NextFloat(MathHelper.TwoPi);

                        if (npc.ai[1] >= (WorldSavingSystem.MasochistModeReal ? 20 : 35) && npc.ai[1] <= 60)
                        {
                            npc.position -= npc.velocity;
                            npc.velocity = Vector2.Zero;

                            if (npc.ai[1] % 5 == 0)
                            {
                                for (float i = 0; i < 1; i += 1f / 24f)
                                {
                                    Vector2 spinningpoint = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 + MathHelper.TwoPi * i + startRotation);
                                    if (FargoSoulsUtil.HostCheck)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + spinningpoint.RotatedBy(-MathHelper.PiOver2) * 30f, Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.3f), 0f, Main.myPlayer, spinningpoint.ToRotation(), i);
                                }
                            }
                        }
                    }
                    break;

                case 6: //sun rays
                    if (!WorldSavingSystem.MasochistModeReal)
                    {
                        if (npc.ai[1] == 0 && AttackTimer == 0 && Main.projectile.Count(p => p.active && p.type == ProjectileID.HallowBossRainbowStreak) > 20)
                        {
                            npc.ai[1] -= 30;
                            npc.netUpdate = true;
                        }

                        if (npc.ai[1] == 1)
                            NetSync(npc);

                        npc.position -= npc.velocity / (npc.ai[3] == 0 ? 2 : 4); //move slower
                    }
                    if (npc.ai[1] < 90)
                        PullNonTargets(npc, npc.Center, 600);
                    break;

                case 7: //sword walls, ends at ai1=260
                    {
                        int start = WorldSavingSystem.MasochistModeReal ? -15 : -45;

                        if (npc.ai[1] == 0)
                        {
                            TryRandom(npc);

                            npc.netUpdate = true;
                            NetSync(npc); //sync it in advance to prepare for actual attacks
                        }

                        const int specialAttackThreshold = 255;
                        if (npc.ai[1] == specialAttackThreshold)
                        {
                            AttackTimer = start;
                            P2SwordsAttackCounter++;
                            startRotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        }

                        if (npc.ai[1] > specialAttackThreshold)
                        {
                            if (DoParallelSwordWalls)
                                ParallelSwordWalls(npc, specialAttackThreshold);
                            else //excel spreadsheet
                                ExcelSpreadsheet(npc, specialAttackThreshold);
                        }
                    }
                    break;

                case 8: //dash from right to left
                case 9: //dash from left to right
                    Dash(npc, useP2Attacks);
                    break;

                case 10: //p2 transition
                    if (npc.dontTakeDamage && npc.ai[1] > 120)
                    {
                        Vector2 target = Main.player[npc.target].Center - 160f * Vector2.UnitY;
                        npc.Center = Vector2.Lerp(npc.Center, target, 0.1f);

                        if (npc.life < npc.lifeMax / 2)
                        {
                            npc.HealEffect(npc.lifeMax / 2 - npc.life);
                            npc.life = npc.lifeMax / 2;
                        }
                    }
                    break;

                case 11: //p2 direct sword trail
                    if ((WorldSavingSystem.MasochistModeReal || npc.ai[1] > 40) && npc.ai[1] % 3 == 0 && npc.HasValidTarget) //add perpendicular swords
                    {
                        Vector2 offset = Main.player[npc.target].velocity;
                        if (offset == Vector2.Zero || offset.Length() < 1)
                            offset = offset.SafeNormalize(-Vector2.UnitY);
                        offset = 90f * offset.RotatedBy(MathHelper.PiOver2);

                        Vector2 spawnPos = Main.player[npc.target].Center + offset;
                        Vector2 vel = Main.player[npc.target].DirectionFrom(spawnPos);

                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.3f), 0f, Main.myPlayer, vel.ToRotation(), npc.ai[1] / 100f);
                    }
                    break;

                case 12: //8-way homing bolts
                    {
                        const int max = 24;
                        const int delay = 75;

                        if (npc.ai[1] < 4)
                        {
                            Vector2 target = Main.player[npc.target].Center - 160f * Vector2.UnitY;
                            npc.Center = Vector2.Lerp(npc.Center, target, 0.1f);
                            if (npc.Distance(target) > 160)
                                npc.ai[1] -= 1f;
                        }

                        if (npc.ai[1] == delay)
                            startRotation = npc.HasValidTarget ? npc.SafeDirectionTo(Main.player[npc.target].Center).ToRotation() : MathHelper.PiOver2;

                        if (npc.ai[1] >= delay && npc.ai[1] < delay + max)
                        {
                            if (FargoSoulsUtil.HostCheck)
                            {
                                float ai1 = (npc.ai[1] - delay) / max;

                                if (WorldSavingSystem.MasochistModeReal)
                                {
                                    float math = MathHelper.TwoPi / max * (npc.ai[1] - delay);
                                    Vector2 boltVel = -Vector2.UnitY.RotatedBy(-math);
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 20f * boltVel, ProjectileID.HallowBossRainbowStreak, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.3f), 0f, Main.myPlayer, npc.target, ai1);
                                }

                                float spread = MathHelper.ToRadians(24);
                                float swordRotation = startRotation + MathHelper.Lerp(-spread, spread, ai1);
                                Vector2 appearVel = swordRotation.ToRotationVector2();
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + appearVel * 160f + appearVel * 10f * 60f, -appearVel * 10f, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.3f), 0f, Main.myPlayer, swordRotation, ai1);
                            }
                        }

                        if (npc.ai[1] > delay + max && !WorldSavingSystem.MasochistModeReal)
                            npc.ai[1] -= 0.65f; //more delay before next attack
                    }
                    break;

                default:
                    break;
            }

            EModeUtils.DropSummon(npc, "PrismaticPrimrose", NPC.downedEmpressOfLight, ref DroppedSummon, Main.hardMode);

            return true;
        }

        #region helper funcs

        private static void TryRandom(NPC npc)
        {
            if (WorldSavingSystem.MasochistModeReal && npc.life < npc.lifeMax / 2) //RANDOM ATTACKS
            {
                npc.ai[2] += Main.rand.Next(3);
                npc.netUpdate = true;
            }
        }

        //defdamage multiplier because expert empress actually has 184 defdamage for some reason and not 110?????
        private static int BaseProjDmg(NPC npc) => Main.dayTime ? 9999 : (int)(npc.defDamage * 0.6);

        private void SwordCircle(NPC npc, float stop)
        {
            Ritual = true;
            int spinTime = WorldSavingSystem.MasochistModeReal ? 210 : 160;
            float spins = 1.5f;
            int startDelay = 60;
            if (AttackTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Item161, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center);

                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<EmpressRitual>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer, 0f, npc.whoAmI, ai2: spinTime * spins + startDelay * 2);
            }
            else if (AttackTimer == startDelay)
            {
                targetPos = Main.player[npc.target].Center;
                startRotation = npc.HasValidTarget ? Main.player[npc.target].velocity.ToRotation() : 0;
            }
            else if (AttackTimer < startDelay) // for ritual visual
            {
                targetPos = Main.player[npc.target].Center;
            }

            AttackTimer++;

            const float radius = 600;
            if (Main.player[npc.target].Distance(targetPos) > radius)
                targetPos = Main.player[npc.target].Center + Main.player[npc.target].SafeDirectionTo(targetPos) * radius;
            
            /*
            if (AttackTimer < startDelay + 30)
            {
                PullNonTargets(npc, targetPos, radius);
            }
            */

            if (AttackTimer % 90 == 30) //rapid fire sound effect
                SoundEngine.PlaySound(SoundID.Item164, Main.player[npc.target].Center);

            if (AttackTimer > startDelay && AttackTimer <= spinTime * spins + startDelay && AttackTimer % 2 == 0)
            {
                int max = 2;
                if (WorldSavingSystem.MasochistModeReal)
                {
                    max = 3;
                    if (Main.getGoodWorld)
                        max = 7;
                }
                for (int i = 0; i < max; i++)
                {
                    int direction = WorldSavingSystem.MasochistModeReal ? -1 : 1;
                    float increment = MathHelper.TwoPi / spinTime * AttackTimer * direction;
                    Vector2 offsetDirection = Vector2.UnitX.RotatedBy(startRotation + increment + MathHelper.TwoPi / max * i);
                    Vector2 spawnPos = targetPos + radius * offsetDirection;
                    Vector2 vel = Vector2.Normalize(targetPos - spawnPos);
                    float ai1 = (float)(AttackTimer - startDelay) / spinTime % 1;
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 appearVel = -vel;
                        appearVel *= WorldSavingSystem.MasochistModeReal ? 7.5f : 2.5f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos - appearVel * 60, appearVel, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, vel.ToRotation(), ai1);

                        /*
                        float angleOffset = MathHelper.ToRadians(45);
                        for (int j = -1; j <= 1; j++)
                        {
                            float length = 128f * (j + 2) + radius;
                            Vector2 newPos = targetPos + length * offsetDirection.RotatedBy(MathHelper.Pi / max / 3 * (2 - j) - increment * 2);
                            Vector2 newBaseVel = Vector2.Normalize(targetPos - newPos);
                            Vector2 fancyVel = 2.5f * (j + 2) * newBaseVel.RotatedBy(MathHelper.PiOver2 * j);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), newPos - fancyVel * 60f, fancyVel, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, newBaseVel.ToRotation() + MathHelper.Pi + angleOffset * j, ai1);
                        }
                        */
                    }
                }
            }

            if (!npc.HasValidTarget)
            {
                npc.TargetClosest(false);
                if (!npc.HasValidTarget)
                    AttackTimer += 9000;
            }

            if (AttackTimer < spinTime * spins + startDelay * 2)
                npc.ai[1] = stop; //stop vanilla ai from progressing
        }

        private void ParallelSwordWalls(NPC npc, float stop)
        {
            if (AttackTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Item161, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center);

                //rainbow trails to prevent running out of range
                for (float i = 0; i < 1; i += 1f / 13f)
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 + MathHelper.TwoPi * i + startRotation);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        void LastingRainbow(Vector2 vel, int timeLeft)
                        {
                            Vector2 pos = Main.player[npc.target].Center + spinningpoint.RotatedBy(-MathHelper.PiOver2) * 30f;
                            int p = Projectile.NewProjectile(npc.GetSource_FromThis(), pos, vel, ProjectileID.HallowBossLastingRainbow, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, 0f, i);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = timeLeft;
                        }

                        LastingRainbow(6f * spinningpoint, 500 + 120);
                        LastingRainbow(-7f * spinningpoint, 500 + 120);
                        LastingRainbow(8.5f * spinningpoint.RotatedBy(MathHelper.PiOver2), 500 + 120);
                    }
                }
            }

            const int delay = 15;
            int attackTime = WorldSavingSystem.MasochistModeReal ? 30 : 45;

            int effectiveTimer = AttackTimer - delay;
            if (effectiveTimer == -1)
            {
                targetPos = Main.player[npc.target].Center;
                startRotation += MathHelper.PiOver2 * Main.rand.NextFloat(0.9f, 1.1f);
            }
            if (effectiveTimer >= 0 && effectiveTimer <= attackTime)
            {
                const float coverage = 1200f;

                int interval = WorldSavingSystem.MasochistModeReal ? 1 : 2;
                if (effectiveTimer % interval == 0)
                {
                    for (int i = -1; i <= 1; i += 2)
                    {
                        float ratio = (float)effectiveTimer / attackTime;

                        float rotation = startRotation;
                        if (i < 0)
                            rotation += MathHelper.Pi;

                        float offsetLength = coverage * (1f - ratio) * i;
                        Vector2 offset = offsetLength * (startRotation + MathHelper.PiOver2).ToRotationVector2();
                        Vector2 spawnPos = targetPos + offset - coverage * rotation.ToRotationVector2();

                        float ai0 = rotation;
                        float ai1 = (float)effectiveTimer / attackTime;

                        Vector2 appearVel = -coverage / 60f * 0.8f * ai0.ToRotationVector2();
                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos - appearVel * 60, appearVel, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, ai0, ai1);
                    }
                }
            }

            bool quit = false;
            if (!npc.HasValidTarget)
            {
                npc.TargetClosest(false);
                if (!npc.HasValidTarget)
                    quit = true;
            }

            int threshold = delay + 90 + attackTime;
            if (++AttackTimer <= threshold) //stop vanilla ai from progressing
                npc.ai[1] = stop;
            else if (!quit && P2SwordsAttackCounter % SwordWallCap != SwordWallCap - 1) //increment the sword counter and repeat this attack
                npc.ai[1] = stop - 1;
        }

        private void ExcelSpreadsheet(NPC npc, float stop)
        {
            if (AttackTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Item161, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center);

                startRotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            int waveDelay = /*WorldSavingSystem.MasochistModeReal ? 20 :*/ 30;
            const int spaceCovered = 800;
            if (++AttackTimer > 0)
            {
                if (AttackTimer % waveDelay == 0)
                {
                    float ai1 = 1;
                    Vector2 spawnPos = targetPos;
                    spawnPos += 600f * Vector2.UnitX.RotatedBy(startRotation);
                    spawnPos += MathHelper.Lerp(-spaceCovered, spaceCovered, ai1) * Vector2.UnitY.RotatedBy(startRotation);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 vel = (startRotation + MathHelper.Pi).ToRotationVector2();
                        Vector2 appearVel = vel.RotatedBy(-MathHelper.PiOver2);
                        appearVel *= WorldSavingSystem.MasochistModeReal ? 2f : 1f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos - 60f * appearVel, appearVel, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, vel.ToRotation(), ai1);
                    }

                    targetPos = npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center;
                    startRotation += MathHelper.PiOver2 * (Main.rand.NextBool() ? -1 : 1);
                    if (Main.rand.NextBool())
                        startRotation += MathHelper.Pi;
                    startRotation += MathHelper.ToRadians(WorldSavingSystem.MasochistModeReal ? 30 : 15) * Main.rand.NextFloat(-1, 1);

                    //whooshy sound effect
                    if (AttackTimer % waveDelay * 4 == 0)
                        SoundEngine.PlaySound(SoundID.Item163, Main.player[npc.target].Center);
                }

                if (AttackTimer % /*(WorldSavingSystem.MasochistModeReal ? 2 : 3)*/ 3 == 0)
                {
                    float ai1 = (float)(AttackTimer % waveDelay) / waveDelay;
                    Vector2 spawnPos = targetPos;
                    spawnPos += 600f * Vector2.UnitX.RotatedBy(startRotation);
                    spawnPos += MathHelper.Lerp(-spaceCovered, spaceCovered, ai1) * Vector2.UnitY.RotatedBy(startRotation);

                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 vel = (startRotation + MathHelper.Pi).ToRotationVector2();
                        Vector2 appearVel = vel.RotatedBy(-MathHelper.PiOver2);
                        appearVel *= WorldSavingSystem.MasochistModeReal ? 1.5f : 1f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos - 60f * appearVel, appearVel, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, vel.ToRotation(), ai1);
                    }
                }
            }

            if (!npc.HasValidTarget)
            {
                npc.TargetClosest(false);
                if (!npc.HasValidTarget)
                    AttackTimer += 9000;
            }

            int waves = WorldSavingSystem.MasochistModeReal ? 12 : 8;
            if (AttackTimer < waveDelay * waves + waveDelay * 2)
                npc.ai[1] = stop; //stop vanilla ai from progressing
        }

        private void Dash(NPC npc, bool useP2Attacks)
        {
            const int dashValue = 4;

            bool doSunWings = AttackCounter % 2 == 0;

            if (npc.ai[1] == 0)
            {
                AttackTimer = 0;

                if (!doSunWings)
                    SoundEngine.PlaySound(SoundID.Item164, Main.player[npc.target].Center);

                if (--DashCounter <= 0) //sometimes do two consecutive dashes
                {
                    DashCounter = dashValue;
                    npc.ai[2] -= 1;
                }
                else if (WorldSavingSystem.MasochistModeReal && npc.life < npc.lifeMax / 2) //RANDOM ATTACKS
                {
                    npc.ai[2] += Main.rand.Next(3);
                }

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (npc.ai[1] < 40)
            {
                bool shouldIncrement = true;

                if (!doSunWings)
                {
                    Vector2 targetPos = Main.player[npc.target].Center;
                    targetPos.X += 550f * (npc.ai[0] == 8 ? 1 : -1);
                    npc.Center = Vector2.Lerp(npc.Center, targetPos, 0.1f);
                    if (npc.Distance(targetPos) < 240)
                    {
                        int direction = ++AttackTimer % 2 == 0 ? -1 : 1;

                        float ai1 = (npc.ai[1] - 10) / 30f;

                        Vector2 vel = Main.rand.NextFloat(24f) * direction * Vector2.UnitY;
                        vel.X += 30f * Math.Sign(npc.SafeDirectionTo(Main.player[npc.target].Center).X);

                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ProjectileID.HallowBossRainbowStreak, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, npc.target, ai1);
                    }
                    else
                    {
                        shouldIncrement = false;
                        if (npc.ai[1] > 1f)
                            npc.ai[1] -= 1f;
                    }
                }

                if (shouldIncrement && DashCounter == dashValue - 1) //for the second consecutive dash
                {
                    if (npc.ai[1] == 0) //add the sound since the longer startup broke it
                        SoundEngine.PlaySound(SoundID.Item160, npc.Center);

                    if (npc.ai[1] < 39) //more startup on this one
                        npc.ai[1] -= 0.33f;
                    else
                        npc.ai[1] = 39; //fix any rounding issues
                }
            }

            if (npc.ai[1] == 40) //add sun wings
            {
                if (doSunWings)
                {
                    float baseDirection = npc.ai[0] == 8 ? 0 : MathHelper.Pi;
                    for (int i = -2; i <= 2; i++)
                    {
                        if (i == 0)
                            continue;

                        float ai0 = baseDirection + MathHelper.ToRadians(20) / 2 * i;
                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.FairyQueenSunDance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, ai0, npc.whoAmI);
                    }
                }
            }

            if (npc.ai[1] >= 40)
            {
                if (doSunWings || !WorldSavingSystem.MasochistModeReal)
                    npc.ai[1] -= 0.33f; //extend the dash

                if (!WorldSavingSystem.MasochistModeReal)
                    npc.velocity.Y = 0;

                if (doSunWings && useP2Attacks && ++AttackTimer % 15 == 0) //extra swords, p2 only
                {
                    float baseDirection = npc.ai[0] == 8 ? 0 : MathHelper.Pi;
                    for (int i = -2; i <= 2; i++)
                    {
                        if (i == 0)
                            continue;

                        if (FargoSoulsUtil.HostCheck)
                        {
                            float ai0 = baseDirection + MathHelper.ToRadians(40) / 2 * i;
                            float ai1 = (npc.ai[1] - 40f) / 50f;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(BaseProjDmg(npc), 1.5f), 0f, Main.myPlayer, ai0, ai1);
                        }
                    }
                }
            }
        }

        private void PullNonTargets(NPC npc, Vector2 center, float radius)
        {
            if (npc.target == Main.myPlayer)
                return;
            float distance = center.Distance(Main.LocalPlayer.Center);
            float threshold = radius;
            Player player = Main.LocalPlayer;
            if (player.active && !player.dead && !player.ghost) //pull into arena
            {
                if (distance > threshold && distance < 3500)
                {
                    if (distance > 1500)
                    {
                        player.Incapacitate();
                        player.velocity.X = 0f;
                        player.velocity.Y = -0.4f;
                    }

                    Vector2 movement = center - player.Center;
                    float difference = movement.Length() - threshold;
                    movement.Normalize();
                    movement *= difference < 32f ? difference : 32f;
                    player.position += movement;

                    for (int i = 0; i < 10; i++)
                    {
                        int DustType = Main.rand.NextFromList(DustID.RainbowTorch);
                        int d = Dust.NewDust(player.position, player.width, player.height, DustType, 0f, 0f, 0, default, 1.25f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 5f;
                    }
                }
            }
        }

        #endregion helper funcs

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<SmiteBuff>(), 600);
        }

        public override void SafeModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.SafeModifyHitByProjectile(npc, projectile, ref modifiers);

            if (projectile.FargoSouls().Homing == true && !FargoSoulsUtil.IsSummonDamage(projectile))
                modifiers.FinalDamage *= 0.75f;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (npc.life < npc.lifeMax / 2)
                modifiers.FinalDamage *= 2.0f / 3.0f;

            base.ModifyIncomingHit(npc, ref modifiers);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 37);

            //LoadGoreRange(recolor, 1262, 1268);

            LoadExtra(recolor, 156); //shader, but isnt working...
            LoadExtra(recolor, 157);
            LoadExtra(recolor, 158);
            LoadExtra(recolor, 159);
            LoadExtra(recolor, 160);
            LoadExtra(recolor, 187);
            LoadExtra(recolor, 188);
        }
    }
}
