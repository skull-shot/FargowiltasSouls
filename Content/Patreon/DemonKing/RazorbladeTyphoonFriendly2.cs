﻿using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.DemonKing
{
    public class RazorbladeTyphoonFriendly2 : RazorbladeTyphoonFriendly
    {
        public override string Texture => "Terraria/Images/Projectile_409";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //if (target.defense > 0) damage += target.defense / 4;
        }
    }
}