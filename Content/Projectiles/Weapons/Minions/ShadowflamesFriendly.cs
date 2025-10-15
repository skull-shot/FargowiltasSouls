using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class ShadowflamesFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_299";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadowflames");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shadowflames);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 180 * (Projectile.extraUpdates + 1);
            Projectile.tileCollide = true;

            Projectile.penetrate = 1;
            //Projectile.usesIDStaticNPCImmunity = true;
            //Projectile.idStaticNPCHitCooldown = 10;
            //Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GiantCursedSkullBolt, 0.0f, 0.0f, 100, default, 1f);
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity *= 3;
                    Dust dust2 = Main.dust[index2];
                    dust2.velocity += Projectile.velocity * 0.5f;
                    Main.dust[index2].noGravity = true;
                }
            }

            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GiantCursedSkullBolt, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 0.6f;
                Main.dust[index2].noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
            for (int index1 = 0; index1 < 30; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GiantCursedSkullBolt, 0.0f, 0.0f, 100, default, 1f);
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3;
                Dust dust2 = Main.dust[index2];
                dust2.velocity += -Projectile.velocity * 0.75f;
                Main.dust[index2].scale *= 1.2f;
                Main.dust[index2].noGravity = true;
            }
        }
    }
}