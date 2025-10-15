using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.MechanicalBosses
{
    public class MechElectricOrbSpaz : MechElectricOrb
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Bosses/MechanicalBosses", "MechElectricOrb");
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.timeLeft = 180;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()))
                CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            base.AI();

            if (++Projectile.ai[1] < 75) //straight accel
                Projectile.velocity *= WorldSavingSystem.MasochistModeReal ? 1.06f : 1.05f;

            Player target = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
            if (target != null)
            {
                float rotation = Projectile.velocity.ToRotation();
                Vector2 vel = target.Center - Projectile.Center;
                float targetAngle = vel.ToRotation();

                //if spaz alive and player isnt behind projectile
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.spazBoss, NPCID.Spazmatism)
                    && System.Math.Abs(MathHelper.WrapAngle(targetAngle - rotation)) < MathHelper.PiOver2)
                {
                    const float deadZone = 600;
                    const float maxHomingRampupDistance = 1800;
                    const float maxLerp = 0.8f;

                    float ratio = (Main.npc[EModeGlobalNPC.spazBoss].Distance(target.Center) - deadZone) / (maxHomingRampupDistance - deadZone);
                    ratio *= ratio; //so the lerp effect ramps up much more violently at distance
                    if (ratio < 0)
                        ratio = 0;
                    if (ratio > 1)
                        ratio = 1;

                    float lerp = maxLerp * ratio;
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, lerp));
                }
            }
        }
    }
}