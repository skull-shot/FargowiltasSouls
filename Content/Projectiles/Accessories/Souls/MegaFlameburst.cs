﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class MegaFlameburst : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_668";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mega Flameburst");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            AIType = ProjectileID.Bullet;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            Projectile.scale = 2f;

            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = 1;

                const int num226 = 12;
                for (int i = 0; i < num226; i++)
                {
                    Vector2 vector6 = Vector2.UnitX.RotatedBy(Projectile.rotation) * 6f;
                    vector6 = vector6.RotatedBy((i - (num226 / 2 - 1)) * 6.28318548f / num226, default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.FlameBurst, 0f, 0f, 0, default, 1.5f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
            }

            const int aislotHomingCooldown = 0;
            const int homingDelay = 5;
            const float desiredFlySpeedInPixelsPerFrame = 16;
            const float amountOfFramesToLerpBy = 20; // minimum of 1, please keep in full numbers even though it's a float!

            Projectile.ai[aislotHomingCooldown]++;
            if (Projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                Projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 480));
                if (n.Alive())
                {
                    Vector2 desiredVelocity = Projectile.SafeDirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override void OnKill(int timeleft)
        {
            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.InfernoFriendlyBlast, Projectile.damage, 0, Projectile.owner);
            if (p != Main.maxProjectiles)
            {
                Main.projectile[p].timeLeft = 5;
                Main.projectile[p].usesLocalNPCImmunity = true;
                Main.projectile[p].localNPCHitCooldown = -1;
                Main.projectile[p].FargoSouls().noInteractionWithNPCImmunityFrames = true;
            }
        }
    }
}
