using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class EyeProjectile : ModProjectile
    {
        public int EyeTimer = 0;
        public int RotationTimer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("EyeProjectile2");
            //ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 250;
            AIType = ProjectileID.Bullet;
        }
        ref float State => ref Projectile.ai[0];
        ref float LaunchDirection => ref Projectile.localAI[0]; // local so we can sync it manually based on owner in ReceiveExtraAI
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(LaunchDirection);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            float buffer = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                LaunchDirection = buffer;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (State != 0) // launched
                modifiers.FinalDamage *= 1.5f;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            const float desiredFlySpeedInPixelsPerFrame = 14;
            const float amountOfFramesToLerpBy = 20;

            if (State == 0) // not launched
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(player.Center + Projectile.DirectionFrom(player.Center).RotatedByRandom(MathHelper.Pi) * 100) * desiredFlySpeedInPixelsPerFrame;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                if (++RotationTimer >= 60)
                Projectile.rotation = (Projectile.SafeDirectionTo(player.Center).ToRotation() + (float)Math.PI / 2);
                Projectile.timeLeft = 140;
                if (!player.channel)
                {
                    State = 1;
                    if (player.whoAmI == Main.myPlayer)
                        LaunchDirection = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
                    Projectile.netUpdate = true;
                    Projectile.velocity *= 0.4f;
                }
                    
            }        
            else // launched
            {
                Projectile.velocity *= 0.92f;
                Projectile.velocity += LaunchDirection.ToRotationVector2() * 1f;
                if (player.whoAmI == Main.myPlayer && State < 20)
                {
                    LaunchDirection = LaunchDirection.ToRotationVector2().RotateTowards(Projectile.DirectionTo(Main.MouseWorld).ToRotation(), 0.01f).ToRotation();
                    State++;
                    if (State % 5 == 0)
                        Projectile.netUpdate = true;
                }
                    
                //Vector2 desiredVelocity = Projectile.DirectionTo(Main.MouseWorld) * desiredFlySpeedInPixelsPerFrame;
                //Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                //Projectile.timeLeft = 100;
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            }


        }

        public override void OnKill(int timeleft)
        {
            for (int num468 = 0; num468 < 20; num468++)
            {
                int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.RedTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 2f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 2f;
                num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.RedTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
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

            SpriteEffects effects = SpriteEffects.None;

            if (State != 0)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = color26 * Projectile.Opacity * 0.75f * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
                }
            }
            

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}