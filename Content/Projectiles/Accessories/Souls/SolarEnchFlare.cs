using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Luminance.Core.Graphics;
using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.Textures;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Buffs.Souls;
using Terraria.Audio;
using Terraria.ID;
using System;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class SolarEnchFlare : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity;

        ref float Timer => ref Projectile.ai[0];

        ref float Decrement => ref Projectile.ai[1];

        ref float speed => ref Projectile.ai[2];

        public int hits = 0;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Timer = 0;
        }

        public override bool PreAI()
        {
            if (Timer % 14 == 0)
            {
                Decrement++;
                Projectile.Opacity = 1 - Decrement / speed;
            }
            //Projectile gets bigger (at a decreasing rate) over time
            Projectile.width += (int)(speed - Decrement);
            Projectile.height += (int)(speed - Decrement);
            if (Projectile.Opacity == 0)
            {
                Projectile.Kill();
                return false;
            }
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            Timer++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            hits += 1;
            if (hits > 1)
            {
                modifiers.FinalDamage *= 1f / hits;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item45 with {Pitch = -0.5f, Volume = 1.5f}, target.Center);
            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            if (!player.Alive() || !player.HasEffect<SolarFlareEffect>())
            {
                Projectile.Kill();
                return false;
            }

            Vector2 auraPos = Projectile.Center;
            float radius = Projectile.width / 2;
            var target = Main.LocalPlayer;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.WavyNoise;
            var maxOpacity = Projectile.Opacity * ModContent.GetInstance<FargoClientConfig>().TransparentFriendlyProjectiles;

            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.SolarEnchantShader");
            shader.TrySetParameter("colorMult", 7.35f);
            shader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            shader.TrySetParameter("radius", radius * Projectile.scale);
            shader.TrySetParameter("anchorPoint", auraPos);
            shader.TrySetParameter("screenPosition", Main.screenPosition);
            shader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            shader.TrySetParameter("playerPosition", target.Center);
            shader.TrySetParameter("maxOpacity", maxOpacity);


            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
