﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Common.Graphics.Particles;
using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons
{
    public class Retirang : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/BossWeapons", Name);
        int counter;
        int returntime;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Retirang");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 2;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.light = 0.4f;

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.FargoSouls().CanSplit = false;
        }
      

        public override void AI()
        {
            if (Main.player[Projectile.owner].Alive())
            {
                if (Main.projectile.Where(p => p.TypeAlive(Type) && p.owner == Projectile.owner).Count() > 3) 
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] > 30)
                {
                    Projectile.ai[0] = 2;
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.ai[0] == 1)
            {
                Projectile.extraUpdates = 0;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center) * 25, 0.2f);

                //kill when back to player
                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 30)
                    Projectile.Kill();

            }

            if (Projectile.ai[0] == 2)
            {
                Projectile.frame = 1;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity * 0, 1 / 10f);

                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1600, false, true));
                if (n.Alive())
                {
                    if (counter < 8)
                    {
                        Projectile.rotation = Projectile.SafeDirectionTo(n.Center).ToRotation() + (float)Math.PI;
                    }

                    if (++Projectile.ai[1] > 25)
                    {
                        if (Projectile.owner == Main.myPlayer && counter <= 8)
                        {
                            SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(n.Center - Projectile.Center) * 20, ModContent.ProjectileType<PrimeLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            if (p != Main.maxProjectiles)
                            {
                                Main.projectile[p].DamageType = DamageClass.Melee;
                                Main.projectile[p].tileCollide = false;
                                ProjectileID.Sets.CultistIsResistantTo[p] = true;
                                
                            }

                            Projectile.velocity -= Vector2.Normalize(n.Center - Projectile.Center);

                            Particle p1 = new SparkParticle(Projectile.Center, (n.Center - Projectile.Center) * 0.02f + Main.rand.NextVector2Circular(5, 0), Color.Red, 0.66f, 25);
                            Particle p2 = new SparkParticle(Projectile.Center, (n.Center - Projectile.Center) * 0.02f + Main.rand.NextVector2Circular(5, 0), Color.Red, 0.66f, 25);
                            Particle p3 = new SparkParticle(Projectile.Center, (n.Center - Projectile.Center) * 0.02f + Main.rand.NextVector2Circular(5, 0), Color.Red, 0.66f, 25);
                            p1.Spawn();
                            p2.Spawn();
                            p3.Spawn();
                            counter += 1;
                        }
                        Projectile.ai[1] = 0;
                        

                    }

                }
                else
                {
                    if (++returntime >= 120)
                    {
                        Projectile.ai[0] = 1;
                        Projectile.frame = 0;
                    }
                    Projectile.rotation = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI;
                }

                if (counter >= 8 && ++returntime >= 120)
                {
                    Projectile.ai[0] = 1;
                    Projectile.frame = 0;
                }

            }
            if (Projectile.ai[0] != 2)
            Projectile.rotation += 0.22f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            //smaller tile hitbox
            width = 22;
            height = 22;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.4f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}