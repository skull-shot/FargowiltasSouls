using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Buffs.Eternity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class AbomMinionScythe : AbomScytheSplit
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/AbomBoss/AbomDeathScythe";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.timeLeft = 120;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            CooldownSlot = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }

            Projectile.rotation += 1f;

            const int aislotHomingCooldown = 1;
            const int homingDelay = 30;
            const float desiredFlySpeedInPixelsPerFrame = 70;
            const float amountOfFramesToLerpBy = 10; // minimum of 1, please keep in full numbers even though it's a float!

            Projectile.ai[aislotHomingCooldown]++;
            if (Projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                Projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                NPC n = FargoSoulsUtil.NPCExists(Projectile.ai[0]);
                if (n.Alive())
                {
                    Vector2 desiredVelocity = Projectile.SafeDirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
                else
                {
                    Projectile.ai[0] = -1;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            const int dustMax = 25;
            const float speed = 6;
            for (int i = 0; i < dustMax; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Scale: 3.5f);
                Main.dust[d].velocity *= speed;
                Main.dust[d].noGravity = true;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 vel = 30f * Vector2.Normalize(Projectile.velocity);
                const int max = 6;
                for (int i = 0; i < max; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel.RotatedBy(MathHelper.TwoPi / max * i), ModContent.ProjectileType<AbomMinionSickle>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 30);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) { }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 600);
        }
    }
}
