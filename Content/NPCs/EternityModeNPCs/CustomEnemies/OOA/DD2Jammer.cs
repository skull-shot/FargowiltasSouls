using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA
{
    public class DD2Jammer : DD2Enemy
    {
        public override int AssignPointValue() => 1;

        public override string Texture => "Terraria/Images/NPC_" + NPCID.DD2WyvernT1;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions.OldOnesArmy,
                new FlavorTextBestiaryInfoElement("Mods.FargowiltasSouls.Bestiary.DD2Jammer")
            });
            base.SetBestiary(database, bestiaryEntry);
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 56;
            NPC.height = 56;
            NPC.lifeMax = DD2Event.OngoingDifficulty == 1 ? 150 : DD2Event.OngoingDifficulty == 2 ? 320 : 580;
            NPC.defense = DD2Event.OngoingDifficulty == 1 ? 8 : DD2Event.OngoingDifficulty == 2 ? 18 : 24;
            NPC.damage = 25;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter == 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y == frameHeight * 3)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public ref float state => ref NPC.ai[1];

        public ref float target => ref NPC.ai[0];

        public ref float timer => ref NPC.ai[2];

        public override void AI()
        {
            if (!DD2Event.Ongoing && false)
            {
                NPC.velocity.Y -= 0.07f;
                NPC.velocity.X *= 0.98f;
                return;
            }

            NPC.spriteDirection = NPC.direction;
            if (state == 0)
            {
                target = -1;
                if (NPC.velocity.Y > -4f)
                {
                    NPC.velocity.Y -= 0.05f;
                    if (DD2Event.Ongoing)
                        NPC.direction = (int)NPC.HorizontalDirectionTo(DD2Utils.GetEterniaCrystal().Center);
                    return;
                }
                NPC.velocity *= 0.5f;
                state = 1;
                NPC crystal = DD2Utils.GetEterniaCrystal();
                if (DD2Event.Ongoing && crystal != null)
                    NPC.velocity += 0.02f * Vector2.UnitX.RotatedBy((crystal.Center - NPC.Center).ToRotation());
                return;
            }
            else if (state == 2)
            {
                Projectile sentry = Main.projectile[(int)target];
                if (!sentry.active)
                {
                    state = 0;
                    target = -1;
                    return;
                }
                NPC.Center = sentry.Center;
                sentry.GetGlobalProjectile<DD2GlobalProj>().Jammed = true;
                return;
            }


            if (target == -1 || timer == 60) // Look for a DD2 sentry
            {
                NPC.velocity *= 0.8f;
                target = FindClosestDD2Sentry(NPC.Center);
            }

            timer++;
            if (target != -1) // target found
            {
                Projectile sentry = Main.projectile[(int)target];
                if (!sentry.active)
                {
                    target = -1;
                    state = 0;
                    return;
                }
                NPC.velocity += 0.05f * Vector2.UnitX.RotatedBy((sentry.Center - NPC.Center).ToRotation());
                NPC.direction = (int) NPC.HorizontalDirectionTo(sentry.Center);
                float dist = NPC.Distance(sentry.Center);
                if (dist < 40f)
                {
                    SoundEngine.PlaySound(SoundID.Item94, NPC.Center);
                    NPC.velocity *= 0f;
                    state = 2;
                    timer = 0;
                    return;
                }
            }
            else // No DD2 sentries
            {
                NPC crystal = DD2Utils.GetEterniaCrystal();
                if (crystal == null)
                    return;

                NPC.velocity += 0.07f * Vector2.UnitX.RotatedBy((crystal.Center - NPC.Center).ToRotation());
                return;
            }

            float speedCap = 3f;
            if (NPC.velocity.Length() > speedCap)
            {
                NPC.velocity.Normalize();
                NPC.velocity *= speedCap;
            }

        }

        public int FindClosestDD2Sentry(Vector2 position)
        {
            int n = -1;
            float dist = -1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || !ProjectileID.Sets.IsADD2Turret[p.type] || p.GetGlobalProjectile<DD2GlobalProj>().Jammed)
                    continue;

                float projDist = (p.Center - position).Length();
                if (dist == -1 || projDist < dist)
                {
                    dist = projDist;
                    n = i;
                }
            }

            return n;
        }

        public override void OnKill()
        {
            base.OnKill();
            if (state == 2)
            {
                Main.projectile[(int)target].GetGlobalProjectile<DD2GlobalProj>().Jammed = false;
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            IEntitySource source = projectile.GetSource_FromThis();
            if (projectile.GetGlobalProjectile<DD2GlobalProj>().IsADD2SentryProj)
            {
                modifiers.FinalDamage *= 0.1f;
            }
            base.ModifyHitByProjectile(projectile, ref modifiers);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
