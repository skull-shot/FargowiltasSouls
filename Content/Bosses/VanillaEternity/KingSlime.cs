using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.NPCs;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles.Masomode.Bosses.KingSlime;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class KingSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.KingSlime);

        public bool DroppedSummon;

        public int DeathTimer = -1;

        public NPC NPC;
        public Player Target => Main.player[NPC.target];
        public ref float Cycle => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float Phase => ref NPC.ai[2];
        public ref float Timer => ref NPC.ai[3];
        public enum States
        {
            Spawn,
            JumpCycle,
            Burst,
            Teleport
        };
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(DeathTimer);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            DeathTimer = binaryReader.Read7BitEncodedInt();
        }
        public override bool SafePreAI(NPC npc)
        {
            NPC = npc;
            NPC.noGravity = false;

            if (DeathTimer >= 0)
            {
                DeathAnimation(npc);
                if (++DeathTimer >= 300)
                {
                    npc.life = 0;
                    npc.dontTakeDamage = false;
                    npc.checkDead();
                }
                return false;
            }


            EModeGlobalNPC.slimeBoss = npc.whoAmI;

            // despawn
            int maxDist = 3000;
            if (Main.player[npc.target].dead || Vector2.Distance(npc.Center, Main.player[npc.target].Center) > maxDist)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || Vector2.Distance(npc.Center, Main.player[npc.target].Center) > maxDist)
                {
                    return true; // run vanilla despawn code
                }
            }

            // Drop summon
            EModeUtils.DropSummon(npc, "SlimyCrown", NPC.downedSlimeKing, ref DroppedSummon);

            // Scaling (from vanilla)
            float num255 = (float)npc.life / (float)npc.lifeMax;
            num255 = num255 * 0.5f + 0.75f;
            float num236 = 1f;
            float num237 = 1f;
            float num238 = 2f;
            if (Main.getGoodWorld)
            {
                num238 -= 1f - (float)npc.life / (float)npc.lifeMax;
                num237 *= num238;
            }
            num255 *= num236;
            num255 *= num237;
            if (num255 != npc.scale)
            {
                npc.position.X += npc.width / 2;
                npc.position.Y += npc.height;
                npc.scale = num255;
                npc.width = (int)(98f * npc.scale);
                npc.height = (int)(92f * npc.scale);
                npc.position.X -= npc.width / 2;
                npc.position.Y -= npc.height;
            }

            int state = (int)State;
            switch ((States)state)
            {
                case States.Spawn:
                    {
                        if (NPC.velocity.Y == 0)
                        {
                            Timer++;
                            if (Timer > 40)
                            {
                                State = (int)States.JumpCycle;
                                Timer = 0;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                    break;
                case States.JumpCycle:
                    {
                        if (Cycle > 2)
                        {
                            Cycle = 0;
                            NPC.netUpdate = true;
                        }
                        if (Cycle < 2) // normal jump
                        {
                            NormalJump();
                        }
                        else if (Cycle == 2) // big jump
                        {
                            BigJump();
                        }
                    }
                    break;
            }
            return false;
        }
        public void NormalJump()
        {
            float xDir = NPC.HorizontalDirectionTo(Target.Center);
            if (Timer == 0) // before jump
            {
                if (NPC.velocity.Y == 0) // grounded
                {
                    float x = MathF.Abs(Target.Center.X - NPC.Center.X) / 1200;
                    if (x > 1)
                        x = 1;
                    int side = 17;
                    int up = 30;
                    if (Cycle == 0)
                    {
                        side = 10;
                        up = 22;
                    }
                    NPC.velocity.X = xDir * side * x;
                    NPC.velocity.Y = -up;
                    Timer = 1;
                    NPC.netUpdate = true;
                }
            }
            else if (Timer == 1) // during jump
            {
                float threshold = 400;
                float distanceFactor = MathF.Abs(Target.Center.X - NPC.Center.X) / threshold;
                if (NPC.velocity.X.NonZeroSign() != xDir || distanceFactor > 1)
                    NPC.velocity.X += xDir * 0.7f;
                else
                    NPC.velocity.X += xDir * 0.7f * distanceFactor;
                int cap = 17;
                if (Cycle == 0)
                    cap = 10;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -cap, cap);
                NPC.noGravity = NPC.velocity.Y > 0;
                if (NPC.velocity.Y == 0)
                {
                    NPC.velocity.X = 0;
                    Timer = 2;
                    NPC.netUpdate = true;
                    // spike burst
                    
                    for (int j = -1; j < 2; j += 2)
                    {
                        Vector2 vel = new(j * 7, -10);
                        for (int i = 0; i < 3; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitX * j * Main.rand.NextFloat(0.7f, 1.3f) * NPC.width * NPC.scale / 4, vel + Main.rand.NextVector2Square(-1f, 1f) * 4f,
                                ModContent.ProjectileType<SlimeSpike>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                        }
                    }
                    
                }
                else
                {
                    if (NPC.velocity.Y < 20)
                        NPC.velocity.Y += 1;
                }
                    
            }
            else // after jump
            {
                Timer++;
                float delay = 35;
                if (WorldSavingSystem.MasochistModeReal)
                {
                    delay -= 20;
                }
                delay += NPC.GetLifePercent() * 35;
                if (Timer > delay)
                {
                    IncrementCycle();
                    return;
                }
            }
        }
        public void BigJump()
        {
            float xDir = NPC.HorizontalDirectionTo(Target.Center);
            if (Timer == 0) // before jump
            {
                if (NPC.velocity.Y == 0) // grounded
                {
                    NPC.velocity.X = xDir * 18;
                    NPC.velocity.Y = -38;
                    Timer = 1;
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/VanillaEternity/KingSlime/KSJump"), NPC.Center);
                    NPC.netUpdate = true;
                }
            }
            else if (Timer >= 1) // during jump
            {
                float threshold = 200;
                float xDist = MathF.Abs(Target.Center.X - NPC.Center.X);
                if (Timer == 1 && (xDist < threshold || NPC.velocity.Y > 0))
                    Timer = 2;
                if (Timer > 1)
                {
                    Timer++;
                    NPC.velocity.X *= 0.96f;
                }
                    
                NPC.noGravity = NPC.velocity.Y > 0;
                if (NPC.velocity.Y == 0)
                {
                    NPC.velocity.X = 0;
                    Timer = -1;
                    NPC.netUpdate = true;

                    // spike burst
                    for (int j = -1; j < 2; j += 2)
                    {
                        Vector2 vel = new(j * 10, -4);
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitX * j * Main.rand.NextFloat(0.7f, 1.3f) * NPC.width * NPC.scale / 4, vel + Main.rand.NextVector2Square(-1f, 1f) * 2f,
                                ModContent.ProjectileType<SlimeSpike>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                        }
                    }

                }
                else
                {
                    float gravity = Timer == 1 ? 0.5f : 1.5f;
                    if (NPC.velocity.Y < 20)
                        NPC.velocity.Y += gravity;
                }

            }
            else // after jump
            {
                Timer--;
                float delay = 35;
                if (WorldSavingSystem.MasochistModeReal)
                {
                    delay -= 20;
                }
                delay += NPC.GetLifePercent() * 35;
                if (Timer < -delay)
                {
                    IncrementCycle();
                    return;
                }
            }
        }
        public void IncrementCycle()
        {
            Timer = 0;
            Cycle++;
            NPC.netUpdate = true;
        }
        public override bool? CanFallThroughPlatforms(NPC npc)
        {
            if (!npc.HasPlayerTarget)
                return base.CanFallThroughPlatforms(npc);
            return (Main.player[npc.target].Top.Y > npc.Bottom.Y + 30) ? true : null;
        }

        public override bool CheckDead(NPC npc)
        {
            if (DeathTimer != -1)
                return true;

            // Dont do the anim if mutant already exists
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss.MutantBoss>())
                || (ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC mutant) && NPC.AnyNPCs(mutant.Type)) || !SoulConfig.Instance.BossRecolors)
            {
                return true;
            }

            if (WorldSavingSystem.HaveForcedMutantFromKS)
                return true;

            npc.life = 1;
            npc.active = true;

            // remove normal crown gore (manually spawned later)
            foreach (Gore gore in Main.gore.Where(g => g.active && g.type == GoreID.KingSlimeCrown))
                gore.active = false;
            DeathTimer++;
            npc.dontTakeDamage = true;
            FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
            npc.netUpdate = true;

            return false;
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck
                && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss.MutantBoss>())
                && ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC mutant) && !NPC.AnyNPCs(mutant.Type))
            {   
                
                // manual gore spawn
                Gore.NewGore(npc.GetSource_FromThis(), npc.Center, -15 * Vector2.UnitY, GoreID.KingSlimeCrown);

                int n = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<ReleasedMutant>());
                if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 60);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 7);
            LoadGore(recolor, 734);
            LoadExtra(recolor, 39);

            LoadSpecial(recolor, ref TextureAssets.Ninja, ref FargowiltasSouls.TextureBuffer.Ninja, "Ninja");
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool resprite = WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors;
            if (!resprite)
                return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
            // Draw the Ninja (Mutant).
            var ninjaOffset = new Vector2(-npc.velocity.X * 2f, -npc.velocity.Y);
            var ninjaRotation = npc.velocity.X * 0.05f;
            
            switch (npc.frame.Y)
            {
                case 120:
                    ninjaOffset.Y += 2f;
                    break;

                case 360:
                    ninjaOffset.Y -= 2f;
                    break;

                case 480:
                    ninjaOffset.Y -= 6f;
                    break;
            }
            
            spriteBatch.Draw(TextureAssets.Ninja.Value, new Vector2(npc.position.X - screenPos.X + npc.width / 2f + ninjaOffset.X, npc.position.Y - screenPos.Y + npc.height / 2f + ninjaOffset.Y), new Rectangle(0, 0, TextureAssets.Ninja.Width(), TextureAssets.Ninja.Height()), drawColor, ninjaRotation, TextureAssets.Ninja.Size() / 2f, 1f, SpriteEffects.None, 0f);
            
            // We have to manually set drawing to immediate mode when rendering
            // the NPC normally.  Bestiary icons don't have this requirement.
            if (!npc.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            }

            // Can also use npc.type instead.
            var ksTexture = TextureAssets.Npc[NPCID.KingSlime].Value;
            
            // Render KS through DrawData since we need to apply a game shader.
            var frameCount = Main.npcFrameCount[npc.type];
            var frameVertical = npc.frame.Y / npc.frame.Height;
            var frame = ksTexture.Frame(1, frameCount, 0, frameVertical);
            frame.Inflate(0, -2);
            
            var drawData = new DrawData(ksTexture, npc.Bottom - screenPos + new Vector2(0f, 2f), frame, drawColor /*with { A = 200 }*/, npc.rotation, frame.Size() * new Vector2(0.5f, 1f), npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            GameShaders.Misc["FargowiltasSouls:KingSlime"].Apply(drawData);
            drawData.Draw(spriteBatch);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            
            if (!npc.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            
            // Render the crown normally without the shader.
            var crownTexture = TextureAssets.Extra[39].Value;
            var center = npc.Center;

            var yOffset = (npc.frame.Y / (ksTexture.Height / Main.npcFrameCount[NPCID.KingSlime])) switch
            {
                0 => 2f,
                1 => -6f,
                2 => 2f,
                3 => 10f,
                4 => 2f,
                5 => 0f,
                _ => 0f,
            };

            var spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            center.Y += npc.gfxOffY - (70f - yOffset) * npc.scale;
            spriteBatch.Draw(crownTexture, center - screenPos, null, Color.White, 0f, crownTexture.Size() / 2f, 1f, spriteEffects, 0f);
            return false;
        }

        public void DeathAnimation(NPC npc)
        {
            Particle p;
            float scaleMult;
            int screenshake = 3;
            npc.velocity.X *= 0.9f;
            Vector2 mutantEyePos = npc.Center + new Vector2(-5f, -12f);
            // Dust
            if (Main.rand.NextBool(5))
            {          
                SoundEngine.PlaySound(npc.HitSound, npc.Center);
            }
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            Dust.NewDust(npc.TopLeft, npc.width, npc.height, DustID.t_Slime);

            if (DeathTimer == 100 || DeathTimer == 200 || DeathTimer == 250)
            {
                screenshake += 2;
                FargoSoulsUtil.ScreenshakeRumble(screenshake);
                SoundEngine.PlaySound(FargosSoundRegistry.MutantSword with { Volume = 0.6f}, npc.Center);
            }

            // initial charge up
            if (DeathTimer >= 180 && DeathTimer < 270)
            {
                Vector2 pos = npc.Center + 5 * Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                scaleMult = (DeathTimer - 180) / 23f;
                p = new SparkParticle(pos, Vector2.UnitX.RotatedBy((pos - npc.Center).ToRotation()), Color.Teal, scaleMult * 0.1f, 10);
                p.Spawn();
            }

            if (DeathTimer >= 270)
            {
                // eye glow
                scaleMult = (DeathTimer - 270) / 14f;
                p = new SparkParticle(mutantEyePos, Vector2.UnitY, Color.Teal, 1.5f, 120);
                p.Scale *= scaleMult;
                p.Spawn();
                p = new SparkParticle(mutantEyePos, Vector2.UnitX, Color.Teal, 1.5f, 120);
                p.Scale *= scaleMult;
                p.Spawn();

                // explosions
                if (DeathTimer % 5 == 0)
                {
                    if (FargoSoulsUtil.HostCheck) 
                    {
                        Vector2 spawnPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height));
                        int type = ModContent.ProjectileType<MutantBombSmall>();
                        Projectile proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        proj.scale *= 0.43f * scaleMult;
                    }
                    SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                    FargoSoulsUtil.ScreenshakeRumble((DeathTimer - 270) / 15f);
                }
            }
            // grand finale
            if (DeathTimer == 298)
            {
                FargoSoulsUtil.ScreenshakeRumble(7f);
                SoundEngine.PlaySound(FargosSoundRegistry.MutantKSKill, npc.Center);
            }
        }
    }
    /*
    public class KingSlimeMinionRemovalHack : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher()
        {
            return new();
        }
        bool KILL = false;
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC sourceNPC && sourceNPC.type == NPCID.KingSlime)
            {
                Main.NewText("yeetus deletus");
                DELETE(npc);
                KILL = true;
            }
        }
        public override bool SafePreAI(NPC npc)
        {
            if (KILL)
            {
                DELETE(npc);
            }
            return base.SafePreAI(npc);
        }
        public override void SafePostAI(NPC npc)
        {
            if (KILL)
            {
                DELETE(npc);
            }
            base.SafePostAI(npc);
        }
        void DELETE(NPC npc)
        {
            npc.life = 0;
            npc.HitEffect();
            npc.checkDead();
            npc.active = false;
            npc.timeLeft = 0;
            npc = null;
        }
    }
    */
}
