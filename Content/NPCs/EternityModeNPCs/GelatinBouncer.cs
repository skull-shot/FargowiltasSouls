using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs
{
    public class GelatinBouncer : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_659";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
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
            NPC.CloneDefaults(NPCID.QueenSlimeMinionPink);

            //because they will double dip on expert/master scaling otherwise
            NPC.lifeMax = 160;
            NPC.damage = 50;

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.timeLeft = 60 * 60;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            NPC.scale *= 1.5f;
            NPC.lifeMax *= 1;
        }

        //public override bool CanHitNPC(NPC target) => false;

        //public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanFallThroughPlatforms()
        {
            if (!NPC.HasPlayerTarget)
                return false;
            if (JumpTimer >= JumpDelay - 40)
                return false;
            Player player = Main.player[NPC.target];
            return player.Bottom.Y > NPC.Bottom.Y + 10;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0f;
            modifiers.DisableKnockback();
        }
        public ref float JumpTimer => ref NPC.ai[0];
        public ref float JumpDelay => ref NPC.ai[1];

        public override void AI()
        {
            if (!NPC.AnyNPCs(NPCID.QueenSlimeBoss) || !EModeGlobalNPC.queenSlimeBoss.IsWithinBounds(Main.maxNPCs))
            {
                NPC.active = false;
                return;
            }
            void RandomizeJumpDelay()
            {
                JumpDelay = 60 * 1 + Main.rand.Next(30);
            }

            if (JumpDelay == 0) // initialize
                RandomizeJumpDelay();

            NPC queenSlime = Main.npc[EModeGlobalNPC.queenSlimeBoss];
            NPC.target = queenSlime.target;
            NPC.GravityMultiplier *= 2f;
            NPC.knockBackResist = 0f;

            // increment bounce timer if standing on ground, and bounce when it's time
            if (NPC.velocity.Y == 0)
            {
                NPC.velocity.X *= 0.5f;
                JumpTimer++;
                if (NPC.HasPlayerTarget)
                {
                    Player player = Main.player[NPC.target];
                    // speed up jump frequency if further
                    int extra = (int)((float)Math.Abs(player.Center.X - NPC.Center.X) / 400f);
                    JumpTimer += extra;

                    if (JumpTimer >= JumpDelay)
                    {
                        JumpTimer = 0;

                        SoundEngine.PlaySound(SoundID.Item154, NPC.Center);
                        RandomizeJumpDelay();

                        // calculate arc velocity to player
                        NPC.velocity.Y = -20;
                        // If player is well above me, jump higher
                        if (player.Center.Y < NPC.position.Y + NPC.height - 240)
                        {
                            NPC.velocity.Y *= 1.5f;
                        }

                        int direction = Math.Sign(player.Center.X - NPC.Center.X);
                        int pastPlayer = 400;
                        Vector2 desiredDestination = player.Center + (Vector2.UnitX * pastPlayer * direction);

                        float jumpTime = Math.Abs(2 * NPC.velocity.Y / NPC.gravity);
                        NPC.velocity.X = (desiredDestination.X - NPC.Center.X) / jumpTime;
                        NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -15, 15);
                    }
                }

            }
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
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                NPC.frame.Y = 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 scale = Vector2.One * NPC.scale;
            if (JumpTimer > JumpDelay - 40)
            {
                float modifier = (float)(JumpTimer - (JumpDelay - 40)) / 40f;
                scale.Y *= MathHelper.SmoothStep(1f, 0.5f, modifier);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Bottom - Vector2.UnitY * (scale.Y * NPC.height * 0.5f) - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, scale, effects, 0);
            return false;
        }
    }
}