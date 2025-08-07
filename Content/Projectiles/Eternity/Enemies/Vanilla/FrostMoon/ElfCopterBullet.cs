using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.FrostMoon
{
    public class ElfCopterBullet : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Enemies/Vanilla/FrostMoon", Name);
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ExplosiveBullet);
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (FargoSoulsUtil.HostCheck)
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ElfCopterBulletExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}