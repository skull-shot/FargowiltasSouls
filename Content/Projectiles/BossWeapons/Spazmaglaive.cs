using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Projectiles.Masomode;
using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class Spazmaglaive : ModProjectile, IPixelatedPrimitiveRenderer
    {
        bool empowered = false;
        bool hitSomething = false;

        public bool DrawTrail = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spazmaglaive");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(empowered);
            writer.Write(hitSomething);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            empowered = reader.ReadBoolean();
            hitSomething = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.light = 0.4f;
            Projectile.tileCollide = false;
            Projectile.width = 75;
            Projectile.height = 75;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            //Projectile.Kill();
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.ai[2] == 0)
                {
                    Projectile.ai[2] = Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
                }
                Projectile.ai[1]++;
                ++Projectile.localAI[0];
                const int maxTime = 45;
                Vector2 DistanceOffset = new Vector2(950 * (float)Math.Sin(Projectile.localAI[0] * Math.PI / maxTime), 0).RotatedBy(Projectile.velocity.ToRotation());
                DistanceOffset = DistanceOffset.RotatedBy(Projectile.ai[2] - Projectile.ai[2] * Projectile.localAI[0] / (maxTime / 2));
                Projectile.Center = Main.player[Projectile.owner].Center + DistanceOffset;

                if (Projectile.ai[1] >= 5)
                {
                    //Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity * 0.8f, 1 / 60f); ;
                }
                if (Projectile.ai[1] >= 10)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.rotation * 2, 1 / 60f);

            }

            if (Projectile.ai[0] == 1 && !empowered)
            {
                Projectile.extraUpdates = 0;
                Projectile.timeLeft = 2;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center) * 35, 0.2f);

                //kill when back to player
                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 50)
                    Projectile.Kill();

            }

            if (Projectile.ai[0] == 1 && empowered)
            {
                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1200, false, true));
                if (n.Alive())
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(n.Center) * 60, 0.2f);
                }
                else 
                {
                    Projectile.extraUpdates = 0;
                    Projectile.timeLeft = 2;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center) * 25, 0.2f);

                    //kill when back to player
                    if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 30)
                        Projectile.Kill();

                }

                if (Projectile.timeLeft <= 10)
                {
                    empowered = false;
                    
                }

            }

            if (Projectile.ai[0] == 2)
            {
                Projectile.rotation += 0.64f;
                Projectile.velocity *= 0;
                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 200, false, true));
                if (n.Alive())
                {
                    Projectile.velocity += n.velocity;
                    //Projectile.Center = n.Center;

                }

                if (Projectile.timeLeft <= 10)
                {
                    Projectile.ai[0] = 1;
                }
            }

            if (Projectile.ai[0] != 2)
                Projectile.rotation += 0.42f;


            /*const int maxTime = 45;
            Vector2 DistanceOffset = new Vector2(950 * (float)Math.Sin(Projectile.ai[0] * Math.PI / maxTime), 0).RotatedBy(Projectile.velocity.ToRotation());
            DistanceOffset = DistanceOffset.RotatedBy(Projectile.ai[1] - Projectile.ai[1] * Projectile.ai[2] / (maxTime / 2));
            Projectile.Center = Main.player[Projectile.owner].Center + DistanceOffset;
            if (Projectile.ai[0] > maxTime)
                Projectile.Kill();*/

            if (Projectile.ai[0] == ModContent.ProjectileType<Retiglaive>())
            {
                empowered = true;
                Projectile.ai[0] = 0;
            }
            else if (Projectile.ai[0] == ModContent.ProjectileType<Spazmaglaive>())
            {
                Projectile.ai[0] = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Distance(new Vector2(targetHitbox.Center.X, targetHitbox.Center.Y)) < 50; //big circular hitbox because otherwise it misses too often
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 800, false, true));
            if (n.Alive())
            {
                Particle p1 = new SparkParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, 1f, 25);
                Particle p2 = new SparkParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, 1f, 25);
                Particle p3 = new SparkParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, 1f, 25);
                p1.Spawn();
                p2.Spawn();
                p3.Spawn();
            }

            SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
            //return;
    
            if (Projectile.timeLeft >= 10)
                Projectile.ai[0] = 2;

            target.AddBuff(BuffID.CursedInferno, 120);

            /*if (!hitSomething)
            {
                hitSomething = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
                    Vector2 baseVel = Main.rand.NextVector2CircularEdge(1, 1);
                    float ai0 = 78;//empowered ? 120 : 78;
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 newvel = baseVel.RotatedBy(i * MathHelper.TwoPi / 5);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, newvel, ModContent.ProjectileType<SpazmaglaiveExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0, target.whoAmI);
                    }
                    /*if (empowered)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            SoundEngine.PlaySound(SoundID.Item105 with { Pitch = -0.3f }, Projectile.Center);
                            Vector2 newvel = baseVel.RotatedBy(i * MathHelper.TwoPi / 12);
                            int p = Projectile.NewProjectile(target.Center, newvel/2, ModContent.ProjectileType<MechElectricOrbFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, target.whoAmI);
                            if(p < 1000)
                            {
                                Main.Projectile[p].magic = false;
                                Main.Projectile[p].melee = true;
                                Main.Projectile[p].timeLeft = 30;
                                Main.Projectile[p].netUpdate = true;
                            }
                        }
                    }
                }
                Projectile.netUpdate = true;
            }*/
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D SpazmaSaw = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/SpazmarangSaw").Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Rectangle rectangle2 = new(0, y3, SpazmaSaw.Width, num156);
            Vector2 origin22 = rectangle2.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            float scale = 1;
            scale = MathHelper.Lerp(Projectile.scale, 2f, 0.5f);

            float opacity = 1;
            opacity = MathHelper.Lerp(0, 1, 0.1f);

            if (empowered)
            {
                DrawTrail = true;
            }


            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.33f)
            {
                int max0 = Math.Max((int)i - 1, 0);
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                if (i < 3)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Main.EntitySpriteDraw(texture2D13, center + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27 * 0.4f, Projectile.oldRot[(int)i], origin2, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            if (Projectile.ai[0] <= 2)
            {
                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            }


            if (Projectile.ai[0] == 2)
            {
                Main.EntitySpriteDraw(SpazmaSaw, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), -Projectile.rotation, origin22, Projectile.scale * 1.3f, SpriteEffects.None, 0);
            }
            if (Projectile.ai[1] >= 35 || Projectile.ai[0] == 1)
            {
                Main.spriteBatch.Draw(SpazmaSaw, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin22, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.width * 0.8f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkGreen, Color.Green, completionRatio);
        }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.Techno1Noise.Value);
            if (DrawTrail)
            {
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 25);
            }
            
        }
    }
}