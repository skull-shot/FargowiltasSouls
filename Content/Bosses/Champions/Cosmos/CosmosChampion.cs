﻿using Fargowiltas.Content.NPCs;
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Placables.Relics;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using FargowiltasSouls.Content.Sky;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.ItemDropRules;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    [AutoloadBossHead]
    public class CosmosChampion : ModNPC
    {
        bool hitChildren;
        float epicMe;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eridanus, Champion of Cosmos");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "厄里达诺斯, 宇宙英灵");

            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPC.AddDebuffImmunities(
            [
                BuffID.Confused,
                BuffID.Chilled,
                BuffID.OnFire,
                BuffID.Suffocation,
                BuffID.Lovestruck,
                ModContent.BuffType<LethargicBuff>(),
                ModContent.BuffType<LightningRodBuff>()
            ]);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = false,
                Position = new Vector2(8, 16),
                PortraitScale = 1f,
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 8
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            ]);
        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                // This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
                return NPC.GetBestiaryEntryColor();
            }
            return base.GetAlpha(drawColor);
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 100;
            NPC.damage = 115;
            NPC.defense = 70;
            NPC.lifeMax = 500000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(10);
            NPC.boss = true;

            /*Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Silent") : MusicID.OtherworldlyLunarBoss;
            SceneEffectPriority = SceneEffectPriority.BossLow;*/

            NPC.scale *= 1.5f;

            NPC.dontTakeDamage = true;
            NPC.alpha = 255;

            NPC.trapImmune = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            //NPC.damage = (int)(FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f * 0.5f));
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = ImmunityCooldownID.Bosses;
            if (NPC.localAI[3] == 0) //just spawned
                return false;
            if (NPC.ai[0] == 1 && NPC.ai[2] < 60) //doing punches with a lot of reposition
                return false;
            return true;
        }

        /*public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (Animation == 15 && NPC.ai[1] > 90 && NPC.ai[1] < 210) //intangible during timestop
                return false;
            return null;
        }*/
        ref float Animation => ref NPC.ai[0];
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(hitChildren);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            hitChildren = reader.ReadBoolean();
        }
        public override int SpawnNPC(int tileX, int tileY)
        {
            return base.SpawnNPC(tileX, tileY);
        }

        void SprayPunch(Vector2 target, int max, float aimVarianceDegrees)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < max; i++)
                {
                    Vector2 offset = Vector2.UnitX;
                    if (NPC.direction < 0)
                        offset.X *= -1f;
                    offset = offset.RotatedBy(NPC.SafeDirectionTo(target).ToRotation());

                    offset += Main.rand.NextVector2Circular(NPC.width / 12, NPC.width / 12);

                    Vector2 vel = NPC.SafeDirectionTo(target).RotatedByRandom(MathHelper.ToRadians(aimVarianceDegrees));
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + offset + 3000 * -vel,
                        vel,
                        ModContent.ProjectileType<CosmosDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 1.25f), 0f, Main.myPlayer);
                }
            }
        }
        public override void AI()
        {
            EModeGlobalNPC.championBoss = NPC.whoAmI;

            if (!SkyManager.Instance["FargowiltasSouls:EridanusSky"].IsActive())
                SkyManager.Instance.Activate("FargowiltasSouls:EridanusSky");

            Main.dayTime = false;
            Main.time = 16200; //midnight, for empress visuals

            Main.raining = false; //disable rain
            Main.rainTime = 0;
            Main.maxRaining = 0;

            Main.bloodMoon = false; //disable blood moon

            if (NPC.localAI[3] == 0) //just spawned
            {
                if (!NPC.HasValidTarget)
                    NPC.TargetClosest(false);

                if (NPC.ai[1] == 0)
                {
                    NPC.Center = Main.player[NPC.target].Center - 250 * Vector2.UnitY;
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CosmosVortex>(), 0, 0f, Main.myPlayer);
                }
                /*if (NPC.ai[1] == 117) // 1 frame before so can music fade up next frame
                {
                    if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
                    {
                        Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/PlatinumStar");
                    }
                    else
                    {
                        Music = MusicID.OtherworldlyLunarBoss;
                    }
                }
                if (NPC.ai[1] > 117)
                    Main.musicFade[Main.curMusic] += 0.2f;*/
                if (++NPC.ai[1] > 120)
                {
                    NPC.netUpdate = true;
                    NPC.ai[1] = 0;
                    NPC.localAI[3] = 1;

                    NPC.velocity = NPC.DirectionFrom(Main.player[NPC.target].Center).RotatedByRandom(MathHelper.PiOver2) * 20f;

                }
                return;
            }

            NPC.alpha = 0;

            Player player = Main.player[NPC.target];
            Vector2 targetPos;

            if (NPC.HasValidTarget && NPC.Distance(player.Center) < 2500 || NPC.localAI[3] == 0)
                NPC.timeLeft = 600;

            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;

            if (NPC.localAI[2] == 0 && Animation != -1 && NPC.life < NPC.lifeMax * (WorldSavingSystem.EternityMode ? .8 : 5))
            {
                if (Animation == 15 && NPC.ai[1] < 210 + 60) //dont phase transition during timestop
                {
                    NPC.life = (int)(NPC.lifeMax * (WorldSavingSystem.EternityMode ? .8 : 5));
                }
                else
                {
                    float buffer = Animation;
                    Animation = -1;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = buffer;
                    NPC.netUpdate = true;

                    FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                }
            }

            if (WorldSavingSystem.EternityMode && NPC.localAI[2] < 2 && Animation != -2 && NPC.life < NPC.lifeMax * .2)
            {
                if (Animation == 15 && NPC.ai[1] < 210 + 60) //dont phase transition during timestop
                {
                    NPC.life = (int)(NPC.lifeMax * .2);
                }
                else
                {
                    Animation = -2;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.netUpdate = true;

                    FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                }
            }

            NPC.dontTakeDamage = false;

            bool IsDeviantt(int n) => Main.npc[n].active && !Main.npc[n].dontTakeDamage && (Main.npc[n].type == ModContent.NPCType<Deviantt>() || Main.npc[n].type == ModContent.NPCType<DeviBoss.DeviBoss>());
            switch ((int)Animation)
            {
                case -4: //hit children
                    {
                        NPC.timeLeft = 600;

                        int ai2 = (int)NPC.ai[2];
                        if (++NPC.ai[3] < 420 && ai2 > -1 && ai2 < Main.maxNPCs && IsDeviantt(ai2))
                        {
                            targetPos = Main.npc[ai2].Center;
                            NPC.direction = NPC.spriteDirection = NPC.Center.X < targetPos.X ? 1 : -1;

                            targetPos.X += NPC.width / 4 * (NPC.Center.X < targetPos.X ? -1 : 1);
                            if (NPC.Distance(targetPos) > NPC.width / 4)
                                Movement(targetPos, 1.6f, 64f);

                            if (NPC.localAI[1] == 0)
                            {
                                NPC.localAI[1] = 1;
                                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                            }

                            if (++NPC.localAI[0] <= 3)
                            {
                                NPC.rotation = NPC.SafeDirectionTo(Main.npc[ai2].Center).ToRotation();
                                if (NPC.direction < 0)
                                    NPC.rotation += (float)Math.PI;

                                if (NPC.localAI[0] == 3)
                                {
                                    NPC.netUpdate = true;
                                    SprayPunch(Main.npc[ai2].Center, Main.rand.Next(1, 8), 9);
                                }
                            }
                            else
                            {
                                if (NPC.localAI[0] > 6)
                                {
                                    NPC.localAI[0] = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            if (NPC.ai[3] >= 420) //if couldn't kill deviantt in 6 seconds, just stop trying
                                hitChildren = true;

                            Animation = NPC.ai[1];
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case -3: //final phase
                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f || player.Center.Y / 16f > Main.worldSurface) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.velocity.Y -= 1f;
                        break;
                    }

                    NPC.rotation = 0;
                    NPC.velocity *= 0.9f;

                    player.wingTime = player.wingTimeMax;

                    if (NPC.ai[1] == 0)
                    {
                        NPC.ai[1] = 1;

                        if (!Main.dedServ && Main.LocalPlayer.active)
                            ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);

                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            if (WorldSavingSystem.EternityMode)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CosmosRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f * 2 / 7), 0f, Main.myPlayer, 0f, NPC.whoAmI);

                            int max = 2;
                            float startRotation = NPC.DirectionFrom(player.Center).ToRotation() + MathHelper.PiOver2; //ensure never spawn one directly at you
                            if (WorldSavingSystem.MasochistModeReal)
                            {
                                max = 3;
                                startRotation = NPC.DirectionFrom(player.Center).ToRotation();
                                if (Main.getGoodWorld)
                                {
                                    max = 4;
                                    startRotation = NPC.DirectionFrom(player.Center).ToRotation() + MathHelper.PiOver4;
                                }
                            }
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CosmosMoon>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f * 2 / 7), 0f, Main.myPlayer, MathHelper.TwoPi / max * i + startRotation, NPC.whoAmI);
                            }

                            //Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -2);

                            epicMe = 1f;
                        }

                        Vector2 size = new(500, 500);
                        Vector2 spawnPos = NPC.Center;
                        spawnPos.X -= size.X / 2;
                        spawnPos.Y -= size.Y / 2;

                        for (int num615 = 0; num615 < 30; num615++)
                        {
                            int num616 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                            Main.dust[num616].velocity *= 1.4f;
                        }

                        for (int num617 = 0; num617 < 50; num617++)
                        {
                            int num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                            Main.dust[num618].noGravity = true;
                            Main.dust[num618].velocity *= 7f;
                            num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                            Main.dust[num618].velocity *= 3f;
                        }

                        for (int num619 = 0; num619 < 2; num619++)
                        {
                            float scaleFactor9 = 0.4f;
                            if (num619 == 1) scaleFactor9 = 0.8f;
                            int num620 = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore97 = Main.gore[num620];
                            gore97.velocity.X++;
                            Gore gore98 = Main.gore[num620];
                            gore98.velocity.Y++;
                            num620 = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore99 = Main.gore[num620];
                            gore99.velocity.X--;
                            Gore gore100 = Main.gore[num620];
                            gore100.velocity.Y++;
                            num620 = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore101 = Main.gore[num620];
                            gore101.velocity.X++;
                            Gore gore102 = Main.gore[num620];
                            gore102.velocity.Y--;
                            num620 = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore103 = Main.gore[num620];
                            gore103.velocity.X--;
                            Gore gore104 = Main.gore[num620];
                            gore104.velocity.Y--;
                        }


                        for (int k = 0; k < 20; k++) //make visual dust
                        {
                            Vector2 dustPos = spawnPos;
                            dustPos.X += Main.rand.Next((int)size.X);
                            dustPos.Y += Main.rand.Next((int)size.Y);

                            for (int i = 0; i < 20; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, DustID.Smoke, 0f, 0f, 100, default, 3f);
                                Main.dust[dust].velocity *= 1.4f;
                            }

                            for (int i = 0; i < 10; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity *= 7f;
                                dust = Dust.NewDust(dustPos, 32, 32, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                                Main.dust[dust].velocity *= 3f;
                            }

                            float scaleFactor9 = 0.5f;
                            for (int j = 0; j < 2; j++)
                            {
                                int gore = Gore.NewGore(NPC.GetSource_FromThis(), dustPos, default, Main.rand.Next(61, 64));
                                Main.gore[gore].velocity *= scaleFactor9;
                                Main.gore[gore].velocity.X += 1f;
                                Main.gore[gore].velocity.Y += 1f;
                            }
                        }
                    }

                    if (++NPC.ai[2] > 200 || NPC.ai[2] == 100)
                    {
                        if (NPC.ai[2] > 200)
                            NPC.ai[2] = 0;

                        NPC.netUpdate = true;

                        SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                        if (!Main.dedServ && Main.LocalPlayer.active)
                            ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);

                        //int type; //for dust

                        if (NPC.ai[3] == 0) //solar
                        {
                            NPC.ai[3]++;
                            if (FargoSoulsUtil.HostCheck)
                            {
                                float modifier = NPC.life / (NPC.lifeMax * 0.2f);
                                if (modifier < 0)
                                    modifier = 0;
                                if (modifier > 1)
                                    modifier = 1;
                                modifier = 11f + 4f * modifier;

                                const int max = 12;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, modifier * NPC.SafeDirectionTo(player.Center).RotatedBy(2 * Math.PI / max * i),
                                        ModContent.ProjectileType<CosmosFireball2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 30, 30 + 60);
                                }
                            }
                        }
                        else if (NPC.ai[3] == 1) //vortex
                        {
                            NPC.ai[3]++;
                            //type = 229;
                            if (FargoSoulsUtil.HostCheck)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(FargosSoundRegistry.Thunder with { Volume = 0.8f, Pitch = 0.5f }, NPC.Center);
                                const int max = 16;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 dir = NPC.SafeDirectionTo(player.Center).RotatedBy(2 * (float)Math.PI / max * i);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * NPC.width / 120f, ModContent.ProjectileType<LightningVortexHostile>(), //ModContent.ProjectileType<CosmosLightning>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer, 1f, dir.ToRotation());
                                }
                            }
                        }
                        else if (NPC.ai[3] == 2) //nebula
                        {
                            NPC.ai[3]++;
                            //type = 242;
                            if (FargoSoulsUtil.HostCheck)
                            {
                                /*const int max = 11;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, 3f * NPC.SafeDirectionTo(player.Center).RotatedBy(2 * Math.PI / max * (i + 0.5)),
                                        ModContent.ProjectileType<CosmosNebulaBlaze>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0.007f);
                                }*/

                                for (int j = -1; j <= 1; j++) //to both sides
                                {
                                    if (j == 0)
                                        continue;

                                    const int gap = 30;
                                    const int max = 15;
                                    const int individualOffset = 8;
                                    Vector2 baseVel = NPC.SafeDirectionTo(Main.player[NPC.target].Center).RotatedBy(MathHelper.ToRadians(gap) * j);
                                    for (int k = 0; k < max; k++) //a fan of blazes
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 6f * baseVel.RotatedBy(MathHelper.ToRadians(individualOffset) * j * k),
                                            ModContent.ProjectileType<CosmosNebulaBlaze>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0.009f);
                                    }
                                }
                            }
                        }
                        else //stardust
                        {
                            NPC.ai[3] = 0;
                            //type = 135;
                            if (FargoSoulsUtil.HostCheck)
                            {
                                const int max = 18;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(2 * Math.PI / max * i),
                                        ModContent.ProjectileType<CosmosInvader>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 180, 0.04f);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(2 * Math.PI / max * (i + 0.5)),
                                        ModContent.ProjectileType<CosmosInvader>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 180, 0.025f);
                                }
                                for (int j = 0; j < 5; j++)
                                {
                                    Vector2 vel = -Vector2.UnitY.RotatedBy(MathHelper.Pi * 0.4f * j);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<CosmosGlowything>(), 0, 0f, Main.myPlayer);
                                }
                                SoundEngine.PlaySound(SoundID.Item25, NPC.Center);
                            }
                        }

                        /*const int num226 = 150;
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.UnitX * 50f;
                            vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226)) + NPC.Center;
                            Vector2 vector7 = vector6 - NPC.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, type, 0f, 0f, 0, default);
                            Main.dust[num228].scale = 3f;
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].velocity = vector7;
                        }*/

                        /*for (int index = 0; index < 50; ++index) //dust
                        {
                            Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                            dust.velocity *= 10f;
                            dust.fadeIn = 1f;
                            dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                            if (Main.rand.Next(3) != 0)
                            {
                                dust.noGravity = true;
                                dust.velocity *= 3f;
                                dust.scale *= 2f;
                            }
                        }

                        Vector2 size = new Vector2(500, 500);
                        Vector2 spawnPos = NPC.Center;
                        spawnPos.X -= size.X / 2;
                        spawnPos.Y -= size.Y / 2;

                        for (int num615 = 0; num615 < 30; num615++)
                        {
                            int num616 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, 31, 0f, 0f, 100, default(Color), 1.5f);
                            Main.dust[num616].velocity *= 1.4f;
                        }

                        for (int num617 = 0; num617 < 50; num617++)
                        {
                            int num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
                            Main.dust[num618].noGravity = true;
                            Main.dust[num618].velocity *= 7f;
                            num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                            Main.dust[num618].velocity *= 3f;
                        }

                        for (int num619 = 0; num619 < 2; num619++)
                        {
                            float scaleFactor9 = 0.4f;
                            if (num619 == 1) scaleFactor9 = 0.8f;
                            int num620 = Gore.NewGore(NPC.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore97 = Main.gore[num620];
                            gore97.velocity.X = gore97.velocity.X + 1f;
                            Gore gore98 = Main.gore[num620];
                            gore98.velocity.Y = gore98.velocity.Y + 1f;
                            num620 = Gore.NewGore(NPC.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore99 = Main.gore[num620];
                            gore99.velocity.X = gore99.velocity.X - 1f;
                            Gore gore100 = Main.gore[num620];
                            gore100.velocity.Y = gore100.velocity.Y + 1f;
                            num620 = Gore.NewGore(NPC.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore101 = Main.gore[num620];
                            gore101.velocity.X = gore101.velocity.X + 1f;
                            Gore gore102 = Main.gore[num620];
                            gore102.velocity.Y = gore102.velocity.Y - 1f;
                            num620 = Gore.NewGore(NPC.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore103 = Main.gore[num620];
                            gore103.velocity.X = gore103.velocity.X - 1f;
                            Gore gore104 = Main.gore[num620];
                            gore104.velocity.Y = gore104.velocity.Y - 1f;
                        }


                        for (int k = 0; k < 20; k++) //make visual dust
                        {
                            Vector2 dustPos = spawnPos;
                            dustPos.X += Main.rand.Next((int)size.X);
                            dustPos.Y += Main.rand.Next((int)size.Y);

                            for (int i = 0; i < 20; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, 31, 0f, 0f, 100, default(Color), 3f);
                                Main.dust[dust].velocity *= 1.4f;
                            }

                            for (int i = 0; i < 10; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity *= 7f;
                                dust = Dust.NewDust(dustPos, 32, 32, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                                Main.dust[dust].velocity *= 3f;
                            }

                            float scaleFactor9 = 0.5f;
                            for (int j = 0; j < 2; j++)
                            {
                                int gore = Gore.NewGore(dustPos, default(Vector2), Main.rand.Next(61, 64));
                                Main.gore[gore].velocity *= scaleFactor9;
                                Main.gore[gore].velocity.X += 1f;
                                Main.gore[gore].velocity.Y += 1f;
                            }
                        }*/
                    }
                    break;

                case -2: //prepare for last phase
                    NPC.rotation = 0;
                    NPC.dontTakeDamage = true;

                    NPC.localAI[2] = 2;

                    targetPos = player.Center;
                    targetPos.X += 600 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    Movement(targetPos, 0.8f, 32f);

                    if (--NPC.ai[2] < 0)
                    {
                        NPC.ai[2] = Main.rand.Next(5);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                            int type = ModContent.ProjectileType<PhantasmalBlast>();
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        }
                    }

                    if (++NPC.ai[1] > 150)
                    {
                        /*const int num226 = 80;
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.UnitX * 40f;
                            vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + NPC.Center;
                            Vector2 vector7 = vector6 - NPC.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 229, 0f, 0f, 0, default(Color), 3f);
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].velocity = vector7;
                        }*/

                        NPC.TargetClosest();
                        Animation--;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case -1: //phase 2 transition
                    NPC.rotation = 0;
                    NPC.dontTakeDamage = true;

                    NPC.velocity *= 0.9f;

                    if (++NPC.ai[1] == 120)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                        NPC.localAI[2] = 1;

                        //if (FargoSoulsUtil.HostCheck) Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -2);
                        epicMe = 1f;

                    }
                    else if (NPC.ai[1] > 180) //LAUGH
                    {
                        NPC.TargetClosest();
                        Animation = NPC.ai[3];
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 0: //float near player, skip next attack and wait if not p2
                    NPC.rotation = 0;

                    if ((!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f || player.Center.Y / 16f > Main.worldSurface)
                        && NPC.localAI[3] != 0) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.velocity.Y -= 1f;
                        break;
                    }

                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 500;
                    Movement(targetPos, 0.8f, 32f);

                    if (NPC.ai[1] == 0 && !(NPC.localAI[2] == 0 || !Main.expertMode))
                    {
                        if (Animation != 5 && FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, NPC.whoAmI, -23);

                    }

                    if (++NPC.ai[1] > 60)
                    {
                        float oldAi0 = Animation;

                        NPC.TargetClosest();
                        Animation += NPC.localAI[2] == 0 || !Main.expertMode ? 2 : 1;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;

                        if (!hitChildren)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++) //look for deviantt to kill
                            {
                                if (Main.npc[i].active && NPC.Distance(Main.npc[i].Center) < 2000 && player.Distance(Main.npc[i].Center) < 2000 && IsDeviantt(i))
                                {
                                    Animation = -4;
                                    NPC.ai[1] = oldAi0;
                                    NPC.ai[2] = i; //store target npc
                                    break;
                                }
                            }

                        }
                    }
                    break;

                case 1: //deathray punches, p2 only
                    {
                        if (NPC.ai[1] == 1)
                        {
                            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                            NPC.ai[3] = NPC.Center.X < player.Center.X ? MathHelper.Pi : 0;
                            NPC.localAI[0] = 0;
                            NPC.netUpdate = true;
                        }

                        bool move = true;

                        int punchTime = WorldSavingSystem.MasochistModeReal ? 45 : 80;
                        int endlag = WorldSavingSystem.MasochistModeReal ? 15 : 15;

                        if (NPC.ai[2] == Math.Max(punchTime - 40, 0) && FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.SafeDirectionTo(player.Center), ModContent.ProjectileType<CosmosTelegraph>(), 0, 0f, Main.myPlayer, ai0: player.whoAmI, ai1: NPC.whoAmI);
                        }

                        if (++NPC.ai[2] == punchTime)
                        {
                            NPC.netUpdate = true;
                            NPC.localAI[0] = endlag;

                            NPC.ai[3] = player.velocity.ToRotation();

                            NPC.velocity = 47f * NPC.DirectionTo(player.Center);
                            if (FargoSoulsUtil.HostCheck)
                            {
                                int modifier = Math.Sign(NPC.Center.Y - player.Center.Y);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + 3000 * NPC.DirectionFrom(player.Center) * modifier, NPC.SafeDirectionTo(player.Center) * modifier,
                                    ModContent.ProjectileType<CosmosDeathray3>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 1.25f), 0f, Main.myPlayer);
                            }

                            NPC.rotation = NPC.velocity.ToRotation();
                        }

                        if (NPC.ai[2] >= punchTime)
                        {
                            move = false;
                            NPC.velocity *= 0.98f;
                            NPC.rotation = NPC.velocity.ToRotation();

                            NPC.ai[3] = Vector2.Lerp(NPC.ai[3].ToRotationVector2(), player.velocity.SafeNormalize(Vector2.Zero), 0.1f).ToRotation();

                            if (NPC.ai[2] > punchTime + endlag)
                            {
                                NPC.netUpdate = true;
                                NPC.ai[2] = 0;
                            }
                        }

                        if (move)
                        {
                            targetPos = player.Center + 360 * NPC.ai[3].ToRotationVector2();
                            float accel = WorldSavingSystem.MasochistModeReal ? 3.5f : 2f;
                            float maxSpeed = WorldSavingSystem.MasochistModeReal ? 51f : 33f;
                            Movement(targetPos, accel, maxSpeed, useAntiWobble: true);
                            NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
                        }

                        if (NPC.direction < 0)
                            NPC.rotation += (float)Math.PI;

                        if (NPC.localAI[0] > 0) //control punch anim
                            NPC.localAI[0]--;

                        int punchesToDo = WorldSavingSystem.MasochistModeReal ? 8 : 5;
                        if (++NPC.ai[1] > (punchTime + endlag) * punchesToDo)
                        {
                            NPC.TargetClosest();
                            Animation++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.localAI[0] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 2: //float near player, proceed to next attack always
                    if (Animation != 10)
                    {
                        NPC.rotation = 0;
                        targetPos = player.Center + NPC.DirectionFrom(player.Center) * 500;
                        Movement(targetPos, 0.8f, 32f);
                    }

                    if ((!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f)
                        && NPC.localAI[3] != 0) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.velocity.Y -= 1f;
                        break;
                    }

                    if (++NPC.ai[1] > 60)
                    {
                        float oldAi0 = Animation;

                        NPC.TargetClosest();
                        Animation++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;

                        if (!hitChildren)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++) //look for deviantt to kill
                            {
                                if (Main.npc[i].active && IsDeviantt(i) && NPC.Distance(Main.npc[i].Center) < 1200 && player.Distance(Main.npc[i].Center) < 1200)
                                {
                                    Animation = -4;
                                    NPC.ai[1] = oldAi0;
                                    NPC.ai[2] = i; //store target npc
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case 3: //fireball dashes
                    {
                        if (NPC.ai[1] == 1)
                        {
                            if (FargoSoulsUtil.HostCheck) //spawn balls
                            {
                                const int max = 8;
                                const float distance = 120f;
                                float rotation = 2f * (float)Math.PI / max;
                                int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 spawnPos = NPC.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<CosmosFireball>(), damage, 0f, Main.myPlayer, NPC.whoAmI, rotation * i);
                                    //if (p != Main.maxProjectiles && !(WorldSavingSystem.MasochistMode && NPC.localAI[2] != 0f)) Main.projectile[p].timeLeft = 300;
                                }
                            }
                        }

                        int threshold = 70; //NPC.localAI[2] == 0 ? 70 : 50;
                        if (WorldSavingSystem.MasochistModeReal)
                            threshold = 50;
                        if (++NPC.ai[2] <= threshold)
                        {
                            targetPos = player.Center;
                            targetPos.X += 600 * (NPC.Center.X < targetPos.X ? -1 : 1);
                            targetPos.Y += 300 * (NPC.Center.Y < targetPos.Y ? -1 : 1);
                            Movement(targetPos, 1.6f, 24f);

                            NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
                            if (NPC.direction < 0)
                                NPC.rotation += (float)Math.PI;

                            if (NPC.ai[2] == threshold)
                            {
                                NPC.velocity = 42f * NPC.SafeDirectionTo(player.Center);
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                            if (NPC.ai[2] > threshold + 30)
                            {
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;

                                if (WorldSavingSystem.EternityMode && NPC.localAI[2] != 0f) //emode p2, do chain blasts
                                {
                                    if (!Main.dedServ && Main.LocalPlayer.active)
                                        ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);

                                    if (FargoSoulsUtil.HostCheck) //chain explosions
                                    {
                                        Vector2 baseDirection = NPC.SafeDirectionTo(player.Center);
                                        const int max = 6; //spread
                                        for (int i = 0; i < max; i++)
                                        {
                                            Vector2 offset = NPC.height / 2 * baseDirection.RotatedBy(Math.PI * 2 / max * i);
                                            float ai1 = i <= 1 || i == max - 1 ? 32 : 8;
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2), Vector2.Zero, ModContent.ProjectileType<Projectiles.Eternity.MoonLordSunBlast>(),
                                                FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, MathHelper.WrapAngle(offset.ToRotation()), ai1);
                                        }
                                    }
                                }
                            }
                        }

                        if (++NPC.ai[1] > 330) //(WorldSavingSystem.MasochistMode && NPC.localAI[2] != 0f ? 360 : 330))
                        {
                            NPC.TargetClosest();
                            Animation++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 4:
                    goto case 0;

                case 5: //meteor punch
                    if (NPC.ai[1] == 1)
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    if (++NPC.ai[2] <= 75)
                    {
                        targetPos = player.Center;
                        targetPos.X += 350 * (NPC.Center.X < targetPos.X ? -1 : 1);
                        targetPos.Y -= 700;
                        Movement(targetPos, 1.6f, 32f, useAntiWobble: false);

                        NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
                        if (NPC.direction < 0)
                            NPC.rotation += (float)Math.PI;

                        NPC.localAI[0] = NPC.Center.X < player.Center.X ? 1 : -1; //store direction im facing

                        if (NPC.ai[2] == 75 - 50 && FargoSoulsUtil.HostCheck)
                        {
                            //Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.SafeDirectionTo(player.Center), ModContent.ProjectileType<CosmosTelegraph>(), 0, 0f, Main.myPlayer, ai0: player.whoAmI, ai1: NPC.whoAmI, ai2: 0.9f);
                        }

                        if (NPC.ai[2] == 75) //falling punch
                        {
                            NPC.velocity = 42f * NPC.SafeDirectionTo(player.Center);
                            NPC.netUpdate = true;

                            if (FargoSoulsUtil.HostCheck)
                            {
                                int modifier = Math.Sign(NPC.Center.Y - player.Center.Y);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + 3000 * NPC.DirectionFrom(player.Center) * modifier, NPC.SafeDirectionTo(player.Center) * modifier,
                                    ModContent.ProjectileType<CosmosDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 1.25f), 0f, Main.myPlayer);

                                const int max = 3;
                                for (int i = -max; i <= max; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 0.5f * -Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / max * i),
                                        ModContent.ProjectileType<CosmosBolt>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                }
                            }

                            NPC.ai[3] = Main.rand.Next(4); //random offset each time
                        }
                    }
                    else
                    {
                        NPC.direction = NPC.spriteDirection = Math.Sign(NPC.localAI[0]); //dont turn around if crossed up

                        if (++NPC.ai[3] > 4)
                        {
                            NPC.ai[3] = 0;
                            if (FargoSoulsUtil.HostCheck)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 0.5f * Vector2.UnitX, ModContent.ProjectileType<CosmosBolt>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -0.5f * Vector2.UnitX, ModContent.ProjectileType<CosmosBolt>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                            }
                        }
                    }
                    BackgroundShift(NPC.ai[2], 75, 110);

                    if (++NPC.ai[1] > 240 || NPC.ai[2] > 60 && NPC.Center.Y > player.Center.Y + 700)
                    {
                        NPC.velocity.Y = 0f;

                        if (FargoSoulsUtil.HostCheck)
                        {
                            const int max = 3;
                            for (int i = -max; i <= max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 0.5f * Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / max * i),
                                    ModContent.ProjectileType<CosmosBolt>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                            }
                        }

                        NPC.TargetClosest();
                        Animation++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 6:
                    goto case 2;

                case 7: //vortex
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 500;
                    Movement(targetPos, 0.6f, 32f);

                    if (NPC.ai[1] == 30)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            float ai1 = WorldSavingSystem.EternityMode && NPC.localAI[2] != 0 ? -1.2f : NPC.localAI[2] == 0 ? 1f : -1.6f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CosmosVortex>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, ai1);
                            /*for (int i = 0; i < 3; i++) //indicate how lightning will spawn
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), player.Center, (MathHelper.TwoPi / 3 * (i + 0.5f)).ToRotationVector2(),
                                      ModContent.ProjectileType<GlowLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 6, player.whoAmI);
                            }*/
                        }

                        int length = (int)NPC.Distance(player.Center);
                        Vector2 offset = NPC.SafeDirectionTo(player.Center);
                        for (int i = 0; i < length; i += 10) //dust warning line for sandnado
                        {
                            int d = Dust.NewDust(NPC.Center + offset * i, 0, 0, DustID.Vortex, 0f, 0f, 0, new Color());
                            Main.dust[d].noGravity = true;
                            Main.dust[d].scale = 1.5f;
                        }
                    }

                    if (++NPC.ai[1] > 450)
                    {
                        NPC.TargetClosest();
                        Animation++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 8:
                    goto case 0;

                case 9: //shen ray and balls torture
                    if (NPC.ai[1] == 1)
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    if (NPC.ai[2] == 0)
                    {
                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, NPC.whoAmI, -20);
                    }

                    if (++NPC.ai[2] <= 200)
                    {
                        targetPos = player.Center;
                        targetPos.X += 600 * (NPC.Center.X < targetPos.X ? -1 : 1);
                        NPC.position += player.velocity / 3f; //really good tracking movement here
                        Movement(targetPos, 1.2f, 32f);

                        if (--NPC.localAI[0] < 0)
                        {
                            NPC.localAI[0] = 90;

                            if (FargoSoulsUtil.HostCheck)
                            {
                                const int max = 11;
                                const int travelTime = 20;
                                for (int j = -1; j <= 1; j += 2)
                                {
                                    for (int i = -max; i <= max; i++)
                                    {
                                        Vector2 target = player.Center;
                                        target.X += 180f * i;
                                        target.Y += (400f + 300f / max * Math.Abs(i)) * j;
                                        //y pos is above and below player, adapt to always outspeed player, with additional V shapes
                                        Vector2 speed = (target - NPC.Center) / travelTime;
                                        int individualTiming = 60 + Math.Abs(i * 2);
                                        float direction = Math.Sign(player.Center.Y - target.Y);
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed / 2, ModContent.ProjectileType<CosmosSphere>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, travelTime, individualTiming, ai2: direction);
                                    }
                                }
                            }
                        }

                        NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
                        if (NPC.direction < 0)
                            NPC.rotation += (float)Math.PI;

                        NPC.ai[3] = NPC.Center.X < player.Center.X ? 1 : -1; //store direction im facing

                        if (NPC.ai[2] == 200 - 50 && FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.SafeDirectionTo(player.Center), ModContent.ProjectileType<CosmosTelegraph>(), 0, 0f, Main.myPlayer, ai0: player.whoAmI, ai1: NPC.whoAmI);
                        }

                        if (NPC.ai[2] == 200) //straight ray punch
                        {
                            NPC.velocity = 42f * NPC.SafeDirectionTo(player.Center);
                            NPC.netUpdate = true;

                            if (FargoSoulsUtil.HostCheck)
                            {
                                int modifier = Math.Sign(NPC.Center.Y - player.Center.Y);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + 3000 * NPC.DirectionFrom(player.Center) * modifier, NPC.SafeDirectionTo(player.Center) * modifier,
                                    ModContent.ProjectileType<CosmosDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 1.25f), 0f, Main.myPlayer);
                            }
                        }
                    }
                    else
                    {
                        NPC.direction = NPC.spriteDirection = Math.Sign(NPC.ai[3]); //dont turn around if crossed up
                    }

                    BackgroundShift(NPC.ai[2], 200, 235);

                    if (++NPC.ai[1] > 400 || NPC.ai[2] > 200 &&
                        (NPC.ai[3] > 0 ? NPC.Center.X > player.Center.X + 800 : NPC.Center.X < player.Center.X - 800))
                    {
                        NPC.velocity.X = 0f;

                        NPC.TargetClosest();
                        Animation++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 10: //dust telegraph for nebula punches
                    if (!NPC.HasValidTarget)
                        goto case 2;

                    void NebulaDust()
                    {
                        Vector2 dustPos = NPC.Center + new Vector2(-26 * NPC.direction, 22) * NPC.scale;
                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(dustPos, 0, 0, DustID.CrystalPulse2, NPC.velocity.X * 0.3f, NPC.velocity.Y * 0.3f, 160, new Color(), 1f);
                            Main.dust[d].scale = 2.4f;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 3f;
                        }
                    };

                    NebulaDust();

                    if (NPC.ai[1] == 0)
                        SoundEngine.PlaySound(SoundID.Item117, NPC.Center);


                    targetPos = player.Center;
                    targetPos.X += 550 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.8f, 24f);

                    NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
                    if (NPC.direction < 0)
                        NPC.rotation += (float)Math.PI;

                    goto case 2;

                case 11: //reticle and nebula blazes
                    targetPos = player.Center;

                    if (WorldSavingSystem.EternityMode && NPC.localAI[2] != 0)
                    {
                        int sign = NPC.Center.X < targetPos.X ? -1 : 1;
                        Vector2 offset = 600 * sign * Vector2.UnitX;
                        float rotation = MathHelper.ToRadians(80) * NPC.ai[1] / 420 * -sign;
                        targetPos += offset.RotatedBy(rotation); //emode p2, slowly rotate to above the player

                        NPC.position += player.velocity / 3f; //really good tracking movement here to reduce odds of crossups
                        Movement(targetPos, 0.8f, 24f);
                    }
                    else //just float beside player
                    {
                        targetPos.X += 550 * (NPC.Center.X < targetPos.X ? -1 : 1);
                        Movement(targetPos, 0.8f, 24f);
                    }

                    NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
                    if (NPC.direction < 0)
                        NPC.rotation += (float)Math.PI;

                    int meatballFreq = 2;
                    int meatballTime = WorldSavingSystem.MasochistModeReal ? meatballFreq * 16 : meatballFreq * 8;
                    if (NPC.ai[1] % 90 <= meatballTime + 10 && NPC.ai[1] % 90 >= 10 && NPC.ai[1] + 90 < 390)
                    {
                        if (NPC.ai[1] % meatballFreq == 0) // cosmetic meatballs
                        {
                            if (FargoSoulsUtil.HostCheck) //rain meteors
                            {
                                Vector2 spawnPos = NPC.Center;
                                Vector2 vel = Main.rand.NextFloat(12f, 16f) * -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 * 0.3f);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<CosmosMeteor>(), 0, 0f, Main.myPlayer, 0f, Main.rand.NextFloat(1f, 1.5f));
                            }
                        }
                    }

                    if (NPC.ai[1] == 30 && FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CosmosReticle>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, NPC.whoAmI);
                    }

                    if (WorldSavingSystem.MasochistModeReal && NPC.ai[1] == 60 && FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CosmosReticle>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, NPC.whoAmI);
                    }

                    if (NPC.ai[1] > 60)
                    {
                        if (++NPC.ai[3] == 3)
                        {
                            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                            if (FargoSoulsUtil.HostCheck)
                            {
                                Vector2 offset = new(80, 6);
                                if (player.Center.X < NPC.Center.X)
                                    offset.X *= -1f;
                                offset = offset.RotatedBy(NPC.rotation);
                                for (int i = 0; i < 2; i++)
                                {
                                    float rotation = MathHelper.ToRadians(NPC.localAI[2] == 0 ? 30 : 20) + Main.rand.NextFloat(MathHelper.ToRadians(10));
                                    if (i == 0)
                                        rotation *= -1f;
                                    Vector2 vel = Main.rand.NextFloat(8f, 12f) * NPC.SafeDirectionTo(player.Center).RotatedBy(rotation);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + offset, vel, ModContent.ProjectileType<CosmosNebulaBlaze>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0.006f);
                                    if (WorldSavingSystem.EternityMode && NPC.localAI[2] != 0)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + offset, vel.RotatedBy(rotation * Main.rand.NextFloat(1f, 4f)), ModContent.ProjectileType<CosmosNebulaBlaze>(),
                                          FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0.006f);
                                    }
                                }
                            }
                        }
                        else if (NPC.ai[3] > 6)
                        {
                            NPC.ai[3] = 0;
                        }
                    }
                    else
                    {
                        NebulaDust();
                    }

                    if (++NPC.ai[1] > 390)
                    {
                        NPC.TargetClosest();
                        Animation++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 12:
                    goto case 0;

                case 13: //multi-punch to uppercut
                    {
                        int firstAttackTime = 220;
                        if (++NPC.ai[1] < firstAttackTime)
                        {
                            targetPos = player.Center;
                            targetPos.X += 300 * (NPC.Center.X < targetPos.X ? -1 : 1);
                            Movement(targetPos, 1.2f, 32f, useAntiWobble: false);

                            if (NPC.ai[1] == 1)
                            {
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.SafeDirectionTo(player.Center), ModContent.ProjectileType<CosmosTelegraph>(), 0, 0f, Main.myPlayer, ai0: player.whoAmI, ai1: NPC.whoAmI);
                                }
                            }
                            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                            int startup = 30;

                            void DoPunchLogic()
                            {
                                int attackTime = 12; //must be divisible by 2
                                if (++NPC.ai[2] <= attackTime / 2)
                                {
                                    NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
                                    if (NPC.direction < 0)
                                        NPC.rotation += (float)Math.PI;

                                    NPC.ai[3] = NPC.Center.X < player.Center.X ? 1 : -1; //store direction im facing

                                    if (NPC.ai[2] == attackTime / 2)
                                    {
                                        NPC.netUpdate = true;
                                        if (NPC.ai[1] > startup)
                                        {
                                            int max = 1;
                                            float aimVarianceDegrees = 0;
                                            if (NPC.ai[1] > startup + 60)
                                            {
                                                max += Main.rand.Next(3);
                                                aimVarianceDegrees += 3;
                                            }
                                            if (NPC.ai[1] > startup + 120)
                                            {
                                                max += Main.rand.Next(4);
                                                aimVarianceDegrees += 6;
                                            }

                                            SprayPunch(player.Center, max, aimVarianceDegrees);
                                        }
                                        else
                                        {
                                            NPC.ai[2] = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    NPC.direction = NPC.spriteDirection = Math.Sign(NPC.ai[3]); //dont turn around if crossed up

                                    if (NPC.ai[2] > attackTime)
                                    {
                                        NPC.ai[2] = 0;
                                        NPC.ai[3] = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }

                            int punchCountMax = 1;
                            if (NPC.ai[1] > startup + 60)
                                punchCountMax++;
                            if (NPC.ai[1] > startup + 120)
                                punchCountMax++;
                            for (int punchCounts = 0; punchCounts < punchCountMax; punchCounts++)
                                DoPunchLogic();
                        }
                        else //uppercut time
                        {
                            BackgroundShift(NPC.ai[1], firstAttackTime + 45, firstAttackTime + 45 + 35);

                            if (NPC.ai[1] <= firstAttackTime + 45)
                            {
                                targetPos = player.Center;
                                targetPos.X += 350 * (NPC.Center.X < targetPos.X ? -1 : 1);
                                targetPos.Y += 700;
                                NPC.position += player.velocity / 3f; //really good tracking movement here
                                Movement(targetPos, 2.4f, 32f);

                                NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
                                if (NPC.direction < 0)
                                    NPC.rotation += (float)Math.PI;

                                //NPC.ai[3] = NPC.Center.X < player.Center.X ? 1 : -1; //store direction im facing


                                if (NPC.ai[1] == firstAttackTime + 45) //rising punch
                                {
                                    const float speed = 42f;
                                    NPC.velocity = speed * NPC.SafeDirectionTo(player.Center);
                                    NPC.netUpdate = true;

                                    NPC.ai[3] = Math.Abs(player.Center.Y - NPC.Center.Y) / speed; //time to travel to player's Y coord
                                    NPC.ai[3] *= 2f; //travel twice that to go above

                                    NPC.localAI[0] = player.Center.X;
                                    NPC.localAI[1] = player.Center.Y;

                                    NPC.localAI[0] += NPC.Center.X < player.Center.X ? -50 : 50; //horiz offset the centerpoint to one side closer to eri

                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        int modifier = Math.Sign(NPC.Center.Y - player.Center.Y);
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + 3000 * NPC.DirectionFrom(player.Center) * modifier, NPC.SafeDirectionTo(player.Center) * modifier,
                                            ModContent.ProjectileType<CosmosDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 1.25f), 0f, Main.myPlayer);
                                    }
                                }
                            }
                            else
                            {
                                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X); //dont turn around if crossed up
                                NPC.rotation = NPC.velocity.ToRotation();
                                if (NPC.direction < 0)
                                    NPC.rotation += (float)Math.PI;

                                if (Math.Abs(NPC.Center.Y - NPC.localAI[1]) < 300) //make the midpoint better at hitting people
                                {
                                    //for finer grain, since eri moves too much in one tick (too much angular rotation change per tick when close)
                                    Vector2 midPos = NPC.Center - NPC.velocity / 2;
                                    Vector2 target = new(NPC.localAI[0], NPC.localAI[1]);
                                    Vector2 vel = Vector2.Normalize(midPos - target);
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        int modifier = Math.Sign(player.Center.X - target.X) == NPC.direction ? 1 : -1;
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, modifier * 0.5f * vel, ModContent.ProjectileType<CosmosBolt>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, modifier * 0.5f * NPC.DirectionFrom(target), ModContent.ProjectileType<CosmosBolt>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                    }
                                }
                                else if (++NPC.ai[2] > 1)
                                {
                                    NPC.ai[2] = 0;

                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        Vector2 target = new(NPC.localAI[0], NPC.localAI[1]);
                                        int modifier = Math.Sign(player.Center.X - target.X) == NPC.direction ? 1 : -1;
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 0.5f * NPC.DirectionFrom(target), ModContent.ProjectileType<CosmosBolt>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                    }
                                }

                                if (NPC.ai[1] > firstAttackTime + 45 + NPC.ai[3])
                                {
                                    NPC.velocity.Y = 0f;

                                    NPC.TargetClosest();
                                    Animation++;
                                    NPC.ai[1] = WorldSavingSystem.EternityMode && NPC.localAI[2] != 0 ? 0 : -120;
                                    NPC.ai[2] = 0;
                                    NPC.ai[3] = 0;
                                    NPC.localAI[0] = 0;
                                    NPC.localAI[1] = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                    }
                    break;

                case 14:
                    goto case 2;

                case 15: //ZA WARUDO
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 500;
                    if (NPC.ai[1] < 130 || NPC.Distance(player.Center) > 200 && NPC.Distance(player.Center) < 600)
                    {
                        NPC.velocity *= 0.97f;
                    }
                    else
                    {
                        Movement(targetPos, 0.6f, 32f);
                        NPC.position += player.velocity / 4f;
                    }

                    if (NPC.ai[1] >= 10) //for timestop visual
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            ManagedScreenFilter filter = ShaderManager.GetFilter("FargowiltasSouls.Invert");
                            filter.SetFocusPosition(NPC.Center);
                        }
                    }

                    if (NPC.ai[1] < 10)
                    {
                        //need to keep this empty case so the below cases dont run early
                    }
                    else if (NPC.ai[1] == 10)
                    {
                        NPC.localAI[0] = Main.rand.NextFloat(2 * (float)Math.PI);

                        if (!Main.dedServ)
                            SoundEngine.PlaySound(FargosSoundRegistry.ZaWarudo, player.Center);
                    }
                    else if (NPC.ai[1] < 210)
                    {
                        int duration = 60 + Math.Max(2, 210 - (int)NPC.ai[1]);

                        if (Main.LocalPlayer.active && !Main.LocalPlayer.dead)
                        {
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<TimeFrozenBuff>(), duration);
                            //Main.LocalPlayer.AddBuff(BuffID.ChaosState, 300); //no cheesing this attack
                        }

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && i != NPC.whoAmI && Main.npc[i].type != NPC.type)
                                Main.npc[i].AddBuff(ModContent.BuffType<TimeFrozenBuff>(), duration, true);
                        }

                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && !Main.projectile[i].FargoSouls().TimeFreezeImmune)
                                Main.projectile[i].FargoSouls().TimeFrozen = duration;
                        }


                        if (NPC.ai[1] > 150 && ++NPC.ai[2] > 8)
                        {
                            NPC.ai[2] = 0;

                            bool phase2Attack = WorldSavingSystem.EternityMode && NPC.localAI[2] != 0;
                            int cap = phase2Attack ? 6 : 8;

                            if (NPC.ai[3] > -1)
                            {
                                DoTimestopAttack(player, phase2Attack);
                                NPC.ai[3]++;
                                if (!phase2Attack) //need to do more because p1 has more waves
                                {
                                    DoTimestopAttack(player, phase2Attack);
                                    NPC.ai[3]++;
                                }
                                if (NPC.ai[3] > cap)
                                    NPC.ai[3] = -1; //using ai3 to control animation too
                            }
                        }
                    }
                    else
                    {
                        NPC.ai[3] = -1;
                    }

                    if (++NPC.ai[1] > 480)
                    {
                        NPC.TargetClosest();
                        Animation++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                default:
                    Animation = 0;
                    goto case 0;
            }

            epicMe -= 0.02f;
            if (epicMe < 0)
                epicMe = 0;
        }
        void DoTimestopAttack(Player player, bool phase2Attack)
        {
            int baseDistance = 300; //altAttack ? 500 : 400;
            float offset = phase2Attack ? 250f : 150f;
            float speed = phase2Attack ? 4f : 2.5f;
            int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage); //altAttack ? FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f * 2 / 7 ): FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);

            if (phase2Attack && NPC.ai[3] % 2 == 0) //emode p2, asgore rings
            {
                float radius = baseDistance + NPC.ai[3] * offset;
                int circumference = (int)(2f * (float)Math.PI * radius);

                //always flip it to opposite the previous side
                NPC.localAI[0] = MathHelper.WrapAngle(NPC.localAI[0] + (float)Math.PI + Main.rand.NextFloat((float)Math.PI / 2));
                const float safeRange = 60f;

                const int arcLength = 120;
                for (int i = 0; i < circumference; i += arcLength)
                {
                    float angle = i / radius;
                    if (angle > 2 * Math.PI - MathHelper.WrapAngle(MathHelper.ToRadians(safeRange)))
                        continue;

                    float spawnOffset = radius;// + Main.rand.NextFloat(-16, 16);
                    Vector2 spawnPos = player.Center + spawnOffset * Vector2.UnitX.RotatedBy(angle + NPC.localAI[0]);
                    Vector2 attackVel = speed * player.DirectionFrom(spawnPos);
                    Vector2 spawnVel = (spawnPos - NPC.Center) / 9f;
                    float ai0 = player.Distance(spawnPos) / speed + 40;
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, spawnVel, ModContent.ProjectileType<CosmosInvaderTime>(), damage, 0f, Main.myPlayer, ai0, attackVel.ToRotation(), attackVel.Length());
                }
            }
            else //scatter
            {
                int max = phase2Attack ? 12 : 8 + (int)NPC.ai[3] * (NPC.localAI[2] == 0 ? 2 : 4);
                float rotationOffset = Main.rand.NextFloat((float)Math.PI * 2);
                for (int i = 0; i < max; i++)
                {
                    float ai0 = baseDistance;
                    float distance = ai0 + NPC.ai[3] * offset;
                    Vector2 spawnPos = player.Center + distance * Vector2.UnitX.RotatedBy(2 * Math.PI / max * i + rotationOffset);
                    Vector2 attackVel = speed * player.DirectionFrom(spawnPos);// distance * player.DirectionFrom(spawnPos) / ai0;
                    Vector2 spawnVel = (spawnPos - NPC.Center) / 9f;
                    ai0 = distance / speed + 40;
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, spawnVel, ModContent.ProjectileType<CosmosInvaderTime>(), damage, 0f, Main.myPlayer, ai0, attackVel.ToRotation(), attackVel.Length());
                }
            }
        }
        public override void PostAI()
        {
            //immune to timestop during own timestop
            NPC.buffImmune[ModContent.BuffType<TimeFrozenBuff>()] = Animation == 15 && NPC.ai[1] < 210;
        }
        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false, bool useAntiWobble = true)
        {
            if (useAntiWobble)
            {
                float accel = 1f * speedModifier;
                float decel = 1.5f * speedModifier;
                float resistance = NPC.velocity.Length() * accel / (35f * speedModifier);
                NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, targetPos, NPC.velocity, accel - resistance, decel + resistance);
            }
            else
            {
                if (NPC.Center.X < targetPos.X)
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
                if (NPC.Center.Y < targetPos.Y)
                {
                    NPC.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y += speedModifier * 2;
                }
                else
                {
                    NPC.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y -= speedModifier * 2;
                }
            }

            float dist = NPC.Distance(targetPos);
            if (dist == 0)
                dist = 0.1f;
            if (NPC.velocity.Length() > dist)
                NPC.velocity = Vector2.Normalize(NPC.velocity) * dist;
            MathHelper.Clamp(NPC.velocity.X, -cap, cap);
            MathHelper.Clamp(NPC.velocity.Y, -cap, cap);
        }

        public override void FindFrame(int frameHeight)
        {
            const int HandsBackFrame = 5;
            const int PunchFrame = 6;
            const int HandsUpFrame1 = 7;
            const int HandsUpFrame2 = 8;

            if (++NPC.frameCounter > 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
            }

            if (NPC.frame.Y > frameHeight * HandsBackFrame - 1)
            {
                NPC.frame.Y = 0;
            }

            switch ((int)Animation)
            {
                case -4:
                    NPC.frame.Y = frameHeight * HandsBackFrame;
                    if (NPC.localAI[0] >= 3)
                        NPC.frame.Y = frameHeight * PunchFrame;
                    break;

                case -3:
                    if (NPC.ai[2] < 30 || NPC.ai[2] > 100 && NPC.ai[2] < 130)
                        NPC.frame.Y = frameHeight * HandsUpFrame2;
                    else if (NPC.ai[2] > 70 && NPC.ai[2] < 100 || NPC.ai[2] > 170)
                        NPC.frame.Y = frameHeight * HandsUpFrame1;
                    break;

                case -2:
                    NPC.frame.Y = frameHeight * HandsBackFrame;
                    break;

                case -1:
                    if (NPC.ai[1] > 120)
                        NPC.frame.Y = frameHeight * HandsUpFrame2;
                    else if (NPC.ai[1] > 100)
                        NPC.frame.Y = frameHeight * HandsUpFrame1;
                    break;

                case 1:
                    if (NPC.localAI[0] > 0)
                        NPC.frame.Y = frameHeight * PunchFrame;
                    else
                        NPC.frame.Y = frameHeight * HandsBackFrame;
                    break;

                case 3:
                    {
                        int threshold = 70; //NPC.localAI[2] == 0 ? 70 : 50;
                        if (NPC.ai[2] <= threshold)
                            NPC.frame.Y = frameHeight * HandsBackFrame;
                        else
                            NPC.frame.Y = frameHeight * PunchFrame;
                    }
                    break;

                case 5:
                    if (NPC.ai[2] <= 75)
                        NPC.frame.Y = frameHeight * HandsBackFrame;
                    else
                        NPC.frame.Y = frameHeight * PunchFrame;
                    break;

                case 7:
                    if (NPC.ai[1] < 30)
                        NPC.frame.Y = frameHeight * HandsUpFrame1;
                    else if (NPC.ai[1] < 60)
                        NPC.frame.Y = frameHeight * HandsUpFrame2;
                    break;

                case 9:
                    if (NPC.ai[2] <= 200)
                        NPC.frame.Y = frameHeight * HandsBackFrame;
                    else
                        NPC.frame.Y = frameHeight * PunchFrame;
                    break;

                case 10:
                    NPC.frame.Y = frameHeight * HandsBackFrame;
                    break;

                case 11:
                    if (NPC.ai[1] > 60)
                        NPC.frame.Y = frameHeight * PunchFrame;
                    else
                        NPC.frame.Y = frameHeight * HandsBackFrame;
                    break;

                case 13:
                    if (NPC.ai[1] < 220)
                    {
                        if (NPC.ai[2] <= 6)
                            NPC.frame.Y = frameHeight * HandsBackFrame;
                        else
                            NPC.frame.Y = frameHeight * PunchFrame;
                    }
                    else //uppercut time
                    {
                        if (NPC.ai[1] <= 220 + 45)
                            NPC.frame.Y = frameHeight * HandsBackFrame;
                        else
                            NPC.frame.Y = frameHeight * PunchFrame;
                    }
                    break;

                case 14:
                    NPC.frame.Y = frameHeight * HandsUpFrame1;
                    break;

                case 15: //ZA WARUDO
                    if (NPC.ai[1] < 10)
                        NPC.frame.Y = frameHeight * HandsUpFrame1;
                    else if (NPC.ai[1] < 130)
                        NPC.frame.Y = frameHeight * HandsUpFrame2;
                    else if (NPC.ai[1] > 150 && NPC.ai[3] > -1)
                    {
                        if (NPC.ai[2] < 4 && NPC.ai[3] > 0)
                            NPC.frame.Y = frameHeight * PunchFrame;
                        else
                            NPC.frame.Y = frameHeight * HandsBackFrame;
                    }
                    break;

                        default:
                    break;
                }

            if (Main.projectile.Any(p => p.TypeAlive<GlowRing>() && p.ai[0] == NPC.whoAmI && (p.ai[1] == -23 || p.ai[1] == -20)))
            {
                NPC.frame.Y = frameHeight * HandsBackFrame;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<DaybrokenBuff>(), 120);
                target.AddBuff(BuffID.Electrified, 300);
                //target.AddBuff(ModContent.BuffType<BerserkedBuff>(), 300);
                target.AddBuff(BuffID.Frostburn, 300);
            }
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (WorldSavingSystem.EternityMode)
                modifiers.FinalDamage *= 0.9f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"CosmosGore{i}").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CosmosChampion], -1);

            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                    int type = ModContent.ProjectileType<MutantBomb>();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                }
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Eternity.MoonLordMoonBlast>(), 0, 0f, Main.myPlayer, -Vector2.UnitY.ToRotation(), 32);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // I have setup the loot placement in this way because 
            // when registering loot for an npc, the bestiary checks for the order of loot registered.
            // For parity with vanilla, the order is as follows: Trophy, Classic Loot, Expert Loot, Master loot.

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EridanusTrophy>(), 10));

            npcLoot.Add(new ChampionEnchDropRule(BaseForce.EnchantsIn<CosmoForce>()));

            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Eridanium>(), 1, 5, 10));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.Find<ModItem>("Fargowiltas", "CrucibleCosmos").Type));

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CosmosBag>()));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<EridanusRelic>()));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //if (!NPC.IsABestiaryIconDummy)
            //{
            //    spriteBatch.End();
            //    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            //}

            Texture2D npcTex = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;

            Texture2D npcGlow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/Champions/Cosmos/CosmosChampion_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D npcGlow2 = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/Champions/Cosmos/CosmosChampion_Glow2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawPos = NPC.Center - screenPos;
            if (Main.LocalPlayer.gravDir < 0)
            {
                drawPos.Y = Main.screenHeight - drawPos.Y;
            }

            Color glowColor = new(Main.DiscoR / 3 + 150, Main.DiscoG / 3 + 150, Main.DiscoB / 3 + 150);
            void lerpGlow(Color color, float modifier)
            {
                if (modifier < 0)
                    modifier = 0;
                if (modifier > 1)
                    modifier = 1;
                glowColor.R = (byte)MathHelper.Lerp(color.R, glowColor.R, modifier);
                glowColor.G = (byte)MathHelper.Lerp(color.G, glowColor.G, modifier);
                glowColor.B = (byte)MathHelper.Lerp(color.B, glowColor.B, modifier);
            }
            switch (Animation)
            {
                /*case -4:
                    {
                        ulong seed = (ulong)(NPC.ai[3] / 6);
                        Color rainbowFlash = new Color(50 * Utils.RandomInt(ref seed, 0, 6) + 5, 50 * Utils.RandomInt(ref seed, 0, 6) + 5, 50 * Utils.RandomInt(ref seed, 0, 6) + 5);
                        lerpGlow(rainbowFlash, 0.2f);
                    }
                    break;*/

                case 2:
                    lerpGlow(Color.OrangeRed, 1f - NPC.ai[1] / 60);
                    break;

                case 3:
                    lerpGlow(Color.OrangeRed, NPC.ai[1] / 150 - 1f);
                    break;

                case 7:
                    lerpGlow(Color.LimeGreen, NPC.ai[1] < 30 ? 1f - NPC.ai[1] / 30 : (NPC.ai[1] - 30) / 360);
                    break;

                case 10:
                    lerpGlow(Color.Purple, 1f - NPC.ai[1] / 30);
                    break;

                case 11:
                    lerpGlow(Color.Purple, NPC.ai[1] / 180 - 1f);
                    break;

                case 14:
                    lerpGlow(Color.Blue, 1f - NPC.ai[1] / 60);
                    break;

                case 15:
                    lerpGlow(Color.Blue, NPC.ai[1] / 240 - 1f);
                    break;

                default: break;
            }

            if (!NPC.IsABestiaryIconDummy)
            {
                glowColor *= NPC.Opacity;

                spriteBatch.UseBlendState(BlendState.Additive);
            }

            if (NPC.localAI[2] != 0 || Animation == -4f)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldDrawPos = NPC.oldPos[i] - screenPos;
                    if (Main.LocalPlayer.gravDir < 0)
                    {
                        oldDrawPos.Y = Main.screenHeight - oldDrawPos.Y;
                    }
                    float num165 = NPC.rotation; //NPC.oldRot[i];
                    float scale = NPC.scale * Main.rand.NextFloat(1f, 1.2f);
                    Main.EntitySpriteDraw(npcGlow, oldDrawPos + NPC.Size / 2f + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor * 0.9f, num165, origin2, scale, effects, 0);
                }
            }

            if (epicMe > 0)
            {
                float scale = 10f * NPC.scale * (float)Math.Cos(Math.PI / 2 * epicMe); //modifier starts at 1 and drops to 0, so using cos
                float opacity = NPC.Opacity * (float)Math.Sqrt(epicMe);
                Main.EntitySpriteDraw(npcGlow, drawPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor * opacity, NPC.rotation, origin2, scale, effects, 0);
            }

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            }

            Main.EntitySpriteDraw(npcTex, drawPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);

            if (!NPC.IsABestiaryIconDummy)
            {
                Main.EntitySpriteDraw(npcGlow2, drawPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, NPC.rotation, origin2, NPC.scale, effects, 0);

                spriteBatch.UseBlendState(BlendState.Additive);

                Main.EntitySpriteDraw(npcGlow2, drawPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, NPC.rotation, origin2, NPC.scale, effects, 0);

                spriteBatch.ResetToDefault();
            }
            return false;
        }

        public void BackgroundShift(float timer, float start, float end)
        {
            if (timer > start && timer < end)
            {
                float strength = LumUtils.InverseLerp(end, start, timer);
                float easeoutTime = end - start;
                    
                EridanusSky.ScrollVector += strength * NPC.velocity * 0.035f / 30;
            }
        }
    }
}
