using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Environment
{
    public class RemoteLightning : LightningArc
    {
        public override string Texture => "Terraria/Images/Projectile_466";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged; //cant crit for some reason but i deem this intentional
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = 10;
            Projectile.extraUpdates += 1;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = -Vector2.UnitY * 30;
            Projectile.tileCollide = true;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Main.myPlayer];
            int distance = (int)Projectile.Distance(player.Center);
            if (distance < 60 && !player.immune)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(player.Center) * 20f;
            }
            return base.PreAI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
            Projectile.damage = (int)(Projectile.damage * 0.9);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.Item62);
            if (FargoSoulsUtil.HostCheck)
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RemoteLightningExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.Kill();
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
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage *= 0f;
            if (Main.masterMode) modifiers.SourceDamage.Flat += 270;
            else if (Main.expertMode) modifiers.SourceDamage.Flat += 180;
            else modifiers.SourceDamage.Flat += 120;
        }
    }
}