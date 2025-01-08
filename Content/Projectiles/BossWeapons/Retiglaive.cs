using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Projectiles.Minions;
using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class Retiglaive : ModProjectile, IPixelatedPrimitiveRenderer
    {
        bool empowered = false;

        public bool DrawTrail = false;
        public Vector2 mousePos = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Retiglaive");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(empowered);
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            empowered = reader.ReadBoolean();
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
                mousePos = buffer;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.light = 0.4f;

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage() => Projectile.ai[0] == 3 ? true : false;

        public override bool PreAI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;
                Projectile.rotation += Projectile.direction * -0.4f;

                if (Projectile.ai[1] <= 50)
                {
                    int buh = empowered ? 10 : 15;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.1f);
                    //fire lasers at cursor
                    if (Projectile.ai[1] % buh == buh - 3) // mp sync
                        Projectile.netUpdate = true;
                    if (Projectile.ai[1] % buh == 0)
                    {
                        Vector2 cursor = mousePos;
                        Vector2 velocity = Vector2.Normalize(cursor - Projectile.Center);

                        if (Projectile.ai[1] > 10)
                            velocity = velocity.RotatedByRandom(Math.PI / 24);

                        float num = 24f;
                        for (int index1 = 0; index1 < num; ++index1)
                        {
                            int type = 235;

                            Vector2 v = (Vector2.UnitX * 0.0f + -Vector2.UnitY.RotatedBy(index1 * (MathHelper.TwoPi / num), new Vector2()) * new Vector2(1f, 4f)).RotatedBy(velocity.ToRotation());
                            int index2 = Dust.NewDust(Projectile.Center, 0, 0, type, 0.0f, 0.0f, 150, new Color(255, 153, 145), 1f);
                            Main.dust[index2].scale = 1.5f;
                            Main.dust[index2].fadeIn = 1.3f;
                            Main.dust[index2].noGravity = true;
                            Main.dust[index2].position = Projectile.Center + v * Projectile.scale * 1.5f;
                            Main.dust[index2].velocity = v.SafeNormalize(Vector2.UnitY);
                        }

                        Player player = Main.player[Projectile.owner];
                        if (!player.Alive())
                        {
                            Projectile.Kill();
                            return false;
                        }

                        Projectile.velocity = -velocity * 8;

                        if (!empowered)
                        {
                            SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                            if (FargoSoulsUtil.HostCheck)
                            {
                                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(mousePos - Projectile.Center) * 20, ModContent.ProjectileType<PrimeLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                if (p != Main.maxProjectiles)
                                {
                                    Main.projectile[p].DamageType = DamageClass.Melee;
                                    Main.projectile[p].tileCollide = false;
                                    Main.projectile[p].localNPCHitCooldown = 30;
                                    Main.projectile[p].usesLocalNPCImmunity = true;
                                }
                            }
                        }
                        else if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<RetiDeathray>(), Projectile.damage, 1f, Projectile.owner, 0, Projectile.identity, Projectile.Distance(mousePos) + 200);
                        }
                    }
                }

                if (Projectile.ai[1] > 60)
                {
                    Projectile.ai[1] = 15;
                    Projectile.ai[0] = 2;
                }

                return false;
            }

            return true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == ModContent.ProjectileType<Spazmaglaive>())
            {
                empowered = true;
                Projectile.ai[0] = 0;
            }
            else if (Projectile.ai[0] == ModContent.ProjectileType<Retiglaive>())
            {
                Projectile.ai[0] = 0;
            }

            //travelling out
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] > 30)
                {
                    //Projectile.velocity /= 3;
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }
            //travel back to player
            else if (Projectile.ai[0] == 2)
            {   

                Projectile.ai[1] += 0.6f;
                //Projectile.extraUpdates = (Projectile.ai[1] < 40) ? 0 : 1;
                float lerpspeed = Projectile.ai[1] < 40 ? 0.07f : 0.3f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * Projectile.ai[1], lerpspeed);

                //kill when back to player
                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 30)
                    Projectile.Kill();
            }

            //spin
            Projectile.rotation += Projectile.direction * -0.4f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
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
            Texture2D flare2 = MiscTexturesRegistry.BloomFlare.Value;
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            if (empowered)
            {
                Main.spriteBatch.Draw(flare2, Projectile.Center - Main.screenPosition, null, Color.Red with { A = 0 } * Projectile.Opacity, Main.GlobalTimeWrappedHourly * -2f, flare2.Size() * 0.5f, 0.2f, 0, 0f);
                DrawTrail = true;
            }
            

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                if (i < 4)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27 * 0.4f, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.width * 0.8f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkRed, Color.Red, completionRatio);
        }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.Techno1Noise.Value);
            if (DrawTrail)
            {
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 25);
            }
            
        }
    }
}