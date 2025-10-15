using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class SpiderEnchantSpiderling : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.DangerousSpider);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.DangerousSpider];
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DangerousSpider);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            AIType = ProjectileID.DangerousSpider;
            Projectile.minion = false;
            Projectile.minionSlots = 0;
            Projectile.usesIDStaticNPCImmunity = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;

            Projectile.timeLeft = 60 * 4;
        }
        public override bool? CanCutTiles() => false;
        public static int SpiderDamage(Player player)
        {
            bool wiz = player.ForceEffect<SpiderEffect>();
            int baseDamage = wiz ? 16 : 9;
            return (int)(baseDamage * player.ActualClassDamage(DamageClass.Summon));
        }
        public ref float Target => ref Projectile.ai[2];
        public override bool PreAI()
        {
            // frames
            Projectile.frameCounter += 1;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 4;
            }
            if (Projectile.frame < 4)
            {
                Projectile.frame = 7;
            }

            // player
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return false;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.Alive() || !player.HasEffect<SpiderEffect>())
            {
                Projectile.Kill();
                return false;
            }

            Projectile.damage = SpiderDamage(player);

            // movement
            if (Target < 0) // has no target
            {
                NPC npc = Projectile.FindTargetWithinRange(800, true);
                if (npc != null && npc.Alive())
                {
                    Target = npc.whoAmI;
                    Projectile.netUpdate = true;
                }
            }
            else // has target, seek it out
            {
                NPC npc = Main.npc[(int)Target];
                if (npc != null && npc.Alive() && npc.CanBeChasedBy(Projectile))
                {
                    Vector2 vectorToIdlePosition = npc.Center - Projectile.Center;
                    float speed = 30f;
                    float inertia = 18f;
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                    if (Projectile.velocity == Vector2.Zero)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    const int MaxSpeed = 30;
                    Projectile.velocity.ClampLength(0, MaxSpeed);
                }
                else
                    Target = -2;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += 0.5f;
        }
    }
}
