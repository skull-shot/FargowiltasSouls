using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.LunaticCultist
{
    public class CultistFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_468";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            if (--Projectile.ai[0] < 0)
            {
                Projectile.Kill();
                return;
            }

            Lighting.AddLight(Projectile.Center, 1.1f, 0.9f, 0.4f);

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (++Projectile.localAI[0] == 12) //loads of vanilla dust :echprime:
            {
                Projectile.localAI[0] = 0.0f;
                for (int index1 = 0; index1 < 12; ++index1)
                {
                    Vector2 vector2 = (Vector2.UnitX * -Projectile.width / 2f + -Vector2.UnitY.RotatedBy(index1 * 3.14159274101257 / 6.0, new Vector2()) * new Vector2(8f, 16f)).RotatedBy(Projectile.rotation - (float)Math.PI / 2, new Vector2());
                    int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Shadowflame, 0.0f, 0.0f, 160, new Color(), 1f);
                    Main.dust[index2].scale = 1.1f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = Projectile.Center + vector2;
                    Main.dust[index2].velocity = Projectile.velocity * 0.1f;
                    Main.dust[index2].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[index2].position) * 1.25f;
                }
            }
            if (Main.rand.NextBool(4))
            {
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    Vector2 vector2 = -Vector2.UnitX.RotatedByRandom(0.196349546313286).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1f);
                    Main.dust[index2].velocity *= 0.1f;
                    Main.dust[index2].position = Projectile.Center + vector2 * Projectile.width / 2f;
                    Main.dust[index2].fadeIn = 0.9f;
                }
            }
            if (Main.rand.NextBool(32))
            {
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    Vector2 vector2 = -Vector2.UnitX.RotatedByRandom(0.392699092626572).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 155, new Color(), 0.8f);
                    Main.dust[index2].velocity *= 0.3f;
                    Main.dust[index2].position = Projectile.Center + vector2 * Projectile.width / 2f;
                    if (Main.rand.NextBool())
                        Main.dust[index2].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool())
            {
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    Vector2 vector2 = -Vector2.UnitX.RotatedByRandom(0.785398185253143).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0.0f, 0.0f, 0, new Color(), 1.2f);
                    Main.dust[index2].velocity *= 0.3f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = Projectile.Center + vector2 * Projectile.width / 2f;
                    if (Main.rand.NextBool())
                        Main.dust[index2].fadeIn = 1.4f;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.ShadowFlame, 120);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                Projectile.penetrate = -1;
                Projectile.position = Projectile.Center;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Projectile.width = Projectile.height = 176;
                Projectile.Center = Projectile.position;
                //Projectile.Damage();
                for (int index1 = 0; index1 < 4; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                }
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0.0f, 0.0f, 200, new Color(), 3.7f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity *= 3f;
                    int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index3].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Dust dust2 = Main.dust[index3];
                    dust2.velocity *= 2f;
                    Main.dust[index3].noGravity = true;
                    Main.dust[index3].fadeIn = 2.5f;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0.0f, 0.0f, 0, new Color(), 2.7f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 3f;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 0, new Color(), 1.5f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 3f;
                }
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    int index2 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Gore gore = Main.gore[index2];
                    gore.velocity *= 0.3f;
                    Main.gore[index2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                    Main.gore[index2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
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