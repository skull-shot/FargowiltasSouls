using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Projectiles.Minions;
using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class Retiglaive : ModProjectile
    {
        public Vector2 mousePos = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Retiglaive");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
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

            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage() => false;

        int counter;
        int returntime;
        public override void AI()
        {
            Player player = Main.LocalPlayer;
            if (Main.player[Projectile.owner].Alive())
            {
                if (Main.projectile.Where(p => p.TypeAlive(Type) && p.owner == Projectile.owner).Count() > 6)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] > 15)
                {
                    Projectile.ai[0] = 1.5f;
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.ai[0] == 1)
            {
                Projectile.extraUpdates = 0;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center) * 25, 0.2f);

                //kill when back to player
                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 25)
                    Projectile.Kill();

            }

            if (Projectile.ai[0] == 1.5f)
            {
                Projectile.extraUpdates = 0;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center) * 35, 0.2f);

                if (Projectile.Distance(Main.player[Projectile.owner].Center) <= 50)
                    Projectile.ai[0] = 2;
            }


            if (Projectile.ai[0] == 2)
            {
                int distance = 120;
                float spinFrames = 90;
                var moons = Main.projectile.Where(p => p.TypeAlive(Type) && p.owner == player.whoAmI).ToList();
                float offset = moons.IndexOf(Projectile) * MathF.Tau / moons.Count;
                float rotation = MathF.Tau * (Main.GameUpdateCount % spinFrames) / spinFrames;
                Projectile.Center = player.Center + new Vector2(30, 0) + Vector2.UnitX.RotatedBy(offset + rotation) * distance;

                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1600, false, true));
                if (n.Alive())
                {

                    if (++Projectile.ai[1] > 15)
                    {
                        if (Projectile.owner == Main.myPlayer && counter <= 14)
                        {
                            Vector2 velocity = Vector2.Normalize(n.Center - Projectile.Center);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<RetiDeathray>(), Projectile.damage, 1f, Projectile.owner, 0, Projectile.identity, Projectile.Distance(mousePos) + 320);
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
                    //Projectile.rotation = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI;
                }

                if (counter >= 14 && ++returntime >= 120)
                {
                    Projectile.velocity -= Vector2.Normalize(player.Center - Projectile.Center) * 5;
                    Projectile.ai[0] = 1;

                    Projectile.frame = 0;
                }
            }
                    /*travelling out
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
                    */

            //LAUGH
            Projectile.rotation += .22f;
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
    }
}