using FargowiltasSouls.Common.Graphics.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2GlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int JammedRecoverTime = 0;
        public bool Jammed = false;
        public bool IsADD2SentryProj = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // there's prob a better way to do this but whatever man
            if (source is EntitySource_Parent parent && parent.Entity is Projectile proj && ProjectileID.Sets.IsADD2Turret[proj.type])
            {
                IsADD2SentryProj = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override bool PreAI(Projectile projectile)
        {
            if (!ProjectileID.Sets.IsADD2Turret[projectile.type])
                return base.PreAI(projectile);

            if (Jammed)
            {
                JammedRecoverTime = 90;
                if (Main.rand.NextBool(4))
                {
                    float rot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    new ElectricSpark(projectile.Center, 5 * Vector2.UnitX.RotatedBy(rot), Color.Purple, 1f, 25).Spawn();
                }

                return false;
            }
            if (JammedRecoverTime > 0)
            {
                JammedRecoverTime--;
                if (JammedRecoverTime % 10 == 0)
                {
                    float rot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    new ElectricSpark(projectile.Center, 5 * Vector2.UnitX.RotatedBy(rot), Color.Purple, 1f, 25).Spawn();
                }
                return false;
            }

            return base.PreAI(projectile);
        }

        public override void PostAI(Projectile projectile)
        {
            base.PostAI(projectile);
            Jammed = false;
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (JammedRecoverTime == 0)
                return base.PreDraw(projectile, ref lightColor);
            lightColor = Color.Purple;
            return base.PreDraw(projectile, ref lightColor);
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (!Jammed)
                return;
            lightColor = Color.DeepPink;
            base.PostDraw(projectile, lightColor);
        }
    }
}
