﻿using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Timber
{
    public class LesserSquirrel : ModNPC
    {
        public override string Texture => "FargowiltasSouls/Content/NPCs/Critters/TophatSquirrelCritter";

        public int counter;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lesser Squirrel");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "小松鼠");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(
                   ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<TimberChampion>()],
                   quickUnlock: true
               );
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            ]);
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 32;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1800;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0f;
            NPC.knockBackResist = .1f;

            AnimationType = NPCID.Squirrel;

            NPC.aiStyle = 7;
            AIType = NPCID.Squirrel;
            if (WorldSavingSystem.EternityMode)
            {
                NPC.aiStyle = NPCAIStyleID.Herpling;
                AIType = NPCID.Herpling;
            }

            NPC.dontTakeDamage = true;
        }

        bool spawnedByP1;

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<TimberChampion>()) is NPC)
                spawnedByP1 = true;
        }

        public override void AI()
        {
            if (NPC.velocity.Y == 0)
            {
                if (!Main.projectile.Any(p => p.active && (p.type == ModContent.ProjectileType<TimberTree>() || p.type == ModContent.ProjectileType<TimberTreeAcorn>())))
                    NPC.dontTakeDamage = false;
            }

            if (++counter > 900)
            {
                NPC.SimpleStrikeNPC(NPC.lifeMax, 0, false, 0, null, false, 0, true);
            }

            if (!NPC.dontTakeDamage && WorldSavingSystem.EternityMode)
            {
                Vector2 nextPos = NPC.position;
                nextPos.X += NPC.velocity.X * 1.5f;
                if (NPC.velocity.Y < 0)
                    nextPos.Y += NPC.velocity.Y;
                if (!Collision.SolidTiles(nextPos, NPC.width, NPC.height))
                    NPC.position = nextPos;
            }
        }

        public override bool CheckDead()
        {
            if (FargoSoulsUtil.HostCheck)
            {
                int p = Player.FindClosest(NPC.Center, 0, 0);
                NPC npc = Main.npc.FirstOrDefault(n => n.active && (n.type == ModContent.NPCType<TimberChampion>() || n.type == ModContent.NPCType<TimberChampionHead>()));
                if (p != -1 && npc is NPC)
                {
                    bool canAttack = true;
                    if (spawnedByP1)
                        canAttack = NPC.AnyNPCs(ModContent.NPCType<TimberChampion>());
                    //Vector2 vel = 4f * NPC.SafeDirectionTo(Main.player[p].Center);
                    //int type = ModContent.ProjectileType<DeviLostSoul>();
                    //float ai0 = 0;
                    //float ai1 = 0;
                    //if (npc.type == ModContent.NPCType<TimberChampionHead>())
                    //{
                    //    vel = 6f * -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    //    type = ProjectileID.HallowBossRainbowStreak;
                    //    ai0 = npc.target;
                    //    ai1 = Main.rand.NextFloat();
                    //}
                    if (canAttack)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 6f * -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4),
                            ProjectileID.HallowBossRainbowStreak, FargoSoulsUtil.ScaledProjectileDamage(110), 0, Main.myPlayer, npc.target, Main.rand.NextFloat());
                    }
                }
            }
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f);

                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 5f;
                    dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 1f);
                    Main.dust[dust].velocity *= 3f;
                }

                for (int j = 0; j < 4; j++)
                {
                    int gore = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                    Main.gore[gore].velocity *= 0.4f;
                    Main.gore[gore].velocity += new Vector2(1f, 1f).RotatedBy(MathHelper.TwoPi / 4 * j);
                }

                if (!Main.dedServ)
                {
                    int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextFloat(6f) * -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4), ModContent.Find<ModGore>(Mod.Name, "TrojanSquirrelGore2_2").Type, NPC.scale);
                    Main.gore[g].rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
            }
        }
    }
}
