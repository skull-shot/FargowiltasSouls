using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.ChallengerItems
{
    public class DecrepitAirstrikeNuke : TrojanAcorn
    {
        public static readonly int ExplosionDiameter = 450;
        public override string Texture => "FargowiltasSouls/Content/Bosses/BanishedBaron/BaronNuke";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.sentry = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 60 * 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; //only hits once
        }

        public ref float TargetX => ref Projectile.ai[0];
        public ref float TargetY => ref Projectile.ai[1];
        public ref float TimeLeft => ref Projectile.ai[2];


        private Vector2 origPos = Vector2.Zero;
        private bool firstTick = true;

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.numHits > 4) return false;
            else return true;
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
        public override void AI()
        {
            if (firstTick)
            {
                if (Projectile.timeLeft > TimeLeft)
                {
                    Projectile.timeLeft = (int)TimeLeft + 3;
                }
                origPos = Projectile.Center;
                firstTick = false;
                Projectile.netUpdate = true;
            }
            Vector2 target = TargetX * Vector2.UnitX + TargetY * Vector2.UnitY;
            Projectile.rotation = (-Projectile.SafeDirectionTo(target)).ToRotation();
            
            if (Projectile.timeLeft <= 3)
            {
                Projectile.width = ExplosionDiameter;
                Projectile.height = ExplosionDiameter;
                Projectile.Center = target;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.Center = Vector2.Lerp(origPos, target, (TimeLeft - (Projectile.timeLeft - 3)) / TimeLeft);
            }
            if (Projectile.timeLeft % 5 == 0)
            {
                SoundEngine.PlaySound(FargosSoundRegistry.NukeBeep, Projectile.Center);
            }
        }

        public override void OnKill(int timeLeft)
        {
            ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);

            foreach (Projectile p in Main.projectile.Where(p => p.active && !p.hostile && p.owner == Main.myPlayer && p.minion && p.minionSlots > 0 && FargoSoulsUtil.IsSummonDamage(p, false, false) && Projectile.Colliding(Projectile.Hitbox, p.Hitbox)))
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 15f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), p.Center, p.velocity + vel, ModContent.ProjectileType<DecrepitAirstrikeNukeSplinter>(), (int)(Projectile.damage / 6 * p.minionSlots), Projectile.knockBack / 10, Projectile.owner);
                }
                p.Kill();

                SoundEngine.PlaySound(SoundID.Item67, p.Center);
            }

            for (int i = 0; i < 100; i++)
            {
                Vector2 pos = Projectile.Center + new Vector2(0, Main.rand.NextFloat(ExplosionDiameter * 0.8f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)); //circle with highest density in middle
                int d = Dust.NewDust(pos, 0, 0, DustID.Fireworks, 0f, 0f, 0, default, 1f);
                Main.dust[d].noGravity = true;
            }

            for (int j = 0; j < 20; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, (Vector2.UnitX * 5).RotatedByRandom(MathHelper.TwoPi), Main.rand.Next(61, 64), 2);
            }
            SoundEngine.PlaySound(SoundID.Item62 with { Pitch = -0.2f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft <= 2) //if exploding
            {
                return false;
            }
            /*
            //draw glow ring
            float modifier = Projectile.localAI[0] / Projectile.ai[0];
            Color RingColor = Color.Lerp(Color.Orange, Color.Red, modifier);
            Texture2D ringTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int ringy = ringTexture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            float RingScale = Projectile.scale * ExplosionDiameter / ringTexture.Height;
            int ringy3 = ringy * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle ringrect = new(0, ringy3, ringTexture.Width, ringy);
            Vector2 ringorigin = ringrect.Size() / 2f;
            RingColor *= modifier;
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(ringrect), RingColor, Projectile.rotation, ringorigin, RingScale, SpriteEffects.None, 0);
            */

            //draw projectile
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Width - Projectile.width) / 2;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + drawOffset + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }


            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);


            return false;
        }
    }
}