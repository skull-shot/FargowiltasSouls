using Fargowiltas;
using Fargowiltas.Content.Projectiles;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs
{
    public class GelatinSubject : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_660";
        public int ContactDamageTimer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gelatin Subject");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.QueenSlimeMinionPurple];
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type] = NPCID.Sets.SpecificDebuffImmunity[NPCID.QueenSlimeBoss];

            this.ExcludeFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.QueenSlimeMinionPurple);
            AIType = 0;
            NPC.aiStyle = -1;

            //because they will double dip on expert/master scaling otherwise
            NPC.lifeMax = 75;
            NPC.damage = 50;

            // this isn't right but makes the hitboxes feel better
            NPC.width /= 2;
            NPC.height /= 2;

            NPC.lifeMax *= 10;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.scale *= 1.5f;
            NPC.width = NPC.height = (int)(NPC.height * 0.9);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }
        public ref float AIVariant => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float Index => ref NPC.ai[2];
        public ref float Height => ref NPC.ai[3];
        public override void AI()
        {
            if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss)
                && !NPC.AnyNPCs(NPCID.QueenSlimeBoss))
            {
                NPC.active = false;
                return;
            }

            const float IdleAccel = 0.025f;
            foreach (NPC n in Main.npc.Where(n => n.active && n.type == NPC.type && n.whoAmI != NPC.whoAmI && NPC.Distance(n.Center) < NPC.width))
            {
                NPC.velocity.X += IdleAccel * (NPC.Center.X < n.Center.X ? -1 : 1);
                NPC.velocity.Y += IdleAccel * (NPC.Center.Y < n.Center.Y ? -1 : 1);
                n.velocity.X += IdleAccel * (n.Center.X < NPC.Center.X ? -1 : 1);
                n.velocity.Y += IdleAccel * (n.Center.Y < NPC.Center.Y ? -1 : 1);
            }
            NPC parent = Main.npc[EModeGlobalNPC.queenSlimeBoss];

            // share healthbar
            if (FargoSoulsUtil.HostCheck)
            {
                if (NPC.lifeMax != parent.lifeMax)
                    NPC.life = NPC.lifeMax = parent.lifeMax;
                NPC.life = parent.life = Math.Min(NPC.life, parent.life);
            }

            switch ((int)AIVariant)
            {
                case 0: // idle a bit, then dash at qs to reform
                    {
                        float startTime = 60;
                        Timer++;
                        if (Timer < startTime)
                        {
                            NPC.velocity *= 0.93f;
                        }
                        else
                        {
                            ReturnToSlime(Timer);
                        }
                    }
                    break;
                case 1: //rainer
                    {
                        if (Timer >= 0)
                        {
                            Timer++;
                            // do the rain thing
                            if (!parent.HasPlayerTarget)
                            {
                                return;
                            }
                            Player player = Main.player[parent.target];

                            float minDistance = 350;
                            if (Timer < 70)
                                minDistance = 500;
                            if (player.Center.Y < Height + minDistance)
                                Height = MathHelper.Lerp(Height, player.Center.Y - Math.Min(500, minDistance), 0.1f);

                            float x = parent.Center.X;
                            float interval = WorldSavingSystem.MasochistModeReal ? 220 : 300;
                            float divisor = WorldSavingSystem.MasochistModeReal ? 130 : 150;
                            x += Index * 300;
                            x += MathF.Sin(MathF.Tau * Timer / divisor) * 200;
                            float y = Height;
                            float speedMod = 1f + Timer / 60;
                            if (speedMod > 5)
                                speedMod = 5;
                            Vector2 pos = new(x, y);
                            Movement(pos, speedMod);

                            if (Timer > 70 && NPC.Distance(pos) < 30 && Timer % 5 == 0)
                            {
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, 8f * Vector2.UnitY,
                                      ProjectileID.QueenSlimeMinionBlueSpike, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                }
                            }
                        }
                        else
                        {
                            Timer--;
                            ReturnToSlime(-Timer * 2);
                        }
                    }
                    break;
            }
            void ReturnToSlime(float timer) // by dm dokuro
            {
                if (NPC.Distance(parent.Center) > 70)
                {
                    float speedMod = 0.3f + timer / 80;
                    Movement(parent.Center, speedMod);
                }
                else
                {
                    NPC.velocity = (parent.Center - NPC.Center) * 0.15f;
                    NPC.scale *= 0.92f;
                    for (int i = 0; i < 3; i++)
                    {
                        int radius = 50;
                        QueenSlime.QSDust(NPC.Center - Vector2.One * radius, 2 * radius, 2 * radius, 0, 0);
                    }
                    if (NPC.scale < 0.1f)
                    {
                        NPC.active = false;
                        return;
                    }
                }
            }
        }
        public void Movement(Vector2 targetPos, float speedModifier = 1.2f)
        {
            float accel = 1f * speedModifier;
            float decel = 1.5f * speedModifier;
            float resistance = NPC.velocity.Length() * accel / (22f * speedModifier);
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, targetPos, NPC.velocity, accel - resistance, decel + resistance);
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage /= 5;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }

        public override bool CheckActive()
        {
            return false;
        }

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
                //SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 0.75f;
                }

                for (int i = 0; i < 2; i++)
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity / 2, 1260, NPC.scale);
            }
        }

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
            if (!Terraria.GameContent.TextureAssets.Npc[NPCID.QueenSlimeMinionPurple].IsLoaded)
                return false;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPCID.QueenSlimeMinionPurple].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection < 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            GameShaders.Misc["HallowBoss"].Apply(new Terraria.DataStructures.DrawData?());

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }

    }
}