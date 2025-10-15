using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.LunarEvents.Solar
{
    public class DrakanianDaybreak : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_636";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Daybreak);
            AIType = ProjectileID.Daybreak;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Default;
        }

        public override void AI()
        {
            Projectile.alpha = 0;
            Projectile.hide = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 900);
            target.AddBuff(ModContent.BuffType<DaybrokenBuff>(), 180);
        }
    }
}