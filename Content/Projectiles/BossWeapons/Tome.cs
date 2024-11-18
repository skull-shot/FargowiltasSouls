using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class Tome : ModProjectile
    {

        /*
         * 0 = initial deployment
         * 1 = firing
         * 2 = despawning
         */
        public ref float state => ref Projectile.ai[0];
        public ref float projCount => ref Projectile.ai[1];
        public ref float timer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            float rot = (Main.MouseWorld - Projectile.Center).ToRotation();
            Projectile.rotation = rot + 5 * MathHelper.PiOver4;
            if (state == 2 || Projectile.timeLeft <= 20)
            {
                Projectile.velocity = Projectile.velocity * (0.98f);
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 20);
                Projectile.Opacity -= 1 / 20f;
                return;
            }
            Player player = Main.player[Projectile.owner];
            if (player.altFunctionUse == 2 && player.HeldItem.type == ModContent.ItemType<DarkTome>())
            {
                state = 1;
            }
            Lighting.AddLight(Projectile.Center, TorchID.Shimmer);
            if (state == 0)
            {
                Vector2 vectorToMousePosition = Projectile.position - Main.MouseWorld;
                float f = Projectile.Center.Distance(Main.MouseWorld) / 2000f;
                Projectile.velocity += f * Vector2.One.RotatedBy(vectorToMousePosition.ToRotation() + 3 * MathHelper.PiOver4);
                return;
            }
            if (projCount > 0 && timer++ % 5 == 0)
            {
                Vector2 vel = new Vector2(10f, 0f).RotatedBy(rot);
                SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);
                float spread = (MathHelper.Pi / 24) * (3 - player.ownedProjectileCounts[Type]);
                float randRot = Main.rand.NextFloat(-spread, spread);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel.RotatedBy(randRot),
                    ModContent.ProjectileType<TomeShot>(), Projectile.damage, 0f, Projectile.owner);
                Projectile.velocity -= 0.2f * vel;

                Dust d = Dust.NewDustDirect(Projectile.position, 0, 0, DustID.PinkTorch);
                d.velocity = 0.2f * vel;

                projCount--;
                if (projCount == 0)
                {
                    state = 2;
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Type].Value;

            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            int bookCount = Math.Min(player.ownedProjectileCounts[Type] - 1, 2);
            Vector2 drawOrigin = new Vector2(texture2D13.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / ((float)Projectile.oldPos.Length));
                float rot = Projectile.oldRot[k] + 3 * MathHelper.PiOver2;
                Main.EntitySpriteDraw(texture2D13, drawPos, null, color, rot, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Color color27 = Color.Lerp(color26, Color.Magenta, 0.5f);
            //Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, scale, spriteEffects, 0);
            // Normal Draw
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}