﻿using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Pets
{
    public class BabyLifelight : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Pets", Name);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.QueenBeePet);
            AIType = ProjectileID.QueenBeePet;
            Projectile.width = 82;
            Projectile.height = 38;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].petFlagQueenBeePet = false;
            return true;
        }

        int realFrameCounter;
        int realFrame;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.dead)
            {
                modPlayer.BabyLifelight = false;
            }
            if (modPlayer.BabyLifelight)
            {
                Projectile.timeLeft = 2;
            }



            if (++realFrameCounter > 2)
            {
                realFrameCounter = 0;
                if (++realFrame >= Main.projFrames[Projectile.type])
                    realFrame = 0;
            }

            Projectile.frame = realFrame;

            Lighting.AddLight(Projectile.Center, Color.Pink.R / 255f, Color.Pink.G / 255f, Color.Pink.B / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
        /*public override void PostDraw(Color lightColor)
        {
            if (Projectile.frame != 2) //hide during the frame when it's behind the sprite
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                Texture2D star = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Effects/LifeStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle rect = new Rectangle(0, 0, star.Width, star.Height);
                float scale = 0.25f * Main.rand.NextFloat(0.5f, 1.25f);
                Vector2 origin = new Vector2((star.Width / 2) + scale, (star.Height / 2) + scale);

                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
                DrawData starDraw = new DrawData(star, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.HotPink).UseSecondaryColor(Color.HotPink);
                GameShaders.Misc["LCWingShader"].Apply(new DrawData?());
                starDraw.Draw(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

        }*/
    }
}