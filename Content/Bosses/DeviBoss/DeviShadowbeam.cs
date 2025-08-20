using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Dungeon;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviShadowbeam : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.ShadowBeamHostile);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("High Velocity Crystal Bullet");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ShadowBeamHostile);
            AIType = ProjectileID.ShadowBeamHostile;

            Projectile.timeLeft = WorldSavingSystem.MasochistModeReal ? 1200 : 420;
            CooldownSlot = 1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Slow, 90);
        }

    }
}