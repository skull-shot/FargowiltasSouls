﻿using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.BloodMoon;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.BloodMoon
{
    public class Dreadnautilus : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BloodNautilus);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.boss = true;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            if (npc.ai[0] == 1)
                return false;

            return base.CanHitPlayer(npc, target, ref CooldownSlot);
        }

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                npc.velocity.Y -= 1f;
            }

            switch ((int)npc.ai[0])
            {
                case 0: //between attacks
                    if (npc.FargoSouls().BloodDrinker)
                    {
                        if (npc.HasValidTarget)
                        {
                            float modifier = npc.Distance(Main.player[npc.target].Center) > 900 ? 0.3f : 0.1f;
                            npc.velocity += modifier * npc.SafeDirectionTo(Main.player[npc.target].Center);
                        }
                        npc.position += npc.velocity;
                    }
                    break;

                case 1: //spinning around player, dashes at ai1=90, ends at ai1=270
                    if (npc.ai[1] <= 90)
                    {
                        if (npc.FargoSouls().BloodDrinker)
                            npc.ai[1]++; //less startup
                    }
                    else
                    {
                        if (npc.HasValidTarget)
                            npc.position += Main.player[npc.target].position - Main.player[npc.target].oldPosition;

                        //spawn thorn missiles on outside of spin
                        if (npc.ai[1] % 2 == 0 && npc.Distance(FargoSoulsUtil.ClosestPointInHitbox(Main.player[npc.target], npc.Center)) > 30)
                        {
                            float rotation = npc.velocity.ToRotation();
                            float diff = MathHelper.WrapAngle(rotation - npc.SafeDirectionTo(Main.player[npc.target].Center).ToRotation());
                            rotation += MathHelper.PiOver2 * System.Math.Sign(diff);

                            Vector2 vel = Vector2.UnitX.RotatedBy(rotation);
                            Vector2 spawnOffset = 100f * vel;

                            if (FargoSoulsUtil.HostCheck)
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + spawnOffset, vel, ModContent.ProjectileType<BloodThornMissile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
                        }
                    }
                    break;

                case 2: //spit blood on you, shoot repeatedly after ai=90, ends at ai1=180
                    if (npc.ai[1] < 90 && npc.FargoSouls().BloodDrinker)
                        npc.ai[1]++;

                    npc.ai[1] += 0.5f;
                    break;

                case 3: //glowing, spawning blood squids, ends at ai1=180
                    {
                        void Checks()
                        {
                            if (npc.ai[1] == 60)
                                SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                            if (npc.ai[1] >= 120 && npc.ai[1] % 15 == 5) //when done
                            {
                                for (int i = 0; i < 10; i++) //spawn blood spikes around player
                                {
                                    Vector2 target = Main.player[npc.target].Center + Main.rand.NextVector2Circular(64, 16);
                                    Vector2 spawnPos = Main.player[npc.target].Bottom + new Vector2(Main.rand.NextFloat(-256, 256), Main.rand.NextFloat(64));
                                    spawnPos.Y += 250;

                                    for (int j = 0; j < 40; j++)
                                    {
                                        Tile tile = Framing.GetTileSafely(spawnPos);
                                        if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                                            break;
                                        spawnPos.Y += 16;
                                    }

                                    Vector2 vel = Vector2.Normalize(target - spawnPos);

                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos + vel * 50, 0.6f * vel, ModContent.ProjectileType<BloodThornMissile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);

                                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, 16f * vel.RotatedByRandom(MathHelper.ToRadians(10)), ProjectileID.SharpTears, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer, 0f, Main.rand.NextFloat(0.5f, 1f));
                                    }
                                }
                            }
                        }

                        Checks();

                        if (npc.FargoSouls().BloodDrinker && npc.ai[1] % 10 != 0)
                        {
                            npc.ai[1]++;
                            Checks();
                        }
                    }
                    break;

                default:
                    break;
            }

            //suck in and kill other enemies
            foreach (NPC n in Main.npc.Where(n => n.active && !n.boss && n.lifeMax <= 1000 && /*n.life < n.lifeMax * 0.6 &&*/ npc.Distance(n.Center) < 666 && Collision.CanHitLine(n.Center, 0, 0, npc.Center, 0, 0)))
            {
                if (npc.Distance(n.Center) < npc.width / 4)
                {
                    npc.AddBuff(ModContent.BuffType<BloodDrinkerBuff>(), 360);

                    //int heal = n.life;
                    //npc.life += heal;
                    //if (npc.life > npc.lifeMax)
                    //    npc.life = npc.lifeMax;
                    //CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal, true);

                    CombatText.NewText(n.Hitbox, Color.Red, n.life, true);

                    n.life = 0;
                    n.HitEffect();
                    n.checkDead();
                    n.active = false;
                }
                else
                {
                    n.position -= n.velocity;
                    n.position += npc.velocity / 2;
                    n.position += n.SafeDirectionTo(npc.Center) * n.velocity.Length() * 2f;
                }
            }

            return base.SafePreAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 600);
        }

        public override bool PreKill(NPC npc)
        {
            npc.boss = false;

            return base.PreKill(npc);
        }
    }

    public class BloodSquid : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BloodSquid);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            //npc.knockBackResist += 0.1f;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //FargoSoulsUtil.PrintAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 1200);
        }
    }
}
