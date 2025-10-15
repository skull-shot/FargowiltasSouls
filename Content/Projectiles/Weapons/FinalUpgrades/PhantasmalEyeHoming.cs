using FargowiltasSouls.Content.Buffs.Eternity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.FinalUpgrades
{
    public class PhantasmalEyeHoming : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_452";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantasmal Eye");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            if (--Projectile.ai[1] < 0)
            {
                Projectile.tileCollide = true;

                if (Projectile.ai[0] == -1) //no target atm
                {
                    if (Projectile.ai[1] % 6 == 0)
                    {
                        Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2000, true);
                        Projectile.netUpdate = true;
                        if (Projectile.ai[0] == -1)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                }
                else //currently have target
                {
                    NPC npc = Main.npc[(int)Projectile.ai[0]];

                    if (npc.active && npc.CanBeChasedBy()) //target is still valid
                    {
                        Vector2 distance = npc.Center - Projectile.Center;
                        double angle = distance.ToRotation() - Projectile.velocity.ToRotation();
                        if (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        if (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        float modifier = Math.Min(Projectile.velocity.Length() / 100f, 1f);
                        Projectile.velocity = Projectile.velocity.RotatedBy(angle * modifier);
                    }
                    else //target lost, reset
                    {
                        Projectile.ai[0] = -1;
                        Projectile.netUpdate = true;
                    }
                }
            }

            if (Projectile.ai[1] < 0)
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * MathHelper.Lerp(Projectile.velocity.Length(), 24f, 0.02f);

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

            if (Projectile.localAI[0] < ProjectileID.Sets.TrailCacheLength[Projectile.type])
                Projectile.localAI[0] += 0.1f;
            else
                Projectile.localAI[0] = ProjectileID.Sets.TrailCacheLength[Projectile.type];

            Projectile.localAI[1] += 0.25f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 600);
            target.immune[Projectile.owner] = 1;
            Projectile.timeLeft = 0;
        }

        public override void OnKill(int timeleft)
        {
            SoundEngine.PlaySound(SoundID.Zombie103, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 144;
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;
            for (int index = 0; index < 2; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
                Main.dust[index3].noGravity = true;
            }

            if (Projectile.penetrate >= 0)
            {
                Projectile.penetrate = -1;
                Projectile.Damage();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantEye_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int rect1 = glow.Height / Main.projFrames[Projectile.type];
            int rect2 = rect1 * Projectile.frame;
            Rectangle glowrectangle = new(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(31, 187, 192, 0), Color.Transparent, 0.74f);
            Vector2 drawCenter = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14;

            for (int i = 0; i < 3; i++) //create multiple transparent trail textures ahead of the Projectile
            {
                Vector2 drawCenter2 = drawCenter + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 8).RotatedBy(MathHelper.Pi / 5 - i * MathHelper.Pi / 5); //use a normalized version of the Projectile's velocity to offset it at different angles
                drawCenter2 -= Projectile.velocity.SafeNormalize(Vector2.UnitX) * 8; //then move it backwards
                float scale = Projectile.scale;
                scale += (float)Math.Sin(Projectile.localAI[1]) / 10;
                Main.EntitySpriteDraw(glow, drawCenter2 - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale, SpriteEffects.None, 0);
            }

            for (float i = Projectile.localAI[0] - 1; i > 0; i -= Projectile.localAI[0] / 5) //trail grows in length as Projectile travels
            {

                float lerpamount = 0.2f;
                if (i > 5 && i < 10)
                    lerpamount = 0.4f;
                if (i >= 10)
                    lerpamount = 0.6f;

                Color color27 = Color.Lerp(glowcolor, Color.Transparent, 0.1f + lerpamount);

                color27 *= (int)((Projectile.localAI[0] - i) / Projectile.localAI[0]) ^ 2;
                float scale = Projectile.scale * (float)(Projectile.localAI[0] - i) / Projectile.localAI[0];
                scale += (float)Math.Sin(Projectile.localAI[1]) / 10;
                Vector2 value4 = Projectile.oldPos[(int)i] - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14;
                Main.EntitySpriteDraw(glow, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 0.8f, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

        }
    }
}