using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.DubiousCircuitry
{
    public class RemoteLightning : LightningArc
    {
        public override string Texture => "Terraria/Images/Projectile_466";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = 10;
            Projectile.extraUpdates += 1;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
            CooldownSlot = ImmunityCooldownID.WrongBugNet;
        }
        public override bool PreAI()
        {
            if (Projectile.tileCollide == false && Projectile.timeLeft < 295 * (Projectile.MaxUpdates - 1)) // account for base timeLeft
                Projectile.tileCollide = true;
            Player player = Main.player[Main.myPlayer];
            int distance = (int)Projectile.Distance(player.Center);
            bool safe = player.creativeGodMode || PlayerLoader.CanBeHitByProjectile(player, Projectile) == false || PlayerLoader.ImmuneTo(player, PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.identity), ImmunityCooldownID.WrongBugNet, false) == true;
            if (distance < 60 && !safe && player.HasEffect<RemoteControlDR>() && !player.HasBuff<SuperchargedBuff>())
                Projectile.velocity = Projectile.SafeDirectionTo(player.Center) * 20f;
            return base.PreAI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 600);
            Projectile.damage = (int)(Projectile.damage * 0.9);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.Item62);
            if (FargoSoulsUtil.HostCheck)
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RemoteLightningExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.velocity = Vector2.Zero;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item62);
            if (FargoSoulsUtil.HostCheck)
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RemoteLightningExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            return base.OnTileCollide(oldVelocity);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (target.townNPC)
                return false;
            return base.CanHitNPC(target);
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