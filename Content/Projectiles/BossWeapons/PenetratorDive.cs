namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class PenetratorDive : Penetrator
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/Penetrator";

        public override void AI()
        {
            base.AI();
            Projectile.localAI[0]++;
        }

        public override bool? CanDamage()
        {
            if (Projectile.localAI[0] > 2)
                return true;
            return null;
        }
    }
}