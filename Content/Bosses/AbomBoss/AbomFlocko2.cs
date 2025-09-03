using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomFlocko2 : AbomFlocko
    {
        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0 || Projectile.ai[0] >= Main.maxPlayers)
            {
                Projectile.Kill();
                return;
            }

            Player player = Main.player[(int)Projectile.ai[0]];

            Vector2 target = player.Center;
            target.X += 700 * Projectile.ai[1];

            Vector2 distance = target - Projectile.Center;
            float length = distance.Length();
            if (length > 100f)
            {
                distance /= 8f;
                Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (Projectile.velocity.Length() < 12f)
                    Projectile.velocity *= 1.05f;
            }

            Projectile.Center = Vector2.Lerp(Projectile.Center, target, 0.05f);

            if (++Projectile.localAI[0] > 90)
            {
                float waveSpeed = WorldSavingSystem.MasochistModeReal ? 7f : 5.5f;

                //in abom p2, after firing first 3 wave spread, fire 4 frost wave in between 3s
                if (Projectile.ai[2] > 1 && Projectile.localAI[0] > 90 + 60 && Projectile.localAI[1] == 30)
                {
                    SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 vel = Projectile.SafeDirectionTo(player.Center) * waveSpeed;
                        for (float i = -1.5f; i <= 1.5f; i++)
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(MathHelper.ToRadians(33) * i), ModContent.ProjectileType<AbomFrostWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                if (++Projectile.localAI[1] > 60) //fire 3 frost wave
                {
                    Projectile.localAI[1] = 0f;
                    SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 vel = Projectile.SafeDirectionTo(player.Center) * waveSpeed;
                        float iter = 1;
                        if (WorldSavingSystem.MasochistModeReal)
                            iter = 0.5f;
                        for (float i = -1; i <= 1; i += iter)
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(MathHelper.ToRadians(33) * i), ModContent.ProjectileType<AbomFrostWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }

            Projectile.rotation += Projectile.velocity.Length() / 12f * (Projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++Projectile.frameCounter > 3)
            {
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }
    }
}