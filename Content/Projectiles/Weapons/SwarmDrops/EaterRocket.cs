﻿using FargowiltasSouls.Assets.Particles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops
{
    public class EaterRocket : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/SwarmWeapons", Name);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 1;

            //Projectile.penetrate = 99;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            //dust!
            int width = 3;   
            Vector2 dustCenter = Projectile.Center - Vector2.UnitX * width;
            Vector2 dustSpinny = Projectile.velocity;
            dustSpinny.Normalize();
            dustCenter -= dustSpinny * Projectile.width / 2;
            dustSpinny *= Projectile.width / 2;
            dustSpinny = dustSpinny.RotatedBy(Projectile.timeLeft * MathF.Tau / 23f);
            dustCenter += dustSpinny;
            int dustId = Dust.NewDust(dustCenter, width * 2, width * 2, DustID.PurpleCrystalShard, 0f, 0f, 100, default, 1f);
            Main.dust[dustId].noGravity = true;
            Main.dust[dustId].velocity *= 0;

            //if (Projectile.penetrate < 99 && Projectile.ai[1] != 1)
            //{
            //    Projectile.ai[1] = 1;
            //    Projectile.timeLeft = 10;
            //}
        }

        bool sweetspot;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player owner = Main.player[Projectile.owner];

            int closeDistance = 300;
            int farDistance = EaterLauncher.BaseDistance;
            int midDistance = (closeDistance + farDistance) / 2;

            Vector2 middleOfSweetspot = owner.Center + owner.SafeDirectionTo(target.Center) * midDistance;
            Vector2 targetPoint = FargoSoulsUtil.ClosestPointInHitbox(target.Hitbox, middleOfSweetspot);
            float dist = Vector2.Distance(targetPoint, owner.Center);

            if (dist > closeDistance)
            {
                modifiers.SourceDamage *= 1.5f;
                sweetspot = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            //dust
            /*for (int num468 = 0; num468 < 20; num468++)
            {
                int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 2f;
                num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[num469].velocity *= 1.5f;
            }*/

            if (sweetspot)
            {
                SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
                for (int i = 0; i < 3; i++)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 100, new Color(), 1f);
                    Main.dust[index2].noGravity = false;
                    Main.dust[index2].velocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.5f);
                    Main.dust[index2].velocity *= Main.rand.NextFloat(0.5f, 1.3f);
                    Main.dust[index2].scale *= Main.rand.NextFloat(0.8f, 1.2f);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptTorch, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptTorch, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }
            }

            if (!sweetspot)
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { Volume = 0.5f }, Projectile.Center);

            for (int j = 0; j < (sweetspot ? 4 : 2); j++)
            {
                Color color = Color.Lerp(Color.DarkViolet, Color.Black, Main.rand.NextFloat(0.3f, 0.7f));
                int lifetime = Main.rand.Next(18, 32);
                Particle p = new SmokeParticle(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.2f) / 4, color, lifetime, 1f, 0.05f, Main.rand.NextFloat(MathF.Tau));
                p.Spawn();
                /*
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default,
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity += new Vector2(1, 1).RotatedBy(MathHelper.TwoPi / 4 * j);
                */
            }

            /*if (Projectile.owner == Main.myPlayer)
            {
                int max = 2;
                for (int i = 0; i < max; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ProjectileID.TinyEater, Projectile.damage / 6, Projectile.knockBack / 6, Main.myPlayer);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].DamageType = DamageClass.Ranged;
                }
            }*/
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.Purple * Projectile.Opacity * 0.55f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}