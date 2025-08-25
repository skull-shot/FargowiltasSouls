using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons
{
    public class TopazBoltExplosion : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.light = 0.75f;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;

        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
        }
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            if (Projectile.localAI[2] == 0)
            {
                Projectile.localAI[2] = 1;
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3), 1, 1, DustID.GemTopaz);
                    Main.dust[d].velocity = Main.rand.NextFloat(1f, 4f) * (Main.dust[d].position - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    Main.dust[d].noGravity = true;
                }
            }
        }
    }
}