using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Fargowiltas.Common.Systems.InstaVisual;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class Tome : ModProjectile
    {

        /*
         * 0 = initial deployment
         * 1 = wind up
         * 2 = firing
         * 3 = despawning
         */
        public ref float state => ref Projectile.ai[0];
        public ref float projCount => ref Projectile.ai[1];
        public ref float timer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 54;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0.65f;
        }

        public override void AI()
        {
            float rot = (Main.MouseWorld - Projectile.Center).ToRotation();
            Frame();
            Projectile.rotation = rot + MathHelper.Pi;
            if (state == 3 || Projectile.timeLeft <= 20)
            {
                Projectile.velocity = Projectile.velocity * (0.98f);
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 20);
                Projectile.Opacity -= 1 / 40f;
                return;
            }
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, TorchID.Shimmer);
            if (state == 0)
            {
                if (player.altFunctionUse == 2 && player.HeldItem.type == ModContent.ItemType<DarkTome>())
                {
                    state = 1;
                }
                Vector2 vectorToMousePosition = Projectile.position - Main.MouseWorld;
                float f = Projectile.Center.Distance(Main.MouseWorld) / 2000f;
                
                if (Projectile.velocity.Length() <= 15.5f)
                    Projectile.velocity += f * Vector2.One.RotatedBy(vectorToMousePosition.ToRotation() + 3 * MathHelper.PiOver4);
                else
                    Projectile.velocity *= 0.95f;

                return;
            }

            // Timer only starts after right click
            timer++;
            if (state == 1 && timer < 8)
            {
                Projectile.velocity *= 0.90f;
            }
            if (state == 2 && projCount > 0 && timer % 5 == 0)
            {
                Vector2 vel = new Vector2(10f, 0f).RotatedBy(rot);
                SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);

                // Determine random spread from book count
                float spread = (MathHelper.Pi / 24) * (3 - player.ownedProjectileCounts[Type]);
                float randRot = Main.rand.NextFloat(-spread, spread);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel.RotatedBy(randRot),
                    ModContent.ProjectileType<TomeShot>(), Projectile.damage, 0f, Projectile.owner);
                
                // Self-knockback
                Projectile.velocity -= 0.15f * vel;

                Dust d = Dust.NewDustDirect(Projectile.position, 0, 0, DustID.PinkTorch);
                d.velocity = 0.2f * vel;

                projCount--;
                if (projCount == 0)
                {
                    state = 3;
                }
            }
        }

        void Frame()
        {
            switch (state)
            {
                case 0:
                    Projectile.frame = 0;
                    break;
                case 1:
                    if (timer % 3 == 0)
                    {
                        if (Projectile.frame < 4)
                            Projectile.frame++;
                        else
                            state = 2;
                    }
                    break;
                case 2:
                    if (timer % 4 == 0)
                    {
                        if (++Projectile.frame > 7)
                            Projectile.frame = 5;
                    }
                    break;
                case 3:
                    Projectile.frame = 4;
                    break;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Type].Value;


            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = Projectile.oldPos.Length - 1; k >= 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + origin2 + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture2D13, drawPos, rectangle, color, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            }

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}