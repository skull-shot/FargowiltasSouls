using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Souls;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class Spazmarang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spazmarang");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.light = 0.4f;
            Projectile.scale = 1.2f;

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
            Projectile.timeLeft = 250;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] >= 30)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity * 0.4f, 1 / 10f);
                }
                if (Projectile.ai[1] >= 60)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.rotation * 2, 1 / 60f);

            }

            if (Projectile.ai[0] == 1)
            {
                Projectile.extraUpdates = 0;
                Projectile.timeLeft = 2;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center) * 25, 0.2f);

                //kill when back to player
                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 30)
                    Projectile.Kill();
                
            }

            if (Projectile.ai[0] == 2)
            {
                Projectile.rotation += 0.64f;
                Projectile.velocity *= 0;
                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 75, false, true));
                if (n.Alive())
                {
                    Projectile.velocity += n.velocity;
                    //Projectile.Center = n.Center;
                }

                if (Projectile.timeLeft <= 10)
                {
                    Projectile.ai[0] = 1;
                }
            }

            //explosion code
            /*for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.type == ModContent.ProjectileType<Retirang>() && projectile.active && projectile.Distance(Projectile.Center) <= 75 && Projectile.ai[0] != 2 && projectile.ai[0] != 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<CobaltExplosion>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, -10f);
                    Projectile.Kill();
                    projectile.Kill();
                    Gore.NewGore(Projectile.GetSource_Death(), projectile.TopLeft, Projectile.velocity * 0.8f, ModContent.Find<ModGore>(Mod.Name, "Spazgore1").Type, 0.9f);
                    Gore.NewGore(Projectile.GetSource_Death(), projectile.BottomRight, Projectile.velocity * -0.8f, ModContent.Find<ModGore>(Mod.Name, "Spazgore2").Type, 0.9f);
                    Gore.NewGore(projectile.GetSource_Death(), projectile.TopRight, projectile.velocity * 0.8f, ModContent.Find<ModGore>(Mod.Name, "Retigore1").Type, 0.9f);
                    Gore.NewGore(projectile.GetSource_Death(), projectile.BottomLeft, projectile.velocity * -0.8f, ModContent.Find<ModGore>(Mod.Name, "Retigore2").Type, 0.9f);
                    Gore.goreTime = 5;
                }
            }*/

            if (Projectile.ai[0] != 2)
            Projectile.rotation += 0.42f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 75, false, true));
            if (n.Alive())
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.type == ModContent.ProjectileType<Spazmarang>() && projectile.active && projectile.ai[0] == 2)
                    {
                        float scale = Main.rand.NextFloat(0.25f, 0.35f);
                        Particle p1 = new RectangleParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, scale, 25, true);
                        Particle p2 = new RectangleParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, scale, 25, true);
                        Particle p3 = new RectangleParticle(n.Center, ((n.Center - Projectile.Center) * 0.2f) + Main.rand.NextVector2Circular(5, 15), Color.Green, scale, 25, true);
                        p1.Spawn();
                        p2.Spawn();
                        p3.Spawn();


                        SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
                        target.AddBuff(BuffID.CursedInferno, 120);
                        return;
                            
                    }
                }
                
            }
            if (n.Alive() && (n.type != NPCID.GolemFistLeft || n.type != NPCID.GolemFistRight || n.type != ModContent.NPCType<CrystalLeaf>()))
            {
                if (Projectile.timeLeft >= 10)
                {
                    Projectile.ai[0] = 2;
                }
            }
            target.AddBuff(BuffID.CursedInferno, 120);
            
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.tileCollide = false;
            //Projectile.ai[0] = 1;
            return false;
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
            Texture2D SpazmaSaw = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/SpazmarangSaw").Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Rectangle rectangle2 = new(0, y3, SpazmaSaw.Width, num156);
            Vector2 origin22 = rectangle2.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            float scale = 1;
            scale = MathHelper.Lerp(Projectile.scale, 2f, 0.5f);

            float opacity = 1;
            opacity = MathHelper.Lerp(0, 1, 0.1f);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.4f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] <= 2)
            {
                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            }


            /*if (Projectile.ai[0] == 2)
            {
                Main.EntitySpriteDraw(SpazmaSaw, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), -Projectile.rotation, origin22, Projectile.scale * 1.3f, SpriteEffects.None, 0);
            }
            if (Projectile.ai[1] >= 35 || Projectile.ai[0] == 1)
            {
                Main.spriteBatch.Draw(SpazmaSaw, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin22, scale, SpriteEffects.None, 0);
            }*/

            return false;
        }
    }
}