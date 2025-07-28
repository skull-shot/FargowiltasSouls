using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.WallOfFlesh
{
    public class CursedFlamethrower : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_101";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye Fire");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire); //has 4 updates per tick
            AIType = ProjectileID.EyeFire;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.width = 20;
            Projectile.height = 400;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 300, true);
        }
    }
}