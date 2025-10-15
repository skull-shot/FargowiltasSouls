using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantPenetratorNuke : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_645";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantasmal Blast");
            Main.projFrames[Projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 470 / 2;
            Projectile.height = 624 / 2;
            Projectile.aiStyle = -1;
            //AIType = ProjectileID.LunarFlare;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            //Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.scale = 1.5f;
            Projectile.alpha = 0;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().DeletionImmuneRank = 2;

            Projectile.hide = true;
        }

        public override bool? CanDamage() => Projectile.localAI[0] != 0;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            projHitbox.X += projHitbox.Width / 2;
            projHitbox.Y += projHitbox.Height / 2;
            projHitbox.Width = (int)(420 * Projectile.scale);
            projHitbox.Height = (int)(420 * Projectile.scale);
            projHitbox.X -= projHitbox.Width / 2;
            projHitbox.Y -= projHitbox.Height / 2;
            return null;
        }

        public override void AI()
        {
            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
                return;
            }

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                }
            }

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

                Projectile.hide = false;
                Projectile.position.Y -= 144 * Projectile.scale;

                if (!Main.dedServ && Main.LocalPlayer.active)
                    ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);

                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Thunder") { Volume = 0.8f, Pitch = 0.5f }, Projectile.Center);

                for (int a = 0; a < 4; a++)
                {
                    for (int index1 = 0; index1 < 3; ++index1)
                    {
                        int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                        Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                    }
                    for (int index1 = 0; index1 < 10; ++index1)
                    {
                        int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 2.5f);
                        Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                        Main.dust[index2].noGravity = true;
                        Dust dust1 = Main.dust[index2];
                        dust1.velocity *= 1f;
                        int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 100, new Color(), 1.5f);
                        Main.dust[index3].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                        Dust dust2 = Main.dust[index3];
                        dust2.velocity *= 1f;
                        Main.dust[index3].noGravity = true;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, default, 3f);
                        Main.dust[dust].velocity *= 1.4f;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 7f;
                        dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                        Main.dust[dust].velocity *= 3f;
                    }

                    for (int index1 = 0; index1 < 10; ++index1)
                    {
                        int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, new Color(), 2f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity *= 21f * Projectile.scale;
                        Main.dust[index2].noLight = true;
                        int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, new Color(), 1f);
                        Main.dust[index3].velocity *= 12f;
                        Main.dust[index3].noGravity = true;
                        Main.dust[index3].noLight = true;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3.5f));
                        if (Main.rand.NextBool(3))
                            Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= Main.rand.NextFloat(9f, 12f);
                        Main.dust[d].position = Main.player[Projectile.owner].Center;
                    }
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
        }
        public override void OnKill(int timeLeft)
        {
            
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Textures/Content/Projectiles/Weapons/PenetratorNuke/PenetratorNuke_" + Projectile.frame.ToString(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

