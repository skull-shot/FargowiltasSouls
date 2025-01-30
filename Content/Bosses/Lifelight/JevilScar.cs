using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class JevilScar : ModProjectile
    {
        //private bool init = false;

        //private NPC lifelight;

        //private Vector2 ScopeAtPlayer = Vector2.Zero;

        private float rotspeed = 0;

        public override string Texture => "FargowiltasSouls/Content/Projectiles/ChallengerItems/EnchantedLifebladeProjectile";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Enchanted Lightblade");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 54; //actually 56 but it's diagonal
            Projectile.height = 54; //actually 56 but it's diagonal
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //circular hitbox
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
        public ref float Timer => ref Projectile.ai[0];
        public ref float TargetID => ref Projectile.ai[1];
        public ref float LockedRotation => ref Projectile.ai[2];
        public ref float KillTimer => ref Projectile.localAI[0];
        public override void AI()
        {
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame %= 3;
                Projectile.frameCounter = 0;
            }
            Projectile.frameCounter++;

            if (++KillTimer > 1200 || NPC.CountNPCS(ModContent.NPCType<LifeChallenger>()) < 1) //set to 1200 at end of attack by Lifelight, then fades out
            {
                Projectile.alpha += 17;
                Projectile.hostile = false;
            }
            if (Projectile.alpha >= 240)
            {
                Projectile.Kill();
            }
            const int CycleTime = 105;
            const int DashTime = 30;
            const int TelegraphTime = 35;
            int targetID = (int)TargetID;
            if (!targetID.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return;
            }
            Player target = Main.player[targetID];
            if (!target.Alive())
            {
                Projectile.Kill();
                return;
            }

            if (Timer < DashTime) // dashing
            {
                Projectile.hostile = true;
                if (Timer < DashTime * 0.7f)
                {
                    rotspeed = 0;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                    if (KillTimer > DashTime) // not right after spawn
                        Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 1.8f;
                }
                else
                {
                    Projectile.velocity *= 0.925f;
                    if (rotspeed < MathHelper.Pi / 10)
                        rotspeed += MathHelper.Pi / 10 / 10;
                    Projectile.rotation += rotspeed;
                }
            }
            else if (Timer < CycleTime - TelegraphTime) // spinning
            {
                Projectile.hostile = false;
                if (Timer == DashTime)
                {
                    LockedRotation = target.DirectionTo(Projectile.Center).ToRotation();
                    LockedRotation += Main.rand.NextFloat(MathHelper.PiOver4 * 0.6f, MathHelper.PiOver2) * (Main.rand.NextBool() ? 1 : -1);
                    Projectile.netUpdate = true;
                }

                if (rotspeed < MathHelper.Pi / 10)
                    rotspeed += MathHelper.Pi / 10 / 10;
                Projectile.rotation += rotspeed;

                Vector2 dir = LockedRotation.ToRotationVector2();
                /*
                List<Projectile> swords = Main.projectile.Where(p => p.TypeAlive(Projectile.type)).ToList();
                int i = swords.IndexOf(Projectile);
                if (swords.Count > 0)
                {
                    i -= swords.Count / 2;
                    i /= swords.Count / 2;
                }
                dir = dir.RotatedBy(MathHelper.PiOver2 * 0.6f * i);
                */
                Vector2 desiredPos = target.Center + dir * 270f;
                Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, desiredPos, Projectile.velocity, 1.4f, 1.4f);
            }
            else // telegraphing new dash
            {
                Projectile.hostile = false;
                Projectile.velocity *= 0.93f;
                rotspeed = 0;
                float desiredRot = Projectile.DirectionTo(target.Center).ToRotation() + MathHelper.PiOver4;
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, desiredRot, 6f / TelegraphTime);

                int rearbackTime = 25;
                if (CycleTime - Timer < rearbackTime)
                {
                    float modifier = (Timer - (CycleTime - rearbackTime)) / (float)rearbackTime;
                    modifier = 1 - modifier;
                    Projectile.velocity -= Projectile.DirectionTo(target.Center) * 1.2f * modifier;
                }
                

                //Vector2 desiredPos = target.Center + target.DirectionTo(Projectile.Center) * 300f;
                //Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, desiredPos, Projectile.velocity, 2f, 2f);
            }
            if (Timer >= CycleTime) // dash
            {
                Timer = 0;
                Projectile.velocity = Projectile.DirectionTo(target.Center) * 4;
                SoundEngine.PlaySound(SoundID.Item71 with { Volume = 0.75f }, Projectile.Center);
            }

            Timer += 1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.SmiteBuff>(), 60 * 3);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100) * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.5f)
            {
                Color color27 = new Color(255, 51, 153) * Projectile.Opacity * 0.5f;
                color27.A = (byte)(color26.A / 2);
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);

            return false;
        }
    }
}
