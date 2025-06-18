using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class SpookySpinScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }
        public static int Diameter => 100;
        public static int MaxScale(Player player) => player.ForceEffect<SpookyEffect>() ? 5 : 4;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = Diameter;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed; // applies flasks
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public ref float CanHit => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public ref float Direction => ref Projectile.localAI[0];

        public override void AI()
        {
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers) || !Main.player[Projectile.owner].Alive())
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.HasEffect<SpookyEffect>() || !player.HasEffectEnchant<SpookyEffect>())
            {
                Projectile.Kill();
                return;
            }

            Timer++;
            float windup = 15;
            float spin = 30;

            float rotation;
            float size;

            float rotStart = -MathHelper.PiOver2 * 1f;
            float rotEnd = MathF.Tau;
            if (Timer < windup)
            {
                CanHit = 0;
                if (Direction == 0)
                    Direction = player.direction;

                float progress = Timer / windup;
                float sqrtProgress = MathF.Pow(progress, 0.5f);

                rotation = rotStart * sqrtProgress;
                size = MaxScale(player) * sqrtProgress;
            }
            else
            {
                
                if (Timer == windup) // first frame of spin
                {
                    
                }
                if (Timer == windup + (int)(spin / 3)) // spin picking up speed
                {
                    SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
                    CanHit = 1;
                }
                float progress = (Timer - windup) / spin;

                rotation = MathHelper.SmoothStep(rotStart, rotEnd, progress);
                size = MaxScale(player);
                float fadeStart = 0.7f;
                if (progress > fadeStart)
                {
                    float fadeProgress = (progress - fadeStart) / (1 - fadeStart);
                    size *= MathF.Pow(1 - fadeProgress, 0.5f);
                }
                if (progress >= 1)
                    Projectile.Kill();

                if (CanHit > 0)
                {
                    foreach (Projectile projectile in Main.ActiveProjectiles)
                    {
                        if (!projectile.hostile && !projectile.trap && !projectile.npcProj)
                        {
                            var modProj = projectile.FargoSouls();
                            if (projectile.minionSlots > 0 && modProj.spookyCD == 0)
                            {
                                float minDistance = 900f;
                                int npcIndex = -1;
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC target = Main.npc[i];

                                    if (target.active && Vector2.Distance(projectile.Center, target.Center) < minDistance && Main.npc[i].CanBeChasedBy(projectile, false))
                                    {
                                        npcIndex = i;
                                        minDistance = Vector2.Distance(projectile.Center, target.Center);
                                    }
                                }

                                if (npcIndex != -1)
                                {
                                    NPC target = Main.npc[npcIndex];

                                    if (projectile.Colliding(Projectile.Hitbox, projectile.Hitbox) && Collision.CanHit(projectile.Center, 0, 0, target.Center, 0, 0))
                                    {
                                        Vector2 velocity = Vector2.Normalize(target.Center - projectile.Center) * 28;


                                        Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity, ModContent.ProjectileType<SpookyScythe>(), Projectile.damage / 10, 2, projectile.owner);

                                        SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f }, projectile.Center);

                                        modProj.spookyCD = (int)spin;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            Projectile.rotation = Direction * rotation;

            Projectile.scale = size;
            Projectile.width = Projectile.height = (int)(Diameter * size);
            Projectile.Center = player.MountedCenter;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (CanHit == 0)
                return false;
            return base.CanHitNPC(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers) || !Main.player[Projectile.owner].Alive())
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.HasEffect<SpookyEffect>() || !player.HasEffectEnchant<SpookyEffect>())
            {
                Projectile.Kill();
                return;
            }

            if (CanHit == 1 && damageDone > 50)
            {
                player.FargoSouls().HealPlayer(damageDone / 20);
                CanHit = 2;
            }
            target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 60 * 5);
        }
        public override void OnKill(int timeLeft)
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}