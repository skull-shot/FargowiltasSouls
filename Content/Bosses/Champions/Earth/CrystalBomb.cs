using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Earth
{
    public class CrystalBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Bomb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            //Projectile.tileCollide = false;
            //Projectile.ignoreWater = true;

            Projectile.alpha = 255;
            Projectile.hide = true;
            CooldownSlot = 1;

            Projectile.scale = 2.5f;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Main.rand.NextBool(2) ? 1f : -1f;
                Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2);
                Projectile.hide = false;
            }

            if (--Projectile.localAI[1] < 0)
            {
                Projectile.localAI[1] = 60;
                SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            }

            Projectile.alpha -= 10;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;

            Projectile.rotation += (float)Math.PI / 40f * Projectile.localAI[0];

            Lighting.AddLight(Projectile.Center, 0.3f, 0.75f, 0.9f);

            int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.NorthPole, 0.0f, 0.0f, 100, Color.Transparent, 1f);
            Main.dust[index3].noGravity = true;

            Projectile.velocity *= 1.03f;

            if (Projectile.Center.Y > Projectile.ai[0])
                Projectile.tileCollide = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);

            for (int index1 = 0; index1 < 40; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard, 0f, 0f, 0, default, 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }

            if (FargoSoulsUtil.HostCheck)
            {
                for (int index = 0; index < 7; ++index)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Main.rand.NextVector2Circular(12f, 12f),
                        ModContent.ProjectileType<CrystalBombShard>(), Projectile.damage, 0f, Projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Projectile.GetTexture();
            Vector2 drawPos = Projectile.GetDrawPosition();
            Rectangle frame = Projectile.GetDefaultFrame();
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12).ToRotationVector2() * 2f * Projectile.scale;
                Color glowColor = Color.DeepPink;

                Main.EntitySpriteDraw(texture, drawPos + afterimageOffset, frame, Projectile.GetAlpha(glowColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects);
            }
            Main.spriteBatch.ResetToDefault();
            Main.EntitySpriteDraw(texture, drawPos, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects);
            return false;
        }
    }
}