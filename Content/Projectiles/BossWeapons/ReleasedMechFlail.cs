using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Common.Graphics.Particles;
using Luminance.Core.Graphics;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class ReleasedMechFlail : ModProjectile
    {

        private float speed;
        private int DashTimer = 0;
        private int DashAmount = 0;
        private int LatchTimer = 0;

        private static Asset<Texture2D> chainTexture;

        private const string ChainTexturePath = "FargowiltasSouls/Content/Projectiles/BossWeapons/MechFlailChain";
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/MechFlail";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("MechEyeProjectile");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
        }

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 74;
            //Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            //AIType = ProjectileID.Bullet;
            Projectile.scale = 1.5f;
            

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;
            if (speed == 0) //store homing speed
                speed = Projectile.velocity.Length() * 2f;

            float desiredFlySpeedInPixelsPerFrame = speed;
            const float amountOfFramesToLerpBy = 20; // minimum of 1, please keep in full numbers even though it's a float!



            NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2400, false, true));
            if (n.Alive())
            {   

                //redirect
                if (++DashTimer <= 45)
                {
                    Vector2 desiredVelocity = Projectile.SafeDirectionTo(n.Center + Projectile.DirectionFrom(n.Center) * 200) * desiredFlySpeedInPixelsPerFrame;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
                
                //dash
                if (++DashTimer >= 45)
                {
                    if (DashAmount < 4)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched with { Volume = 0.2f, MaxInstances = 1 }, Projectile.Center);
                    }
                    Projectile.velocity = Projectile.SafeDirectionTo(n.Center + n.velocity * 10) * 65 + Main.rand.NextVector2Circular(5,5);
                    DashTimer = 0;
                    DashAmount += 1;
                }
                // bite
                if (DashAmount >= 4 && DashTimer <= 1)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(n.Center) * 85;
                    Projectile.damage *= 2;
                }
                // kill
                if (DashAmount >= 4 && Projectile.Distance(n.Center) <= 50)
                {   
                    
                    Projectile.Kill();
                }
                Projectile.rotation = Projectile.SafeDirectionTo(n.Center).ToRotation() + (float)Math.PI / 2;
                
                

            }
                  
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float pscale1 = 2f;
            float pscale2 = 1.75f;
            float pscale3 = 1.5f;
            Particle p = new SparkParticle(target.Center, (Projectile.velocity + Main.rand.NextVector2Circular(45, 45)) * 0.4f, Color.DarkRed, pscale1, 25);
            Particle p2 = new SparkParticle(target.Center, (Projectile.velocity + Main.rand.NextVector2Circular(35, 35)) * 0.4f, Color.DarkRed, pscale2, 25);
            Particle p3 = new SparkParticle(target.Center, (Projectile.velocity + Main.rand.NextVector2Circular(25, 25)) * 0.4f, Color.DarkRed, pscale3, 25);
            if (DashAmount >= 3)
            {
                pscale1 = 4f;
                pscale2 = 3.75f;
                pscale3 = 2.5f;
            }
            p.Spawn();
            p2.Spawn();
            p3.Spawn();

            Vector2 vector54 = Main.player[Projectile.owner].Center - Projectile.Center;
            Vector2 vector55 = vector54 * -1f;
            vector55.Normalize();
            vector55 *= Main.rand.Next(45, 65) * 0.1f;
            vector55 = vector55.RotatedBy((Main.rand.NextDouble() - 0.5) * 1.5707963705062866);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center.X, Main.player[Projectile.owner].Center.Y, vector55.X, vector55.Y, ModContent.ProjectileType<MechEyeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -10f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center.X, Main.player[Projectile.owner].Center.Y, vector55.X, vector55.Y, ModContent.ProjectileType<MechEyeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -10f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center.X, Main.player[Projectile.owner].Center.Y, vector55.X, vector55.Y, ModContent.ProjectileType<MechEyeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -10f);



            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            for (int num468 = 0; num468 < 20; num468++)
            {
                int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Silver, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 2f;
                num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Silver, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, .75f);
                Main.dust[num469].velocity *= 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                if (i < 7)
                {
                    Color color27 = color26 * 0.4f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return true;
        }
    }
}