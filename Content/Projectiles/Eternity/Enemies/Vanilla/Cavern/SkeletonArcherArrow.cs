using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Cavern
{
    public class SkeletonArcherArrow : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Enemies/Vanilla/Cavern", Name);
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.VenomArrow);
            AIType = ProjectileID.VenomArrow;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.arrow = false;
            Projectile.hostile = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 180);
        }
    }
}