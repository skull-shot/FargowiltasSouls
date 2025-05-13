using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class MeteorExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
            Main.projFrames[Type] = 7;
        }

        private int fpf = 4;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Main.projFrames[Type] * fpf;
            Projectile.tileCollide = false;
            Projectile.light = 0.75f;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;

            Projectile.scale = 2f;

        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override void AI()
        {
            if (++Projectile.frameCounter > fpf)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }

            if (Projectile.frame >= Main.projFrames[Type])
            {
                Projectile.Kill();
            }
            Projectile.velocity = Vector2.Zero;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
        /*
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }
        }
        */
    }
}