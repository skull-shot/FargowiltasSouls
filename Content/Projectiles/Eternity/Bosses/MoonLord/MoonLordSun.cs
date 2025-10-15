using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.MoonLord
{
    public class MoonLordSun : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Bosses/MoonLord", Name);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 410;
            Projectile.height = 410;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.extraUpdates = 0;
            CooldownSlot = ImmunityCooldownID.Bosses;

            //Projectile.FargoSouls().GrazeCheck = projectile => CanDamage() && Projectile.Distance(Main.LocalPlayer.Center) < Math.Min(Projectile.width, Projectile.height) / 2 + Player.defaultHeight + Main.LocalPlayer.FargoSouls().GrazeRadius && Collision.CanHit(Projectile.Center, 0, 0, Main.LocalPlayer.Center, 0, 0);
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.penetrate = -1;

            Projectile.scale = 0.75f;
            Projectile.alpha = 255;

            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 120;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= Projectile.width / 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            //int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87, 0f, 0f, 0, Color.White, 6f);
            //Main.dust[d].noGravity = true;
            //Main.dust[d].velocity *= 4f;

            NPC core = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.MoonLordCore);
            NPC socket = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.MoonLordHand);
            if (socket == null || core == null || socket.ai[3] != core.whoAmI || core.ai[0] == 2f)
            {
                Projectile.Kill();
                return;
            }

            if (++Projectile.localAI[0] < 120) //prepare to throw down
            {
                //if (Projectile.localAI[0] == 1) Dusts();

                //use hand's x pos but core's y pos
                /*Vector2 targetPos = new Vector2(Main.npc[ai1].Center.X, Main.npc[ai0].Center.Y - 300);
                Projectile.velocity = (targetPos - Projectile.Center) / 30;
                Main.npc[ai1].Center = Projectile.Center + Projectile.velocity; //glue hand to me until thrown*/

                Vector2 targetPos = core.Center;
                targetPos.X += 36 * 16 * Math.Sign(socket.Center.X - core.Center.X);
                targetPos.Y -= 24 * 16;
                socket.Center = Vector2.Lerp(socket.Center, targetPos, 0.03f);

                Projectile.Center = socket.Center;
                Projectile.position.Y -= 250f * Math.Min(1f, Projectile.localAI[0] / 85);

                if (Projectile.localAI[0] < 60)
                {
                    Vector2 dustTarget = Projectile.Center + 10f * (socket.position - socket.oldPosition);

                    for (int i = 0; i < 10; i++)
                    {
                        if (!Main.rand.NextBool(4))
                        {
                            float f = Main.rand.NextFloat() * MathHelper.TwoPi;
                            float num2 = Main.rand.NextFloat();
                            Dust dust = Dust.NewDustPerfect(dustTarget + f.ToRotationVector2() * (110 * 4f + 110 * num2 * 8f), DustID.AmberBolt, (f - MathHelper.Pi).ToRotationVector2() * (10f * 4f + 8f * num2 * 8f));
                            dust.scale = 3.2f;
                            //dust.fadeIn = 0.3f + num2 * 0.3f;
                            dust.noGravity = true;
                        }
                    }
                }

                Projectile.alpha -= 3;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            else if (Projectile.localAI[0] == 120) //launch at player
            {
                //d = (v+0)*t/2
                //v = d*2/t
                Projectile.velocity = (Main.player[core.target].Center - Projectile.Center) * 2f / 90f;
                //0 = v+a*t
                //a = -v/t
                Projectile.localAI[1] = Projectile.velocity.Length() / 90f;

                Projectile.timeLeft = 91;
                Projectile.netUpdate = true;

                SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            }
            else
            {
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() - Projectile.localAI[1]);
                Projectile.alpha = 0;
            }

            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Projectile.scale = Main.rand.NextFloat(0.95f, 1.05f);
        }
        public override void OnKill(int timeLeft)
        {
            NPC core = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.MoonLordCore);
            NPC socket = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.MoonLordHand);
            if (socket == null || core == null || socket.ai[3] != core.whoAmI || core.ai[0] == 2f)
            {
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(), 0, 0f, Projectile.owner);
            }
            else
            {
                if (!Main.dedServ && Main.LocalPlayer.active)
                    ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);

                if (FargoSoulsUtil.HostCheck) //chain explosions
                {
                    //perpendicular
                    /*Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.velocity.ToRotation() + MathHelper.PiOver2, 5);
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.velocity.ToRotation() - MathHelper.PiOver2, 5);*/

                    const int max = 4; //spread
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 offset = Projectile.width / 2 * Vector2.UnitX.RotatedBy(Math.PI * 2 / max * i);
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(),
                            Projectile.damage, Projectile.knockBack, Projectile.owner, MathHelper.WrapAngle(offset.ToRotation()), 32);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DaybrokenBuff>(), 120);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.LightYellow;
            color.A = 0;
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color glow = Projectile.GetAlpha(lightColor);

            //for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            //{
            //    Vector2 value4 = Projectile.oldPos[i];
            //    float num165 = Projectile.oldRot[i];
            //    float ratio = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            //    Color color = glow * ratio;
            //    float scale = Projectile.scale * ratio;
            //    Main.spriteBatch.Draw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, num165, origin2, scale, SpriteEffects.None, 0);
            //}
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow * 0.35f, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}