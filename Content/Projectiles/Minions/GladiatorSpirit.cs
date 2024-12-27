using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class GladiatorSpirit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 60;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 60 * 60;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public const int AuraSize = 500;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            FargoSoulsPlayer localModPlayer = Main.LocalPlayer.FargoSouls();

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.HasEffect<WillEffect>() || !player.HasEffect<GladiatorBanner>()))
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 60;


            // movement
            int deadzone = 500;
            float distance = Projectile.Distance(player.Center);
            if (distance > deadzone-100)
            {
                float distanceFactor = (distance - deadzone);
                distanceFactor /= 6000;
                //if (distanceFactor > 1)
                    //distanceFactor = 1;
                float speed = 60 * distanceFactor;
                if (speed < 7)
                    speed = 7;
                Vector2 desiredVel = Projectile.DirectionTo(player.Center) * speed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVel, 0.05f);
            }
            else
            {
                Projectile.velocity *= 0.97f;
            }

            //FargoSoulsUtil.AuraDust(Projectile, AuraSize, DustID.GoldCoin);
            if (FargoSoulsUtil.ClosestPointInHitbox(Main.LocalPlayer.Hitbox, Projectile.Center).Distance(Projectile.Center) < AuraSize && player.HasEffect<GladiatorBanner>() && !localModPlayer.Purified)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<GladiatorSpiritBuff>(), 2);
            }

            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X.NonZeroSign();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color darkColor = Color.DarkGoldenrod;
            Color mediumColor = Color.Goldenrod;
            Color lightColor2 = Color.Lerp(Color.LightGoldenrodYellow, Color.White, 0.35f);

            Vector2 auraPos = Projectile.Center;
            var target = Main.LocalPlayer;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargosTextureRegistry.WavyNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;
            var maxOpacity = ModContent.GetInstance<FargoClientConfig>().TransparentFriendlyProjectiles;

            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.GenericInnerAura");
            borderShader.TrySetParameter("colorMult", 7.35f);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            borderShader.TrySetParameter("radius", AuraSize);
            borderShader.TrySetParameter("anchorPoint", auraPos);
            borderShader.TrySetParameter("screenPosition", Main.screenPosition);
            borderShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            borderShader.TrySetParameter("playerPosition", target.Center);
            borderShader.TrySetParameter("maxOpacity", maxOpacity);
            borderShader.TrySetParameter("darkColor", darkColor.ToVector4());
            borderShader.TrySetParameter("midColor", mediumColor.ToVector4());
            borderShader.TrySetParameter("lightColor", lightColor2.ToVector4());
            borderShader.TrySetParameter("opacityAmp", 1f);

            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, borderShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return base.PreDraw(ref lightColor);
        }
    }
}
