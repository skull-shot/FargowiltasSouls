using Microsoft.Xna.Framework;
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
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            const float desiredFlySpeedInPixelsPerFrame = 10;
            const float amountOfFramesToLerpBy = 30;

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
                }
                    
            }        
            else // launched
            {
                Projectile.velocity *= 0.92f;
                Projectile.velocity += LaunchDirection.ToRotationVector2() * 1f;
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
    }
}