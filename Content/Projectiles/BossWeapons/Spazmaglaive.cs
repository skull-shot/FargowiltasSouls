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
    public class Spazmaglaive : ModProjectile
    {

        public bool DrawTrail = false;
        public Vector2 mousePos = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spazmaglaive");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
                mousePos = buffer;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.light = 0.4f;
            Projectile.tileCollide = false;
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            //Projectile.Kill();
            /*if (Projectile.ai[0] == 0)
            {
                if (Projectile.ai[2] == 0)
                {
                    Projectile.ai[2] = Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
                }
                Projectile.ai[1]++;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity * 0.6f, 1 / 10f);
                /*++Projectile.localAI[0];
                const int maxTime = 45;
                Vector2 DistanceOffset = new Vector2(950 * (float)Math.Sin(Projectile.localAI[0] * Math.PI / maxTime), 0).RotatedBy(Projectile.velocity.ToRotation());
                DistanceOffset = DistanceOffset.RotatedBy(Projectile.ai[2] - Projectile.ai[2] * Projectile.localAI[0] / (maxTime / 2));
                Projectile.Center += Main.player[Projectile.owner].Center;/
                int maxtime = empowered ? 15 : 30;

                if (Projectile.ai[1] >= maxtime)
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
                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2400, false, true));
                if (n.Alive())
                {   
                    if (hitSomething == false)
                    {
                        Projectile.timeLeft = 60;
                    }
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(n.Center) * 60, 0.2f);
                }
                else 
                {
                    Projectile.extraUpdates = 0;
                    Projectile.timeLeft = 2;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center) * 25, 1 / 60f);

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
            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;
            }

            if (Main.LocalPlayer.channel)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(mousePos) * 35, 0.2f);

                Projectile.timeLeft = 2;
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

            Projectile.rotation += 0.62f;

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
                float scale = Main.rand.NextFloat(0.25f, 0.35f);
                Particle p1 = new RectangleParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, scale, 25, true);
                Particle p2 = new RectangleParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, scale, 25, true);
                Particle p3 = new RectangleParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, scale, 25, true);
                p1.Spawn();
                p2.Spawn();
                p3.Spawn();
            }

            SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
            Projectile.velocity += Vector2.Normalize(n.Center - Projectile.Center) * 35;
            //return;

            if (Projectile.timeLeft >= 10)
                Projectile.ai[0] = 2;

            target.AddBuff(BuffID.CursedInferno, 120);

            
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

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}