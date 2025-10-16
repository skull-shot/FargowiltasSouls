using FargowiltasSouls.Assets.Textures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Cavern
{
    public class TimMagicBlast : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_30";

        public ref float state => ref Projectile.ai[0];
        public ref float timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.maxPenetrate = -1;
        }

        public override void AI()
        {
            base.AI();

            timer++;
            switch(state)
            {
                case 0:
                    if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height) || (timer > 60 || Main.player.Any(p => p.active && !p.dead && !p.ghost && p.Distance(Projectile.Center) < 75)))
                    {
                        SoundEngine.PlaySound(SoundID.Item104, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                        Projectile.velocity *= 0;
                        Projectile.timeLeft = 240;
                        float mult = 7f;
                        Projectile.width = (int)(Projectile.width * mult);
                        Projectile.height = (int)(Projectile.height * mult);
                        Projectile.position -= Projectile.Size / 2;
                        state = 1;
                        timer = 0;
                    }
                    else
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame);
                    }
                    break;
                case 1:
                    if (timer <= 20)
                    {
                        Projectile.ai[2] = (timer * Projectile.width) / 40;
                        Projectile.Opacity = (timer / 20);
                    }
                    Projectile.velocity *= 0;
                    if (Projectile.timeLeft <= 20)
                    {
                        state = 2;
                        timer = 0;
                    }
                    break;
                case 2:
                    Projectile.ai[2] = Projectile.width - (timer * Projectile.width) / 40;
                    Projectile.Opacity = 0.8f - (timer / 20);
                    break;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            target.AddBuff(BuffID.ShadowFlame, 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (state == 0)
                return base.PreDraw(ref lightColor);

            Vector2 auraPos = Projectile.Center;
            float radius = Projectile.ai[2];
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.WavyNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;
            var maxOpacity = Projectile.Opacity;

            Vector4 darkColor = Color.DarkViolet.ToVector4();
            Vector4 midColor = Color.Purple.ToVector4();
            Vector4 lColor = Color.Pink.ToVector4();


            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.NatureExplosionTelegraphShader");
            borderShader.TrySetParameter("darkColor", darkColor);
            borderShader.TrySetParameter("midColor", midColor);
            borderShader.TrySetParameter("lightColor", lColor);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            borderShader.TrySetParameter("radius", radius);
            borderShader.TrySetParameter("anchorPoint", auraPos);
            borderShader.TrySetParameter("screenPosition", Main.screenPosition);
            borderShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            borderShader.TrySetParameter("maxOpacity", maxOpacity);

            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, borderShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
