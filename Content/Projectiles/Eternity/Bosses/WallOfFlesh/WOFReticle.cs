using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses.WallOfFlesh;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.WallOfFlesh
{
    public class WOFReticle : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Bosses/WallOfFlesh", Name);
        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 300;
            //CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        int additive = 130;

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;

            if (++Projectile.ai[0] < 0)
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha < 0) //fade in
                    Projectile.alpha = 0;

                int modifier = Math.Min(110, (int)Projectile.ai[0]);
                Projectile.scale = 4f - 3f / 110 * modifier; //start big, shrink down

                /*Projectile.Center = Main.npc[ai0].Center;
                Projectile.velocity = Main.player[Main.npc[ai0].target].Center - Projectile.Center;
                Projectile.velocity = Projectile.velocity / 60 * modifier; //move from npc to player*/
                Projectile.rotation = (float)Math.PI * 2f / 55 * modifier * Projectile.localAI[0];

                if (Projectile.ai[0] % 30 == 0)
                {
                    //if (!Main.dedServ)
                    //    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ReticleBeep"), Projectile.Center);
                }
            }
            else //if (Projectile.ai[0] < 145)
            {
                additive -= 7;
                if (additive < 0)
                    additive = 0;

                Projectile.alpha += 15;
                if (Projectile.alpha > 255) //fade out
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }

                Projectile.scale = 4f - 3f * Projectile.Opacity; //scale back up

                //if (Projectile.ai[0] == 130 && FargoSoulsUtil.HostCheck) Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -13);

                if (Projectile.ai[0] % 2 == 0 && Projectile.localAI[1]++ < 4)
                {
                    //Vector2 spawnPos = Projectile.Center;
                    //spawnPos.X += Main.rand.NextFloat(-250, 250);
                    //spawnPos.Y += Main.rand.NextFloat(700, 800) * Projectile.localAI[0];
                    int above = Projectile.localAI[1] % 2 == 0 ? 1 : -1;
                    float randAngle = (above * Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2 * 0.27f).ToRotation();
                    Vector2 spawnPos = Projectile.Center - randAngle.ToRotationVector2() * 900;

                    Vector2 vel = Main.rand.NextFloat(0.8f, 1.2f) * (Projectile.Center - spawnPos) / 90;
                    if (vel.Length() < 10f)
                        vel = Vector2.Normalize(vel) * Main.rand.NextFloat(10f, 15f);
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), spawnPos, vel / 3, ModContent.ProjectileType<WOFChain>(), Projectile.damage, 0f, Main.myPlayer, Projectile.Center.X, Projectile.Center.Y, randAngle);

                    FargoSoulsUtil.ScreenshakeRumble(4);

                    SoundEngine.PlaySound(SoundID.NPCDeath13 with { Volume = 0.5f }, Projectile.Center);

                    Projectile.localAI[0] *= -1;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, additive) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}