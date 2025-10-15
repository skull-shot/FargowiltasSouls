﻿using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomStyxGazer : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/FinalUpgrades", "StyxGazer");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public static int TelegraphTime => WorldSavingSystem.MasochistModeReal ? 30 : 30;
        public int maxTime = 60;

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.scale = 1f;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 60;
            //Projectile.alpha = 250;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;

            Projectile.hide = true;

            CooldownSlot = ImmunityCooldownID.Bosses;
        }
        public ref float TelegraphTimer => ref Projectile.ai[2];
        public ref float RotationTimer => ref Projectile.localAI[2];
        public ref float Rotation => ref Projectile.localAI[1];
        public static int Direction = 1;
        public override void AI()
        {
            Projectile.hide = false; //to avoid edge case tick 1 wackiness

            //the important part
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<AbomBoss>());
            if (npc != null)
            {
                if (npc.ai[0] == 0) Projectile.extraUpdates = 1;

                if (!npc.HasValidTarget || !Main.player[npc.target].Alive())
                {
                    Projectile.Kill();
                    return;
                }
                Player player = Main.player[npc.target];
                float toPlayer = npc.DirectionTo(player.Center).ToRotation();
                float idleRot = MathHelper.Pi * 0.7f * -Math.Sign(Projectile.ai[1]);

                if (TelegraphTimer < TelegraphTime)
                {
                    TelegraphTimer += 1f / Projectile.MaxUpdates;
                    Rotation = MathHelper.SmoothStep(toPlayer, toPlayer + idleRot, MathF.Pow(TelegraphTimer / TelegraphTime, 0.5f));


                    float fadein = 15;
                    if (TelegraphTimer <= fadein)
                        Projectile.Opacity = TelegraphTimer / fadein;
                }
                else
                {
                    RotationTimer++;
                    Rotation = MathHelper.SmoothStep(toPlayer + idleRot, toPlayer - idleRot, MathF.Pow(RotationTimer / maxTime, 0.5f));


                    float fadeout = 15;
                    if (RotationTimer >= maxTime - fadeout)
                        Projectile.Opacity = 1 - ((RotationTimer - (maxTime - fadeout)) / fadeout);

                    if (RotationTimer >= maxTime)
                        Projectile.Kill();
                }
                Projectile.Center = npc.Center + new Vector2(60, 60).RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4) * Projectile.scale;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.localAI[0] == 0 && TelegraphTimer >= TelegraphTime)
            {
                Projectile.localAI[0] = 1;

                /*Vector2 basePos = Projectile.Center - Projectile.velocity * 141 / 2 * Projectile.scale;
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(basePos + Projectile.velocity * Main.rand.NextFloat(127) * Projectile.scale, 0, 0, 87, Scale: 3f);
                    Main.dust[d].velocity *= 4.5f;
                    Main.dust[d].noGravity = true;
                }*/

                SoundEngine.PlaySound(FargosSoundRegistry.AbomScytheSwing, Projectile.Center);
            }

            /*if (Projectile.timeLeft == maxTime - 20)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    int p = Player.FindClosest(Projectile.Center, 0, 0);
                    if (p != -1)
                    {
                        Vector2 vel = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 30f;
                        int max = 8;
                        for (int i = 0; i < max; i++)
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(MathHelper.TwoPi / max * i), ModContent.ProjectileType<AbomSickle3>(), Projectile.damage, Projectile.knockBack, Projectile.owner, p);
                        }
                    }
                }
            }*/

            Projectile.velocity = Rotation.ToRotationVector2();
            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.ai[1]);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(Projectile.direction < 0 ? 135 : 45);
            //Main.NewText(MathHelper.ToDegrees(Projectile.velocity.ToRotation()) + " " + MathHelper.ToDegrees(Projectile.ai[1]));
        }

        public override void OnKill(int timeLeft)
        {
            /*Vector2 basePos = Projectile.Center - Projectile.velocity * 141 / 2 * Projectile.scale;
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(basePos + Projectile.velocity * Main.rand.NextFloat(127) * Projectile.scale, 0, 0, 87, Scale: 3f);
                Main.dust[d].velocity *= 4.5f;
                Main.dust[d].noGravity = true;
            }*/
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<AbomFangBuff>(), 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = lightColor * Projectile.Opacity;
            color.A = (byte)(255 * Projectile.Opacity);
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            Texture2D texture2D14 = FargoAssets.GetTexture2D("Content/Items/Weapons/FinalUpgrades", "StyxGazer_glow").Value;
            Main.EntitySpriteDraw(texture2D14, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}