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
            Projectile.width = 16;
            Projectile.height = 24;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            AIType = ProjectileID.Bullet;
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
                if (DashAmount !>= 8)
                {
                    Vector2 desiredVelocity = Projectile.SafeDirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
                

                if (++DashTimer >= 25)
                {
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched with { Volume = 0.2f, MaxInstances = 1}, Main.LocalPlayer.Center);
                    Projectile.velocity = Projectile.SafeDirectionTo(n.Center) * 35 + Main.rand.NextVector2Circular(5,5);
                    DashTimer = 0;
                    DashAmount += 1;
                }

                if (DashAmount >= 8)
                {
                    Projectile.Kill();
                }

            }
                  
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (DashAmount >= 8)
            {
            }     
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
            Vector2 origin2 = rectangle.Size() * 0.5f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            /*for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.63f)
            {
                int max0 = Math.Max((int)i - 1, 0);
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                if (i < 3)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), rectangle, color27, Projectile.oldRot[(int)i], origin2, Projectile.scale, SpriteEffects.None, 0);
                }
            }*/
            return true;
        }
    }
}