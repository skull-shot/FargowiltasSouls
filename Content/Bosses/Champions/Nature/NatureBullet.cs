using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Nature
{
    public class NatureBullet : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/SniperBullet";

        public int stopped;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nature Bullet");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ExplosiveBullet);
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 7;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.localAI[1] = Projectile.velocity.Length();
                SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
            }

            Projectile.Opacity = 1f;

            if (--Projectile.ai[0] < 0 && Projectile.ai[0] > -40 * Projectile.MaxUpdates)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.Opacity = 0f;
                //Projectile.hide = true;

                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Vortex, Scale: 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                }
            }
            else if (Projectile.ai[0] < -40 * Projectile.MaxUpdates && Projectile.velocity == Vector2.Zero)
            {
                SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1)
                    Projectile.velocity = Projectile.SafeDirectionTo(Main.player[p].Center) * Projectile.localAI[1];
                else
                    Projectile.Kill();
            }
            else if (Projectile.ai[0] < -40 * Projectile.MaxUpdates)
            {
                Projectile.tileCollide = true;
                Projectile.ignoreWater = false;
            }

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard, 0f, 0f, 0, default, 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }

            if (FargoSoulsUtil.HostCheck)
            {
                for (int index = 0; index < 6; ++index)
                {
                    float SpeedX = -Projectile.velocity.X * Main.rand.Next(30, 60) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -Projectile.velocity.Y * Main.rand.Next(30, 60) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, SpeedX, SpeedY,
                        ModContent.ProjectileType<SniperBulletShard>(), 0, 0f, Projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int p = Player.FindClosest(Projectile.Center, 0, 0);
            if (--Projectile.ai[0] < 0 && Projectile.ai[0] > -40 * Projectile.MaxUpdates && p != -1)
            {
                Asset<Texture2D> line = TextureAssets.Extra[178];
                float opacity = LumUtils.InverseLerp(0, -40 * Projectile.MaxUpdates, Projectile.ai[0]);
                Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Color.Cyan * opacity, Projectile.DirectionTo(Main.player[p].Center).ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(1f, 2), SpriteEffects.None);
            }

            return base.PreDraw(ref lightColor);
        }
    }
}