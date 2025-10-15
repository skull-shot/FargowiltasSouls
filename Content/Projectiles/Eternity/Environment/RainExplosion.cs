﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Environment
{
    public class RainExplosion : CobaltExplosion
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/Souls", "CobaltExplosion");
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Default;

            Projectile.scale = 3;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
           => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width * 0.9f / 2;
        public override bool CanHitPlayer(Player target)
        {
            if (target.HasEffect<LightningImmunity>())
                return false;
            return base.CanHitPlayer(target);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.townNPC)
                return;
            modifiers.SourceDamage *= 10;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
    }
}