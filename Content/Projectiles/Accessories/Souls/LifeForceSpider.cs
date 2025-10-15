using System;
using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class LifeForceSpider : LifeForceWasp
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.JumperSpider);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.JumperSpider];
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.extraUpdates = 0;
            Projectile.aiStyle = ProjAIStyleID.BabySpider;
            Projectile.tileCollide = true;
        }
        private NPC? Target;
        private int SpitTimer;
        public override bool PreAI()
        {
            if (Identity != -1 && !FlowerDied)
            {
                Projectile.timeLeft++;
                Projectile.frameCounter += (int)(Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y));
                if (Projectile.frameCounter > 5)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame > 7) Projectile.frame = 4;
                if (Projectile.frame < 4) Projectile.frame = 7;

                Vector2 pos = Projectile.ai[1] == 3 ? new(0, -16) : Projectile.ai[1] == 2 ? new(0, 16) : Projectile.ai[1] == 1 ? new(16, 0) : new(-16, 0);
                Vector2 idlepos = Main.projectile[Identity].Center + pos;

                NPC target = Projectile.FindTargetWithinRange(400, true);
                if (target != null && target.Alive() && target.CanBeChasedBy()) Target = target;

                if (Projectile.Center.Distance(idlepos) < 0.5f)
                {
                    Projectile.velocity *= 0.1f;
                    Projectile.Center = idlepos;
                }
                else
                {
                    Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, idlepos, Projectile.velocity, 1.5f, 1.5f);
                    if (Target != null && Target.Alive() && Target.CanBeChasedBy()) Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }

                if (Target != null && Target.Alive() && Target.CanBeChasedBy())
                {
                    Projectile.rotation = Projectile.rotation.ToRotationVector2().RotateTowards(Projectile.DirectionTo(Target.Center).ToRotation() + MathHelper.PiOver2, 0.1f).ToRotation();
                    SpitTimer++;
                    if (SpitTimer >= 40 && Projectile.owner == Main.myPlayer)
                    {
                        SpitTimer = 0;
                        int spit = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedBy(Projectile.rotation - MathHelper.PiOver2) * 10, ProjectileID.WebSpit, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (spit.IsWithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[spit].hostile = false;
                            Main.projectile[spit].friendly = true;
                            Main.projectile[spit].DamageType = DamageClass.Summon;
                            Main.projectile[spit].extraUpdates = 1;
                            Main.projectile[spit].penetrate = 1;
                        }
                    }
                }
                return false;
            }
            else
            {
                FlowerDied = true;
                Projectile.rotation = 0f;
                Projectile.penetrate = 1;
                if (Projectile.ai[2] == 0)
                {
                    Projectile.velocity.Y = Main.rand.NextFloat(-10, -8); // yump
                    Projectile.ai[2] = 1;
                }
                return true;
            }
        }
        public override void AI()
        {
            base.AI();
            if (FlowerDied && Target != null && Target.Alive() && Target.CanBeChasedBy()) //improved speed bc vanilla baby spider ai sucks
            {
                Projectile.velocity.X += Math.Sign(Target.Center.X - Projectile.Center.X) * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 180);
        }
        public override void OnKill(int timeLeft)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}