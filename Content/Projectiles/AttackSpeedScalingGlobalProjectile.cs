using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class AttackSpeedScalingGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool UseAttackSpeedToDoubleHit;
        public bool UseAttackSpeedForAdditionalUpdates;

        Dictionary<int, float> AttackSpeedOnHitDebt = new Dictionary<int, float>();
        bool AllowAttackSpeedOnHitThisTick;

        float AttackSpeedUpdateDebt;

        float GetLocalPlayerAttackSpeed(DamageClass damageClass) => Math.Max(0f, Main.LocalPlayer.FargoSouls().AttackSpeed - 1f);

        public override bool PreAI(Projectile projectile)
        {
            AllowAttackSpeedOnHitThisTick = true;

            if (UseAttackSpeedForAdditionalUpdates)
            {
                float aspd = GetLocalPlayerAttackSpeed(projectile.DamageType);
                AttackSpeedUpdateDebt += aspd;
                if (AttackSpeedUpdateDebt > 1f)
                {
                    AttackSpeedUpdateDebt -= 1f;
                    AttackSpeedUpdateDebt -= aspd; //offset the amount it will be incremented by for calling AI() again
                    projectile.AI();
                }
            }

            return base.PreAI(projectile);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!UseAttackSpeedToDoubleHit || Main.dedServ || projectile.owner != Main.myPlayer || !AllowAttackSpeedOnHitThisTick)
                return;

            float attackSpeedIncrement = GetLocalPlayerAttackSpeed(projectile.DamageType);
            if (!AttackSpeedOnHitDebt.ContainsKey(projectile.type))
                AttackSpeedOnHitDebt.Add(projectile.type, 0);
            AttackSpeedOnHitDebt[projectile.type] += attackSpeedIncrement;
            if (AttackSpeedOnHitDebt[projectile.type] > 1f)
            {
                //Main.NewText("double hit!");
                AllowAttackSpeedOnHitThisTick = false; //make sure we dont somehow get in an infinite loop of stacking endless hits in one tick
                AttackSpeedOnHitDebt[projectile.type] -= 1f;
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
