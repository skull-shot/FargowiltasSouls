using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode.Accessories.PureHeart
{
    public class GelicWingSpike : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_920";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Spike");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.QueenSlimeMinionBlueSpike];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.QueenSlimeMinionBlueSpike);
            AIType = ProjectileID.QueenSlimeMinionBlueSpike;
            Projectile.tileCollide = true;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Generic;

            FargowiltasSouls.MutantMod.Call("LowRenderProj", Projectile);
        }

        public override bool PreAI()
        {
            if (Projectile.ai[1] == 0)
                SoundEngine.PlaySound(SoundID.Item154 with {Volume = 0.5f}, Projectile.Center);
                Projectile.ai[1] = 1;
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 0;
        }
    }
}