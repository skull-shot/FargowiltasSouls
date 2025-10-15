﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class LifeRuneHitbox : ModProjectile, IPixelatedPrimitiveRenderer
    {

        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1;
            Projectile.timeLeft = 6000;
        }
        public override bool? CanDamage() => Timer > 15 ? base.CanDamage() : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //line collision, needed because of the speed they move at when creating the arena, to form a solid wall
        {
            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.oldPos[1], Projectile.width, ref collisionPoint))
            {
                return true;
            }
            return false;
        }
        private Color GetColor()
        {
            int i = (int)Projectile.ai[1];
            if (i % 3 == 0) //cyan
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.UltraBrightTorch);
                return Color.Cyan;
            }
            else if (i % 3 == 1) //yellow
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.YellowTorch);
                return Color.Goldenrod;
            }
            else //pink
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.PinkTorch);
                return Color.DeepPink;
            }
        }
        public int Timer = 0;
        public override void AI()
        {
            NPC lifelight = Main.npc[(int)Projectile.ai[0]];
            if (!lifelight.TypeAlive<Lifelight>())
            {
                Projectile.Kill();
            }
            float RuneDistance = lifelight.localAI[0];
            float BodyRotation = lifelight.localAI[1];
            int RuneCount = (int)lifelight.localAI[2];

            int i = (int)Projectile.ai[1];

            float runeRot = (float)(BodyRotation + Math.PI * 2 / RuneCount * i);
            Vector2 runePos = lifelight.Center + runeRot.ToRotationVector2() * RuneDistance;
            Projectile.rotation = runeRot + MathHelper.PiOver2;
            Projectile.Center = runePos;

            Timer++;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<SmiteBuff>(), 60 * 4);
        }       
        public override bool PreDraw(ref Color lightColor)
        {
            int i = (int)Projectile.ai[1];
            AtlasTexture RuneTexture = AtlasManager.GetTexture("FargowiltasSouls." + $"Rune{i + 1}");

            //rune glow
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                Color glowColor;

                if (i % 3 == 0) //cyan
                    glowColor = new Color(0f, 1f, 1f, 0f) * 0.7f;
                else if (i % 3 == 1) //yellow
                    glowColor = new Color(1f, 1f, 0f, 0f) * 0.7f;
                else //pink
                    glowColor = new Color(1, 192 / 255f, 203 / 255f, 0f) * 0.7f;
            
                Utilities.Draw(Main.spriteBatch, RuneTexture, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, RuneTexture.Size * 0.5f, new Vector2(Projectile.scale), SpriteEffects.None);
            }

            Utilities.Draw(Main.spriteBatch, RuneTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(RuneTexture.Width / 2, RuneTexture.Height / 2), new Vector2(Projectile.scale), SpriteEffects.None);
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(GetColor(), Color.Transparent, completionRatio) * 0.7f;
        }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargoAssets.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }

    }
}
