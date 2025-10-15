﻿using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.Deerclops
{
    public class DeerclopsHand : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_965";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.InsanityShadowHostile];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = ProjectileID.Sets.TrailCacheLength[ProjectileID.InsanityShadowHostile];
            ProjectileID.Sets.TrailingMode[Projectile.type] = ProjectileID.Sets.TrailingMode[ProjectileID.InsanityShadowHostile];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.InsanityShadowHostile);
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 900;
        }

        bool spawned;
        float oldMash;

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, Projectile.Center);
            }

            if (Projectile.ai[1] == 0)
            {
                Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
                if (player != null && player.FargoSouls().MashCounter < 20 && Projectile.timeLeft > 60)
                {
                    if (player.active && !player.dead && !player.ghost && Projectile.Colliding(Projectile.Hitbox, player.Hitbox))
                    {
                        player.frozen = true;
                        if (WorldSavingSystem.MasochistModeReal)
                            player.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 2);
                        player.AddBuff(ModContent.BuffType<GrabbedHarmlessBuff>(), 2);

                        if (oldMash < player.FargoSouls().MashCounter)
                        {
                            oldMash = player.FargoSouls().MashCounter;
                            int handCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] / 2 + 1;
                            if (Main.rand.NextBool(handCount))
                            {
                                SoundEngine.PlaySound(SoundID.Item27, player.Center);
                                for (int i = 0; i < 10; i++)
                                {
                                    Dust.NewDust(player.position, player.width, player.height, DustID.IceRod);
                                }
                            }
                        }
                    }

                    Projectile.velocity = (player.Center - Projectile.Center) / 10f;
                    Projectile.position += player.position - player.oldPosition;

                    Projectile.direction = Projectile.spriteDirection = Math.Sign(player.Center.X - Projectile.Center.X);
                    Projectile.rotation = Projectile.SafeDirectionTo(player.Center).ToRotation();

                    const float IdleAccel = 2f;
                    foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == Projectile.type && p.whoAmI != Projectile.whoAmI && p.Distance(Projectile.Center) < Projectile.width))
                    {
                        p.position.X += IdleAccel * (p.position.X < Projectile.position.X ? -1 : 1);
                        p.position.Y += IdleAccel * (p.position.Y < Projectile.position.Y ? -1 : 1);
                    }
                }
                else
                {
                    if (Projectile.velocity == Vector2.Zero)
                        Projectile.velocity = Main.rand.NextVector2CircularEdge(1, 1) * 16f;

                    Projectile.velocity *= -1f;

                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, Projectile.Center);

                    Projectile.ai[1] = 1;
                    Projectile.netUpdate = true;
                }

                Projectile.alpha -= 25;
                if (Projectile.alpha < 50)
                    Projectile.alpha = 50;
            }
            else
            {
                Projectile.velocity *= 0.98f;

                Projectile.alpha += 25;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }

                Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Projectile.spriteDirection < 0)
                Projectile.rotation += MathHelper.Pi;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Frostburn, 90);
            if (WorldSavingSystem.MasochistModeReal)
                target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 900);
        }

        //public override Color? GetAlpha(Color lightColor)
        //{
        //    return Color.White * Projectile.Opacity;
        //}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Color.Black * Projectile.Opacity;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.LightBlue * Projectile.Opacity * 0.75f;
                float ratio = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= ratio * ratio;
                float scale = Projectile.scale * 1.1f * ratio;
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}