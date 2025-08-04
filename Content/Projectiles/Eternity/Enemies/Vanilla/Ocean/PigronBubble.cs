using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Ocean
{
    public class PigronBubble : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_405";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Default;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[1]);
            if (player != null && player.active && !player.dead)
            {
                Vector2 dist = player.Center - Projectile.Center;
                dist.Normalize();
                dist *= Main.hardMode ? 10f : 8f;
                if (FargoSoulsUtil.PlayerExists(Projectile.ai[1]).ZoneSnow)
                {
                    Projectile.tileCollide = false;
                    dist *= 0.5f;
                    Projectile.light = 0.3f;
                }
                Projectile.velocity = (Projectile.velocity * 20 + dist) / 21;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
            target.AddBuff(BuffID.Wet, 600);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            FargoSoulsUtil.DustRing(Projectile.Center, 20, DustID.BubbleBurst_White, 2.5f);
        }
    }
}