using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class LifeSlurp : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Assets/ExtraTextures/LifelightParts/ShardGem1";

        private int rGem = 1;

        public bool home = true;

        public bool homingonPlayer;

        public bool chosenDirection;

        public bool First = true;

        public NPC lifelight;

        private int RotDirect = 1;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gem");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.light = 0.5f;
            Projectile.scale = 2f;
        }
        public override bool? CanDamage() => Projectile.ai[0] >= 30f ? base.CanDamage() : false;
        public override void AI()
        {
            if (Projectile.ai[0] > 30 && !First && !lifelight.TypeAlive<LifeChallenger>())
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Main.rand.Next(100);
                RotDirect = Main.rand.NextBool(2) ? -1 : 1;
                rGem = Main.rand.Next(1, 9);
            }
            Projectile.rotation += 0.2f * RotDirect;


            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 4;
            }
            if (Projectile.ai[0] > 30f)
            {
                if (First)
                {
                    lifelight = Main.npc[(int)Projectile.ai[1]];
                    if (!lifelight.TypeAlive<LifeChallenger>())
                    {
                        Projectile.Kill();
                        return;
                    }
                    Projectile.ai[1] = 0;
                    First = false;
                }
                Vector2 vectorToIdlePosition = Projectile.Center;
                float speed = 8f;
                float inertia = 5f;
                if (Projectile.ai[1] <= 1000f)
                {
                    vectorToIdlePosition = lifelight.Center - Projectile.Center;
                    float maxSpeed = 10f;
                    speed = ((Projectile.ai[0] - 30) / 15) * maxSpeed;
                    speed = MathHelper.Clamp(speed, 0, maxSpeed);
                    if (Projectile.ai[1] > 50)
                    {
                        speed = 16f;
                        inertia = 2f;
                    }
                        
                }
                else
                {
                    if (lifelight.HasPlayerTarget)
                    {
                        Player Player = Main.player[lifelight.target];
                        vectorToIdlePosition = Player.Center - Projectile.Center;
                    }
                    speed = 12f;
                    homingonPlayer = true;
                    home = false;
                }
                float num = vectorToIdlePosition.Length();
                if (homingonPlayer)
                {
                    home = false;
                }
                if (num < 20f)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.velocity = (lifelight.Center - Projectile.Center) * 1f;
                }
                if (num > 20f && home)
                {
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
                if (!home && homingonPlayer && !chosenDirection)
                {
                    if (!WorldSavingSystem.EternityMode)
                        Projectile.Kill();
                    vectorToIdlePosition.Normalize();
                    float amplitude = 0.6f;
                    if (!WorldSavingSystem.EternityMode)
                        amplitude = 1f;
                    vectorToIdlePosition = vectorToIdlePosition.RotatedBy(MathF.Sin(MathF.PI * Projectile.ai[2] * 0.16f) * amplitude);
                    vectorToIdlePosition *= 15;
                    Projectile.velocity = vectorToIdlePosition;
                    chosenDirection = true;
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);
                }
                    
            }
            if (Projectile.ai[0] > 600f)
            {
                Projectile.Kill();
            }
            Projectile.ai[0] += 1f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.SmiteBuff>(), 60 * 3);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>($"FargowiltasSouls/Assets/ExtraTextures/LifelightParts/ShardGem{rGem}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.Purple * Projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
