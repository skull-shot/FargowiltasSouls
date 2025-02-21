using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Common.Graphics.Particles;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class BigStinger : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Big Stinger");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.HornetStinger);
            AIType = ProjectileID.Bullet;
            Projectile.minion = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 240;
            Projectile.width = 22;
            Projectile.height = 38;
            Projectile.extraUpdates = 2;

            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frame += 1;
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
                


            //stuck in enemy
            if (Projectile.ai[0] == 1)
            {
                Projectile.extraUpdates = 0;
                Projectile.aiStyle = -1;

                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

                int secondsStuck = 15;
                bool kill = false;

                Projectile.localAI[0] += 1f;

                int npcIndex = (int)Projectile.ai[1];
                if (Projectile.localAI[0] >= 60 * secondsStuck)
                {
                    kill = true;
                }
                else if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
                {
                    kill = true;
                }
                else if (Main.npc[npcIndex].active && !Main.npc[npcIndex].dontTakeDamage)
                {
                    Projectile.Center = Main.npc[npcIndex].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcIndex].gfxOffY;
                }
                else
                {
                    kill = true;
                }

                if (kill)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.position += Projectile.velocity * 0.5f;

                //dust from stinger
                if (Main.rand.NextBool())
                {
                    int num92 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs);
                    Main.dust[num92].noGravity = true;
                    Main.dust[num92].velocity *= 0.5f;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.type == Projectile.type && p != Projectile && Projectile.Hitbox.Intersects(p.Hitbox))
                {
                    target.SimpleStrikeNPC(Projectile.damage / 2, 0, noPlayerInteraction: true, crit: false);
                    // target.StrikeNPC(Projectile.damage / 2, 0, 0, true); //normal damage but looks like a crit ech
                    target.AddBuff(BuffID.Venom, 600);
                    DustRing(p, 24);
                    p.Kill();

                    SoundEngine.PlaySound(SoundID.Item27 with { Pitch = -0.4f }, Projectile.Center);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        const float range = 220f; //stinger spray
                        const int time = 22;
                        const int max = 8;
                        Vector2 baseVel = Vector2.UnitX.RotatedByRandom(Math.PI * 2 / max);
                        for (int j = 0; j < max; j++)
                        {
                            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), p.Center, range / time * baseVel.RotatedBy(Math.PI * 2 / max * j),
                                ModContent.ProjectileType<Stinger>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                            if (proj != Main.maxProjectiles)
                                Main.projectile[proj].timeLeft = time;
                        }

                        Vector2 spawn = Main.screenPosition;
                        if (Main.player[Projectile.owner].direction < 0)
                            spawn.X += Main.screenWidth;
                        spawn.Y += Main.rand.Next(Main.screenHeight);
                        Vector2 targetPos = target.position;
                        targetPos.X += Main.rand.NextFloat(target.width);
                        targetPos.Y += Main.rand.NextFloat(target.height);
                        Vector2 vel = 22f * Vector2.Normalize(targetPos - spawn);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, vel, ModContent.ProjectileType<BigSting22>(), Projectile.damage / 2, Projectile.knockBack * 2f, Projectile.owner, -1);
                    }
                    break;
                }
            }

            Projectile.ai[0] = 1;
            Projectile.ai[1] = target.whoAmI;
            Projectile.aiStyle = -1;
            Projectile.velocity = (Main.npc[target.whoAmI].Center - Projectile.Center) * 1f; //distance it sticks out
            Projectile.damage = 0;
            Projectile.timeLeft = 300;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 1)
                Projectile.velocity = Vector2.Zero; //for the dust

            for (int i = 0; i < 10; i++)
            {
                /*int num92 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 0.9f);
                Main.dust[num92].noGravity = true;
                Main.dust[num92].velocity *= 0.25f;
                Main.dust[num92].fadeIn = 1.3f;*/
                Particle p = new RectangleParticle(Projectile.Center, Projectile.velocity * 0.7f + Main.rand.NextVector2Circular(15, 45), Color.Yellow, Main.rand.NextFloat(0.1f, 0.25f), 50, true);
                p.Spawn();
            }
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Bottom, Projectile.velocity, ModContent.Find<ModGore>(Mod.Name, "BigStingerGore").Type, Projectile.scale);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }

        private static void DustRing(Projectile proj, int max)
        {
            //dust
            /*for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + proj.Center;
                Vector2 vector7 = vector6 - proj.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.GemTopaz, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }*/
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Color color25 = Lighting.GetColor((int)(Projectile.position.X + Projectile.width * 0.5) / 16, (int)((Projectile.position.Y + Projectile.height * 0.5) / 16.0));
            Texture2D texture2D3 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y3 = num156 * Projectile.frame;
            Rectangle rectangle = new(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 7;
            int num159 = 0;
            float num160 = 0f;

            int num161 = num159;
            /*while (Projectile.ai[0] != 1 && num161 < num157) //doesnt draw trail while stuck in enemy
            {
                Color color26 = color25;
                color26 = Projectile.GetAlpha(color26);
                float num164 = num157 - num161;
                color26 *= num164 / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[num161];
                float num165 = Projectile.rotation;
                SpriteEffects effects = spriteEffects;
                Main.EntitySpriteDraw(texture2D3, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + Projectile.rotation * num160 * (num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, Projectile.scale * 0.8f, effects, 0);
                num161++;
            }*/

            Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/BigStingerGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition + new Vector2(0.5f, Projectile.gfxOffY - 0.1f), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Orange, Projectile.rotation, origin2, Projectile.scale * 1.1f * Main.cursorScale, spriteEffects, 0);

            Color color29 = Projectile.GetAlpha(color25);
            Main.EntitySpriteDraw(texture2D3, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color29, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.width * 0.8f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkOrange, Color.Orange, completionRatio);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.StingerTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.ColorNoiseMap.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 25);
        }
    }
}