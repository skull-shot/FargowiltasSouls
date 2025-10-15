﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantSpearSpin : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantSpear_April" :
            FargoAssets.GetAssetString("Content/Projectiles/Weapons/FinalUpgrades", "Penetrator");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 152;
            Projectile.height = 152;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.FargoSouls().TimeFreezeImmune = true;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        private bool predictive;
        private int direction = 1;
        private float spinRender;

        public ref float Variant => ref Projectile.ai[2];
        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = Main.rand.NextBool() ? -1 : 1;
                Projectile.timeLeft = (int)Projectile.ai[1];
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            NPC mutant = Main.npc[(int)Projectile.ai[0]];
            if (mutant.active && mutant.type == ModContent.NPCType<MutantBoss>())
            {
                Projectile.Center = mutant.Center;
                direction = mutant.direction;

                float[] allowedStates = {
                    4,
                    13,
                    21,
                    32,
                    40,
                    -3,
                    -4,
                };
                if (allowedStates.Contains(mutant.ai[0]))
                {
                    Projectile.rotation += (float)Math.PI / 6.85f * Projectile.localAI[1] * Math.Min(1f, ++Projectile.localAI[2] / 90);

                    if (Variant == 0 && ++Projectile.localAI[0] > 8)
                    {
                        Projectile.localAI[0] = 0;
                        if (FargoSoulsUtil.HostCheck && Projectile.Distance(Main.player[mutant.target].Center) > 360)
                        {
                            Vector2 speed = mutant.DirectionFrom(Main.player[mutant.target].Center).RotatedByRandom(Math.PI / 2) * Main.rand.NextFloat(6f, 9f);
                            float ai1 = Projectile.timeLeft + Main.rand.Next(Projectile.timeLeft / 2);
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.position + Main.rand.NextVector2Square(0f, Projectile.width),
                                speed, ModContent.ProjectileType<MutantEyeHoming>(), Projectile.damage, 0f, Projectile.owner, mutant.target, ai1);
                        }
                    }

                    if (Projectile.timeLeft % 20 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                    }

                    if (mutant.ai[0] == 13)
                        predictive = true;

                    Projectile.alpha = 0;

                    //if (WorldSavingSystem.MasochistModeReal)
                    //{
                    //    Main.projectile.Where(x => x.active && x.friendly && !FargoSoulsUtil.IsMinionDamage(x, false)).ToList().ForEach(x => //reflect projectiles
                    //    {
                    //        if (Vector2.Distance(x.Center, mutant.Center) <= Projectile.width / 2)
                    //        {
                    //            for (int i = 0; i < 5; i++)
                    //            {
                    //                int dustId = Dust.NewDust(x.position, x.width, x.height, 87,
                    //                    x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                    //                Main.dust[dustId].noGravity = true;
                    //            }

                    //            // Set ownership
                    //            x.hostile = true;
                    //            x.friendly = false;
                    //            x.owner = Main.myPlayer;
                    //            x.damage /= 4;

                    //            // Turn around
                    //            x.velocity *= -1f;

                    //            // Flip sprite
                    //            if (x.Center.X > mutant.Center.X)
                    //            {
                    //                x.direction = 1;
                    //                x.spriteDirection = 1;
                    //            }
                    //            else
                    //            {
                    //                x.direction = -1;
                    //                x.spriteDirection = -1;
                    //            }

                    //            //x.netUpdate = true;

                    //            if (x.owner == Main.myPlayer)
                    //                Projectile.NewProjectile(Projectile.InheritSource(Projectile), x.Center, Vector2.Zero, ModContent.ProjectileType<Souls.IronParry>(), 0, 0f, Main.myPlayer);
                    //        }
                    //    });
                    //}
                }
                else
                {
                    Projectile.alpha = 255;
                }
            }
            else
            {
                Projectile.Kill();
                return;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), target.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, ModContent.ProjectileType<MutantBombSmall>(), 0, 0f, Projectile.owner);
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            /*if (WorldSavingSystem.MasochistModeReal) //reflects projs indicator trail
            {
                for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.1f)
                {
                    Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/PenetratorSpinGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    Color color27 = Color.Lerp(new Color(51, 255, 191, 210), Color.Transparent, (float)Math.Cos(Projectile.ai[0]) / 3 + 0.3f);
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    float scale = Projectile.scale - (float)Math.Cos(Projectile.ai[0]) / 5;
                    scale *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    int max0 = Math.Max((int)i - 1, 0);
                    Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                    float smoothtrail = i % 1 * (float)Math.PI / 6.85f;
                    bool withinangle = Projectile.rotation > -Math.PI / 2 && Projectile.rotation < Math.PI / 2;
                    if (withinangle && direction == 1)
                        smoothtrail *= -1;
                    else if (!withinangle && direction == -1)
                        smoothtrail *= -1;

                    center += Projectile.Size / 2;

                    Vector2 offset = (Projectile.Size / 4).RotatedBy(Projectile.oldRot[(int)i] - smoothtrail * -Projectile.direction);
                    Main.EntitySpriteDraw(
                        glow,
                        center - offset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                        null,
                        color27,
                        Projectile.rotation,
                        glow.Size() / 2,
                        scale * 0.4f,
                        SpriteEffects.None,
                        0);
                }
            }*/

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            if (Variant < 2 && Projectile.ai[1] > 0)
            {
                Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantSpearAimGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                float modifier = Projectile.timeLeft / Projectile.ai[1];
                Color glowColor = FargoSoulsUtil.AprilFools ? new Color(255, 191, 51, 210) : new(51, 255, 191, 210);
                if (predictive)
                    glowColor = FargoSoulsUtil.AprilFools ? new Color(255, 0, 0, 210) : new Color(0, 0, 255, 210);
                
                const int fireballAnimTime = 30;
                float repeats = (float)(Projectile.localAI[2] * Math.Min(1f, Projectile.localAI[2] / 90f) % fireballAnimTime) / fireballAnimTime;
                FancyFireballs(repeats, glowColor);

                glowColor *= 1f - modifier;
                float glowScale = Projectile.scale * 8f * modifier;
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glow.Bounds), glowColor, 0, glow.Bounds.Size() / 2, glowScale, SpriteEffects.None, 0);
            }
            return false;
        }

        void FancyFireballs(float repeats, Color glowColor)
        {
            AtlasTexture glow = AtlasManager.GetTexture("FargowiltasSouls.Bloom"); //ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantEye_Glow").Value;
            int rect1 = glow.Height;
            int rect2 = rect1 * Projectile.frame;
            Rectangle? glowrectangle = new(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Value.Size() / 2f;

            const int max = 6;
            const int trail = 12;
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < trail; j++)
                {
                    float modifier = repeats - j * 0.01f;

                    float distance = 360 * (1f - modifier);
                    float rotation = MathHelper.TwoPi * modifier;
                    float lerp = MathF.Pow((float)j / trail, 0.5f);
                    Color baseColor = Color.Lerp(Color.White, glowColor, 0.7f + 0.3f * lerp);
                    Color finalColor = baseColor * (float)Math.Sqrt(modifier);

                    finalColor *= 1f - (float)j / trail;

                    Vector2 offset = distance * Vector2.UnitX.RotatedBy(rotation + MathHelper.TwoPi / max * i);
                    Vector2 drawPos = Projectile.Center + offset;
                    float scale = 1f * (1f - 0.9f * modifier);
                    float drawRotation = offset.ToRotation();
                    float endRotation = drawRotation + MathHelper.PiOver2;
                    drawRotation = MathHelper.Lerp(drawRotation, endRotation, modifier);

                    Utilities.Draw(Main.spriteBatch, glow, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), glowrectangle, finalColor with { A = 0 }, drawRotation, null, Vector2.One * scale, SpriteEffects.None);
                    //SpriteBatch.Draw(glow, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), glowrectangle, finalColor, drawRotation, gloworigin2, scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}