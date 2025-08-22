using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Nature
{
    public class NatureFireball : WillFireball
    {
        public override string Texture => "Terraria/Images/Projectile_711";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.tileCollide = true;

            if (Projectile.velocity.Length() < 24)
                Projectile.velocity *= 1.06f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<DaybrokenBuff>(), 300);
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}