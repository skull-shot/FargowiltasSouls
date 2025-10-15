using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.SkyAndRain;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Hallow
{
    public class RainbowSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.RainbowSlime);

        public int Counter;
        public bool SpawnedByOtherSlime;
        public bool DoStompAttack;
        public bool FinishedSpawning;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            bitWriter.WriteBit(SpawnedByOtherSlime);
            bitWriter.WriteBit(DoStompAttack);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            SpawnedByOtherSlime = bitReader.ReadBit();
            DoStompAttack = bitReader.ReadBit();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            Counter++;

            if (!SpawnedByOtherSlime && Main.netMode == NetmodeID.Server && Counter <= 65 && Counter % 15 == 5) //mp sync
            {
                npc.netUpdate = true;
                NetSync(npc);
            }

            if (!SpawnedByOtherSlime && !FinishedSpawning)
            {
                FinishedSpawning = true;
                npc.lifeMax *= 5;
                npc.life = npc.lifeMax;
                npc.HealEffect(npc.lifeMax);

                npc.Center = npc.Bottom;
                npc.scale *= 3;
                npc.width *= 3;
                npc.height *= 3;
                npc.Bottom = npc.Center;
            }

            npc.dontTakeDamage = Counter < 30;

            if (DoStompAttack) //shoot spikes whenever jumping
            {
                if (npc.velocity.Y == 0f) //start attack
                {
                    DoStompAttack = false;
                    if (npc.HasPlayerTarget && FargoSoulsUtil.HostCheck)
                    {
                        const float gravity = 0.15f;
                        const float time = 120f;
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance += Main.player[npc.target].velocity * 30f;
                        distance.X /= time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        float ai0 = SpawnedByOtherSlime ? 1 : 0;
                        int max = SpawnedByOtherSlime ? 3 : 25;
                        float spread = SpawnedByOtherSlime ? 0.5f : 1.5f;
                        for (int i = 0; i < max; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance + spread * Main.rand.NextVector2Circular(-1f, 1f),
                                ModContent.ProjectileType<RainbowSlimeSpike>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 8), 0f, Main.myPlayer, ai0);
                        }
                    }
                }
            }
            else if (npc.velocity.Y > 0)
            {
                DoStompAttack = true;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 120);
            target.AddBuff(ModContent.BuffType<FlamesoftheUniverseBuff>(), 240);
        }

        public override bool CheckDead(NPC npc)
        {
            if (!SpawnedByOtherSlime)
            {
                npc.active = false;
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int slimeIndex = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), NPCID.RainbowSlime);
                        if (slimeIndex != Main.maxNPCs)
                        {
                            NPC slime = Main.npc[slimeIndex];
                            slime.GetGlobalNPC<RainbowSlime>().SpawnedByOtherSlime = true;
                            slime.velocity = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 1));

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slimeIndex);
                        }
                    }
                }
                for (int i = 0; i < 20; i++)
                {
                    int num469 = Dust.NewDust(npc.Center, npc.width, npc.height, DustID.RainbowMk2, -npc.velocity.X * 0.2f, -npc.velocity.Y * 0.2f, 100, default, 5f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(npc.Center, npc.width, npc.height, DustID.RainbowMk2, -npc.velocity.X * 0.2f, -npc.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[num469].velocity *= 2f;
                }
                return false;
            }
            else
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    int[] slimes = [NPCID.Crimslime, NPCID.Pinky, NPCID.Gastropod, NPCID.CorruptSlime];

                    for (int i = 0; i < slimes.Length; i++)
                    {
                        if (!Main.rand.NextBool(3))
                            continue;

                        int spawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), 1);
                        Main.npc[spawn].SetDefaults(slimes[i]);
                        Main.npc[spawn].velocity.X = npc.velocity.X * 2f;
                        Main.npc[spawn].velocity.Y = npc.velocity.Y;

                        NPC spawn2 = Main.npc[spawn];
                        spawn2.velocity.X += (Main.rand.Next(-20, 20) * 0.1f + i * npc.direction * 0.3f);
                        NPC spawn3 = Main.npc[spawn];
                        spawn3.velocity.Y -= (Main.rand.Next(0, 10) * 0.1f + i);
                        Main.npc[spawn].ai[0] = -1000 * Main.rand.Next(3);

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                    }
                }
            }

            return base.CheckDead(npc);
        }
    }
}
