using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class GladiatorStandard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 74;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 60 * 600;
        }
        ref float hits => ref Projectile.ai[2];
        public override bool? CanDamage()
        {
            return Projectile.velocity != Vector2.Zero && hits < 5; //only while travelling and hasn't hit more than 5 times
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hits++;
        }
        public static int AuraSize(Player player) => player.FargoSouls().ForceEffect<GladiatorEnchant>() ? 800 : 400;
        public override void AI()
        {
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.Alive())
            {
                Projectile.Kill();
                return;
            }
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            FargoSoulsPlayer localModPlayer = Main.LocalPlayer.FargoSouls();

            

            //FargoSoulsUtil.AuraDust(Projectile, AuraSize, DustID.GoldCoin);
            if (FargoSoulsUtil.ClosestPointInHitbox(Main.LocalPlayer.Hitbox, Projectile.Center).Distance(Projectile.Center) < AuraSize(player) && player.HasEffect<GladiatorBanner>() && !localModPlayer.Purified)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<GladiatorBuff>(), 2);
            }

            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            const int rootAmount = 8;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.position += Projectile.velocity;
            Projectile.position += Vector2.UnitY * rootAmount;
            Projectile.velocity = Vector2.Zero;

            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Projectile.Center + (Vector2.UnitY * Projectile.height / 2), 0, 0, DustID.Gold, Main.rand.NextFloat(-30, 30), -Main.rand.NextFloat(4, 8));
            }
            Projectile.tileCollide = false;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return true;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.Alive())
            {
                Projectile.Kill();
                return true;
            }
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
            borderShader.TrySetParameter("radius", AuraSize(player));
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
