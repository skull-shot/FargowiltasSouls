using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class LifeForceBeetle : LifeForceWasp
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Life/ChampionBeetle";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 0;
        }
        private float IdleTimer;
        private int AttackCD;
        private Vector2 IdleLocation;
        private Vector2 ReturnLocation;
        private NPC? Target;
        public override bool PreAI() => true;
        public override void AI()
        {
            base.AI();
            if (Projectile.velocity.X > 0) Projectile.spriteDirection = 1;
            else if (Projectile.velocity.X < 0) Projectile.spriteDirection = -1;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3) Projectile.frame = 0;
            }
            NPC npc = Projectile.FindTargetWithinRange(800, true);
            if (npc != null && npc.Alive() && npc.CanBeChasedBy()) Target = npc;

            if (Identity != -1 && !FlowerDied) // flower active
            {
                Projectile.timeLeft++;
                if (Target != null && Target.Alive() && Target.CanBeChasedBy()) // attack
                {
                    if (Projectile.ai[2] == 0)
                    {
                        HomingMovement(Target.Center);
                        if (Projectile.Colliding(Projectile.Hitbox, Target.Hitbox))
                        {
                            Projectile.ai[2] = 2;
                            Projectile.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (Projectile.ai[2] == 2)
                        {
                            ReturnLocation = Main.projectile[Identity].Center + Main.rand.NextVector2Circular(48, 32);
                            Projectile.ai[2] = 1;
                        }
                        HomingMovement(ReturnLocation);
                        if (Projectile.Center.Distance(ReturnLocation) < 16)
                        {
                            if (Target.Center.DirectionTo(Projectile.Center).X > 0) Projectile.spriteDirection = -1; else Projectile.spriteDirection = 1;
                            AttackCD++; // min timer it needs to rest at flower before attacking again
                            if (AttackCD >= 20)
                            {
                                Projectile.ResetLocalNPCHitImmunity();
                                AttackCD = 0; Projectile.ai[2] = 0;
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
                else // idle, copied from beetle ench because i DONT CAR !!
                {
                    if (IdleTimer > 0) IdleTimer--;
                    if (IdleTimer == 0)
                    {
                        IdleLocation = Main.projectile[Identity].Center + Main.rand.NextVector2Circular(48, 32);
                        IdleTimer = Main.rand.Next(20, 40);
                    }
                    if (Projectile.Distance(IdleLocation) > 16)
                        Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, IdleLocation, Projectile.velocity, Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(0.5f, 1f));
                }
            }
            else // flower has been killed, disperse
            {
                Projectile.penetrate = 1;
                Projectile.ResetLocalNPCHitImmunity();
                NPC newguy = Projectile.FindTargetWithinRange(800, true);
                if (newguy != null) HomingMovement(newguy.Center);
                else Projectile.velocity.Y -= Main.rand.NextFloat(0.3f, 0.4f); // ascend.
            }
        }

        public void HomingMovement(Vector2 destination)
        {
            float speedMod = 1.25f;
            float accel = 1f * speedMod;
            float decel = 1.5f * speedMod;
            float resistance = Projectile.velocity.Length() * accel / (35f * speedMod);
            Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, destination, Projectile.velocity, accel - resistance, decel + resistance);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits >= 5)
                Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath47 with { MaxInstances = 1, Volume = 0.25f }, Projectile.Center);
            for (int k = 0; k < 20; k++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ambient_DarkBrown, 0, -1f);
                Main.dust[d].scale += 0.5f;
                Main.dust[d].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}