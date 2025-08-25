using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    // allows single projectiles to scale with attack speed
    // best used with yoyos and other cases of single high pierce, long duration weapons
    // not useful for spamming tons of projectiles that hit once at most (read: hell zone)
    // todo, create a projectile type static iframe-like version of this...?
    public class AttackSpeedOnHitGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public Dictionary<int, float> AttackSpeedDebt = new Dictionary<int, float>();

        public bool UseAttackSpeedToDoubleHit;
        bool AllowAttackSpeedCountingThisTick;

        public override bool PreAI(Projectile projectile)
        {
            AllowAttackSpeedCountingThisTick = true;
            return base.PreAI(projectile);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!UseAttackSpeedToDoubleHit || Main.dedServ || projectile.owner != Main.myPlayer || !AllowAttackSpeedCountingThisTick)
                return;

            float attackSpeedIncrement = Math.Max(0f, Main.LocalPlayer.FargoSouls().AttackSpeed - 1f);
            if (!AttackSpeedDebt.ContainsKey(projectile.type))
                AttackSpeedDebt.Add(projectile.type, 0);
            AttackSpeedDebt[projectile.type] += attackSpeedIncrement;
            if (AttackSpeedDebt[projectile.type] > 1f)
            {
                //Main.NewText("double hit!");
                AllowAttackSpeedCountingThisTick = false; //make sure we dont somehow get in an infinite loop of stacking endless hits in one tick
                AttackSpeedDebt[projectile.type] -= 1f;
                if (projectile.usesIDStaticNPCImmunity)
                    Projectile.perIDStaticNPCImmunity[projectile.type][target.whoAmI] = Main.GameUpdateCount;
                else if (projectile.usesLocalNPCImmunity)
                    projectile.localNPCImmunity[target.whoAmI] = 0;
                else
                    target.immune[projectile.owner] = 0;
                projectile.Damage();
            }
        }
    }
}
