using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.VerdantDoomsayerMask
{
    public class GeyserFriendly : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Geyser");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GeyserTrap);
            Projectile.trap = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            AIType = ProjectileID.GeyserTrap;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
        }
    }
}