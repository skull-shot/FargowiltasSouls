﻿using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.MoonLord
{
    public class MoonLordVortexOld : CosmosVortex
    {
        public override string Texture => "Terraria/Images/Projectile_578";

        public override bool? CanDamage()
        {
            return Projectile.localAI[1] > 120;
        }

        public int suck;

        public override void AI()
        {
            const int time = 1800;
            const int maxScale = 3;
            const float suckRange = 150;

            void Suck()
            {
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.friendly && p.Distance(Projectile.Center) < suckRange && p.CountsAsClass(DamageClass.Ranged) && FargoSoulsUtil.CanDeleteProjectile(p) && p.type != ModContent.ProjectileType<LunarCultistLightningArc>()))
                {
                    //suck in nearby friendly projs
                    p.velocity = p.SafeDirectionTo(Projectile.Center) * p.velocity.Length();
                    p.velocity *= 1.015f;

                    //kill ones that actually fall in and retaliate
                    if (FargoSoulsUtil.HostCheck && Projectile.Colliding(Projectile.Hitbox, p.Hitbox))
                    {
                        Player player = Main.player[p.owner];
                        if (player.active && !player.dead && !player.ghost && suck <= 0)
                        {
                            suck = 60;

                            Vector2 dir = Projectile.SafeDirectionTo(player.Center).RotatedByRandom(MathHelper.ToRadians(10));
                            float ai1New = Main.rand.NextBool() ? 1 : -1; //randomize starting direction
                            Vector2 vel = Vector2.Normalize(dir) * 6f;
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel * 6, ModContent.ProjectileType<CosmosLightning>(),
                                Projectile.damage, 0, Main.myPlayer, dir.ToRotation(), ai1New);
                        }
                        p.Kill();
                    }
                }
            };

            if (suck > 0) 
                suck--;

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.MoonLordCore);
            if (npc != null)
            {
                Projectile.localAI[0]++;

                const int orbitTime = 180;

                Vector2 offset;
                offset.X = 300f * (float)Math.Sin(Math.PI * 2 / (orbitTime * 2) * Projectile.localAI[0]);
                offset.Y = 150f * (float)Math.Sin(Math.PI * 2 / orbitTime * Projectile.localAI[0]);

                Projectile.Center = npc.Center + offset;

                //if (Projectile.localAI[1] < 120)
                //{
                //    float num1 = 0.5f;
                //    for (int i = 0; i < 5; ++i)
                //    {
                //        if (Main.rand.NextFloat() >= num1)
                //        {
                //            float f = Main.rand.NextFloat() * 6.283185f;
                //            float num2 = Main.rand.NextFloat();
                //            Dust dust = Dust.NewDustPerfect(Projectile.Center + f.ToRotationVector2() * (110 + 600 * num2), 229, (f - 3.141593f).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);
                //            dust.scale = 0.9f;
                //            dust.fadeIn = 1.15f + num2 * 0.3f;
                //            //dust.color = new Color(1f, 1f, 1f, num1) * (1f - num1);
                //            dust.noGravity = true;
                //            //dust.noLight = true;
                //        }
                //    }
                //}

                if (npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState != 1 && Projectile.timeLeft > 60)
                    Projectile.timeLeft = 60;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            for (int i = 0; i < 10; i++)
            {
                Vector2 offset = new();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * suckRange);
                offset.Y += (float)(Math.Cos(angle) * suckRange);
                Dust dust = Main.dust[Dust.NewDust(
                    Projectile.Center + offset - new Vector2(4, 4), 0, 0,
                    DustID.Vortex, 0, 0, 100, Color.White, 1f
                    )];
                dust.velocity = npc.velocity / 3;
                if (Main.rand.NextBool(3))
                    dust.velocity += Vector2.Normalize(offset);
                dust.noGravity = true;
            }

            Projectile.localAI[1]++;
            if (Projectile.localAI[1] <= 50)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy((float)Math.PI / 2, new Vector2()) * 4f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
                if (Main.rand.NextBool(4))
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 2f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
            }
            else if (Projectile.localAI[1] <= 90)
            {
                Projectile.scale = (Projectile.localAI[1] - 50) / 40 * maxScale;
                Projectile.alpha = 255 - (int)(255 * Projectile.scale / maxScale);
                Projectile.rotation = Projectile.rotation - 0.1570796f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 3f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }

                Suck();
            }
            else if (Projectile.localAI[1] <= 90 + time)
            {
                Projectile.velocity *= 0.9f;

                Projectile.scale = maxScale;
                Projectile.alpha = 0;
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 60f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
                else
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 3f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }

                Suck();
            }
            else
            {
                Projectile.scale = (float)(1.0 - (Projectile.localAI[1] - time) / 60.0) * maxScale;
                Projectile.alpha = 255 - (int)(255 * Projectile.scale / maxScale);
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 30f;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                for (int index = 0; index < 2; ++index)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            Vector2 spinningpoint1 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                            Dust dust1 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint1 * 30f, 0, 0, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 1f)];
                            dust1.noGravity = true;
                            dust1.position = Projectile.Center - spinningpoint1 * Main.rand.Next(10, 21);
                            dust1.velocity = spinningpoint1.RotatedBy((float)Math.PI / 2, new Vector2()) * 6f;
                            dust1.scale = 0.5f + Main.rand.NextFloat();
                            dust1.fadeIn = 0.5f;
                            dust1.customData = Projectile.Center;
                            break;
                        case 1:
                            Vector2 spinningpoint2 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                            Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint2 * 30f, 0, 0, DustID.Granite, 0.0f, 0.0f, 0, new Color(), 1f)];
                            dust2.noGravity = true;
                            dust2.position = Projectile.Center - spinningpoint2 * 30f;
                            dust2.velocity = spinningpoint2.RotatedBy(-(float)Math.PI / 2, new Vector2()) * 3f;
                            dust2.scale = 0.5f + Main.rand.NextFloat();
                            dust2.fadeIn = 0.5f;
                            dust2.customData = Projectile.Center;
                            break;
                    }
                }
            }

            Dust dust3 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 0, new Color(), 1f)];
            dust3.velocity *= 5f;
            dust3.fadeIn = 1f;
            dust3.scale = 1f + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
            dust3.noGravity = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 360);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            int type = 229;
            for (int index = 0; index < 80; ++index)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.velocity *= 10f;
                dust.fadeIn = 1f;
                dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                if (!Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    dust.scale *= 2f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}