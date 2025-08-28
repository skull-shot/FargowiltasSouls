using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.BloodMoon;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Hell;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Earth
{
    public class EarthFireball : DemonFireball
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.DD2BetsyFireball);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.hostile = true;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            CooldownSlot = ImmunityCooldownID.Bosses;
        }
        ref float DoHoming => ref Projectile.ai[0];
        ref float TileCollide => ref Projectile.ai[1];
        ref float Timer => ref Projectile.localAI[1];

        public override void AI()
        {
            // Projectile.tileCollide = TileCollide == 1;

            if (Timer++ == 0)
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            if (DoHoming == 1f)
            {
                if (Timer < 90) //accelerate
                {
                    Projectile.velocity *= 1.035f;
                }

                if (Timer > 60 && Timer < 150
                    && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<EarthChampion>())
                    && Main.npc[EModeGlobalNPC.championBoss].HasValidTarget) //home
                {
                    float rotation = Projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.championBoss].target].Center
                        + Main.player[Main.npc[EModeGlobalNPC.championBoss].target].velocity * 10f - Projectile.Center;
                    float targetAngle = vel.ToRotation();
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.03f));
                }
            }

            Projectile.rotation += 0.4f;
        }

        public override void OnKill(int timeLeft)
        {
            //if (FargoSoulsUtil.HostCheck) Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FusedExplosion>(), Projectile.damage, 0f, Main.myPlayer);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            //Projectile.timeLeft = 0;

            target.AddBuff(BuffID.OnFire, 120);

            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<LethargicBuff>(), 300);
            }
        }
    }
}