using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.TrojanSquirrel
{
    public class TrojanSquirrelArms : TrojanSquirrelLimb
    {
        public LoopedSoundInstance? Loop;
        int looptimer;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.lifeMax = 450;
            NPC.DeathSound = FargosSoundRegistry.TrojanCannonDeath;
            NPC.width = baseWidth = 114;
            NPC.height = baseHeight = 64;
        }

        public override void AI()
        {
            //return;
            base.AI();

            if (body == null)
                return;

            NPC.velocity = Vector2.Zero;
            NPC.target = body.target;
            NPC.direction = NPC.spriteDirection = body.direction;
            NPC.Center = body.Bottom + new Vector2(18f * NPC.direction, -105f) * body.scale;
            if (NPC.ai[0] != 1 && Loop?.HasLoopSoundBeenStarted == true)
            {
                looptimer = 0;
                Loop?.Stop();
            }

            switch ((int)NPC.ai[0])
            {
                case 0:
                    if (body.ai[0] == 0 && body.localAI[0] <= 0)
                    {
                        NPC.ai[1] += WorldSavingSystem.EternityMode ? 1.5f : 1f;

                        if (body.dontTakeDamage)
                            NPC.ai[1] += 1f;

                        int threshold = 360;

                        //structured like this so body gets priority first
                        int stallPoint = threshold - 30;
                        if (NPC.ai[1] > stallPoint)
                        {
                            TrojanSquirrel squirrel = body.As<TrojanSquirrel>();
                            if (squirrel.head != null && squirrel.head.ai[0] != 0f) //wait if other part is attacking
                                NPC.ai[1] = stallPoint;
                        }

                        if (NPC.ai[1] > threshold && Math.Abs(body.velocity.Y) < 0.05f)
                        {
                            //dont attack unless player is in 90 degree cone in front of squrrl
                            float baseAngle = NPC.direction > 0 ? 0f : MathHelper.Pi;
                            if (Math.Abs(MathHelper.WrapAngle(NPC.SafeDirectionTo(Main.player[NPC.target].Center).ToRotation() - baseAngle)) > MathHelper.PiOver4)
                            {
                                NPC.ai[1] = stallPoint;
                            }
                            else
                            {
                                NPC.ai[0] = 1 + NPC.ai[2];
                                NPC.ai[1] = 0;
                                if (Main.expertMode)
                                    NPC.ai[2] = NPC.ai[2] == 0 ? 1 : 0;
                                NPC.netUpdate = true;

                                body.localAI[3] = Math.Sign(body.SafeDirectionTo(Main.player[body.target].Center).X);
                                body.netUpdate = true;
                            }
                        }
                    }
                    break;

                case 1: //chains
                    {
                        if (++looptimer >= 90)
                        {
                            Loop ??= LoopedSoundManager.CreateNew(FargosSoundRegistry.TrojanHookLoop with { Volume = 0.5f }, () =>
                            {
                                return NPC == null || !NPC.active || NPC.ai[0] != 1;
                            });

                            Loop?.Update(NPC.Center);

                            if (Loop?.HasBeenStopped == true && Loop?.HasLoopSoundBeenStarted == true)
                            {
                                Loop?.Restart();
                            }
                        }

                        int start = 90;
                        if (WorldSavingSystem.EternityMode)
                            start -= 30;
                        if (WorldSavingSystem.MasochistModeReal)
                            start -= 30;
                        int end = 310;

                        int teabagInterval = start / (WorldSavingSystem.MasochistModeReal ? 3 : 2);

                        if (NPC.ai[1] < start) //better for animation
                        {
                            body.velocity.X *= 0.9f;
                            if (NPC.ai[1] <= 1)
                                SoundEngine.PlaySound(FargosSoundRegistry.TrojanHookTelegraph, NPC.Center);
                        }

                        NPC.ai[1]++;

                        //to help animate body
                        NPC.ai[3] = NPC.ai[1] < start && NPC.ai[1] % teabagInterval < teabagInterval / 2 ? 1 : 0;

                        if (NPC.ai[1] > start && NPC.ai[1] < end && NPC.ai[1] % (body.dontTakeDamage || WorldSavingSystem.MasochistModeReal ? 40 : 70) == 0)
                        {
                            Vector2 pos = GetShootPos();

                            float baseAngle = NPC.direction > 0 ? 0f : MathHelper.Pi;
                            float angle = NPC.SafeDirectionTo(Main.player[NPC.target].Center).ToRotation();
                            if (Math.Abs(MathHelper.WrapAngle(angle - baseAngle)) > MathHelper.PiOver2)
                                angle = MathHelper.PiOver2 * Math.Sign(angle);

                            if (FargoSoulsUtil.HostCheck)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, 8f * angle.ToRotationVector2(), ModContent.ProjectileType<TrojanHook>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                        }

                        if (NPC.ai[1] > 300 && FargoSoulsUtil.HostCheck && Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<TrojanHook>()] <= 0)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;

                            body.localAI[3] = 0;
                            body.netUpdate = true;
                        }
                    }
                    break;

                case 2: //snowballs
                    {
                        NPC.ai[1]++;

                        int start = 70;
                        int end = 340;
                        if (WorldSavingSystem.EternityMode)
                        {
                            start -= 30;
                            end -= 30;
                        }
                        if (WorldSavingSystem.MasochistModeReal)
                            end -= 60;

                        body.velocity.X *= 0.98f;

                        if (NPC.ai[1] == 10)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 pos = GetShootPos();
                                SoundEngine.PlaySound(FargosSoundRegistry.TrojanGunStartup, pos);
                                for (int j = 0; j < 20; j++)
                                {
                                    int d = Dust.NewDust(pos, 0, 0, DustID.SnowBlock, Scale: 3f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].velocity *= 4f;
                                    Main.dust[d].velocity.X += NPC.direction * Main.rand.NextFloat(6f, 24f);
                                }
                            }
                        }

                        if (NPC.ai[1] > start && NPC.ai[1] % 4 == 0)
                        {
                            SoundEngine.PlaySound(FargosSoundRegistry.Minigun, GetShootPos());
                            if (NPC.ai[1] % 8 == 0)
                            {
                                Vector2 pos = GetShootPos();

                                SoundEngine.PlaySound(FargosSoundRegistry.TrojanSnowball, pos);

                                float ratio = (NPC.ai[1] - start) / (end - start);

                                Vector2 target = NPC.Center;
                                target.X += Math.Sign(NPC.direction) * (WorldSavingSystem.EternityMode ? 1800f : 1200f) * ratio; //gradually targets further and further
                                                                                                                                 //target.Y -= 8 * 16;
                                target += Main.rand.NextVector2Circular(16, 16);
                                const float gravity = 0.5f;
                                float time = 45f;
                                Vector2 distance = target - pos;
                                distance.X /= time;
                                distance.Y = distance.Y / time - 0.5f * gravity * time;
                                if (FargoSoulsUtil.HostCheck)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, distance, ModContent.ProjectileType<TrojanSnowball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, gravity);
                            }
                            NPC.ai[1] += NPC.ai[1] > end / 3 ? NPC.ai[1] > end * (2 / 3) ? 3 : 1 : 0;
                        }

                        if (NPC.ai[1] > end)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;

                            body.localAI[3] = 0;
                            body.netUpdate = true;
                        }
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {   
            //74 is the width of each X Frame.
            NPC.frame.X = 0;
            if (++NPC.frameCounter >= 7)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = 0;
            }

            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                NPC.frame.Y = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = new Rectangle(NPC.frame.X, NPC.frame.Y, NPC.frame.Width / 3, NPC.frame.Height);
            Vector2 origin2 = rectangle.Size() / 2f;
            //NPC.direction = 1;


            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (body != null)
                Main.EntitySpriteDraw(texture2D13, body.Center - screenPos + new Vector2(NPC.direction < 0 ? 20f : -2f, NPC.gfxOffY - 24 * NPC.scale), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            return false;
        }

        private Vector2 GetShootPos()
        {
            NPC.localAI[0] = NPC.localAI[0] == 0 ? 1 : 0;

            Vector2 pos = NPC.Bottom;
            pos.X += NPC.width / 2f * NPC.direction;
            pos.Y -= 16 * NPC.scale;

            pos.X -= (NPC.localAI[0] == 0 ? 10 : 48) * NPC.direction * NPC.scale;

            return pos;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Loop?.Stop();
                for (int i = 8; i <= 10; i++)
                {
                    Vector2 pos = Main.rand.NextVector2FromRectangle(NPC.Hitbox);
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TrojanSquirrelGore{i}").Type, NPC.scale);
                }
            }
        }
    }
}
