﻿using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class FossilBone : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_21";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bone);
            Projectile.DamageType = DamageClass.Default;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 0.2f;

            bool slowdown = true;
            if (++Projectile.localAI[2] > 90f)
            {
                if (player.active && !player.dead && !player.ghost)
                {
                    int distance = player.HasEffect<IronEquippedEffect>() ? 80 + (IronEquippedEffect.GrabRangeBonus(player) / 2) : 80;
                    if (player.Distance(Projectile.Center) < distance)
                    {
                        slowdown = false;
                        Projectile.velocity = Projectile.SafeDirectionTo(player.Center) * 9f;
                        Projectile.timeLeft++;

                        if (Projectile.Colliding(Projectile.Hitbox, player.Hitbox))
                        {
                            player.FargoSouls().HealPlayer(15);
                            Projectile.Kill();
                            FargoGlobalItem.OnRetrievePickup(player);
                            return;
                        }
                    }
                }
            }

            if (slowdown)
                Projectile.velocity *= 0.95f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.SandyBrown;
        }

        public override void OnKill(int timeLeft)
        {
            const int max = 16;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Dirt, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.myPlayer != Projectile.owner)
                return false;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Color.White;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 3f;
                Color glowColor = Color.White;

                Main.EntitySpriteDraw(texture, drawPosition + afterimageOffset, rectangle, glowColor, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, color, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}