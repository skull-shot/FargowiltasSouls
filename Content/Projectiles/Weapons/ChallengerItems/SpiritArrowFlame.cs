using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.ChallengerItems
{

    public class SpiritArrowFlame : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_659"; //spirit flame
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit Flame");

            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.SpiritFlame];
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 26;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.scale = 0.1f;
        }
        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.2f);
            float lightFade = (255f - Projectile.alpha) / 255f;
            Lighting.AddLight(Projectile.Center, lightFade * 0.7f * Color.Plum.R / 255f, lightFade * 0.7f * Color.Plum.G / 255f, lightFade * 0.7f * Color.Plum.B / 255f);
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = -1;
            }
            if (Projectile.ai[0] > 1f)
            {
                if (Projectile.ai[0] % 30 == 11f)
                    Projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 3000, true);

                if (Projectile.ai[1] != -1)
                {
                    if (Main.npc[(int)Projectile.ai[1]].active) //nestled for no index error
                    {
                        
                        Vector2 vectorToIdlePosition = Main.npc[(int)Projectile.ai[1]].Center - Projectile.Center;
                        float distance = vectorToIdlePosition.Length();
                        float speed = 20000f / (2000f + distance * distance);
                        float inertia = 12f;
                        float deadzone = 2f;
                        if (distance > deadzone)
                        {
                            vectorToIdlePosition.Normalize();
                            vectorToIdlePosition *= speed;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                        }
                        else if (Projectile.velocity == Vector2.Zero)
                        {
                            Projectile.velocity.X = -0.15f;
                            Projectile.velocity.Y = -0.05f;
                        }
                    }
                }
                else
                {
                    Projectile.velocity *= 0.9f;
                    if (Projectile.velocity.Length() < 1)
                        Projectile.velocity = Vector2.Zero;
                }
            }
            if (Projectile.ai[0] >= 90)
                Projectile.alpha += 17;

            if (Projectile.alpha > 250)
                Projectile.Kill();

            Projectile.ai[0] += 1f;
        }

    }
}
