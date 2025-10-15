using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.DubiousCircuitry
{
    public class RemoteLightningExplosion : CobaltExplosion
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/Souls", "CobaltExplosion");
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 3;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
            CooldownSlot = ImmunityCooldownID.WrongBugNet;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
           => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width * 0.9f / 2;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 600);
            Projectile.damage = (int)(Projectile.damage * 0.9);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage *= 0f;
            if (target.HasEffect<RemoteControlDR>() && !target.HasBuff<SuperchargedBuff>())
            {
                modifiers.Knockback *= 0f;
                modifiers.DisableSound();
            }
            //doing it like this bc scaled projectile damage doesnt work?
            if (Main.masterMode) modifiers.SourceDamage.Flat += 270;
            else if (Main.expertMode) modifiers.SourceDamage.Flat += 180;
            else modifiers.SourceDamage.Flat += 120;
        }
    }
}