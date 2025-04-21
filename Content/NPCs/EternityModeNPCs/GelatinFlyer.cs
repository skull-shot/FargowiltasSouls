using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs
{
    public class GelatinFlyer : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_660";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type] = NPCID.Sets.SpecificDebuffImmunity[NPCID.QueenSlimeBoss];

            NPC.AddDebuffImmunities(
            [
                BuffID.Confused,
                BuffID.Chilled,
                BuffID.Suffocation,
                ModContent.BuffType<LethargicBuff>(),
                ModContent.BuffType<ClippedWingsBuff>()
            ]);

            this.ExcludeFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.QueenSlimeMinionPurple);

            //because they will double dip on expert/master scaling otherwise
            NPC.lifeMax = 110;
            NPC.damage = 50;

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.timeLeft = 60 * 60;
            NPC.noTileCollide = true;
            NPC.noGravity = true;

            NPC.scale *= 1.5f;
            NPC.lifeMax *= 3;
        }

        //public override bool CanHitNPC(NPC target) => false;

        //public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0f;
            modifiers.DisableKnockback();
        }
        public ref float Offset => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public override void AI()
        {
            if (!NPC.AnyNPCs(NPCID.QueenSlimeBoss) || !EModeGlobalNPC.queenSlimeBoss.IsWithinBounds(Main.maxNPCs))
            {
                NPC.active = false;
                return;
            }

            NPC queenSlime = Main.npc[EModeGlobalNPC.queenSlimeBoss];
            NPC.target = queenSlime.target;
            NPC.knockBackResist = 0f;

            if (NPC.velocity.X > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
            }
            if (NPC.velocity.X < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 3.14f;
            }

            if (NPC.HasPlayerTarget)
            {
                Player player = Main.player[NPC.target];
                if (Timer <= 0)
                {
                    Timer--;
                    Vector2 hoverPos = new(player.Center.X + Offset, player.Center.Y - 190);
                    Movement(hoverPos);
                    if (NPC.Distance(hoverPos) < 60 && Timer < -30)
                    {
                        Timer = 1;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    Timer++;
                    float dir = queenSlime.HorizontalDirectionTo(player.Center);
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dir * 13, 0.1f);
                    NPC.velocity.Y += 0.5f;
                    if (Timer > 100)
                    {
                        NPC.active = false;
                        return;
                    }
                }
            }
            else
            {
                NPC.active = false;
                return;
            }
        }
        public void Movement(Vector2 targetPos)
        {
            float speedModifier = 1.2f;
            float accel = 1f * speedModifier;
            float decel = 1.5f * speedModifier;
            float resistance = NPC.velocity.Length() * accel / (22f * speedModifier);
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, targetPos, NPC.velocity, accel - resistance, decel + resistance);
        }

        /*
        public override bool CheckDead()
        {
            if (NPC.DeathSound != null)
                SoundEngine.PlaySound(NPC.DeathSound.Value, NPC.Center);
            NPC.active = false;

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PinkTorch);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 0.75f;
                }

                //for (int i = 0; i < 2 ; i++)
                //    if (!Main.dedServ)
                //            Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity / 2, 1260, NPC.scale);
            }
        }
        */

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                NPC.frame.Y = 0;
        }
    }
}