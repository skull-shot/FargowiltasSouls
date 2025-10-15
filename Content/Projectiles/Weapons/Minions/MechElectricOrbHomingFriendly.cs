using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses.MechanicalBosses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class MechElectricOrbHomingFriendly : MechElectricOrb
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Bosses/MechanicalBosses","MechElectricOrb");
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 180;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            base.AI();

            Projectile.ai[1]++;

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0]);
            if (npc != null)
            {
                if (Projectile.ai[1] < 60)
                {
                    float rotation = Projectile.velocity.ToRotation();
                    Vector2 vel = npc.Center - Projectile.Center;
                    float targetAngle = vel.ToRotation();
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.05f));
                }
            }
            else
            {
                Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 750);
            }

            if (Projectile.ai[1] < 60)
                Projectile.velocity *= 1.065f;
        }

      /*public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 6;
        }*/
    }
}
