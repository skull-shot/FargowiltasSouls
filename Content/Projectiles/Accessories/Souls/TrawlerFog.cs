using Fargowiltas.Common.Configs;
using Fargowiltas.Content.Projectiles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class TrawlerFog : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public static int Duration => 170;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.scale = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Duration;
            Projectile.penetrate = -1;
            Projectile.Opacity = 1f;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[0];
        public override bool? CanDamage() => Projectile.Opacity > 0.2f ? null : false;
        public override bool? CanCutTiles() => false;
        public override bool? CanHitNPC(NPC target)
        {
            if (target.lifeMax <= 5 || target.CountsAsACritter || target.friendly)
                return false;
            else return base.CanHitNPC(target);
        }
        public override void AI()
        {
            Timer++;
            float halfDuration = Duration / 2;
            if (Timer < halfDuration)
            {
                Projectile.Opacity = Utils.SmoothStep(0, 1, Timer / halfDuration);
            }
            else
            {
                Projectile.Opacity = Utils.SmoothStep(1, 0, (Timer - halfDuration) / halfDuration);
            }
            bool movement = false;
            if (Target < 0) // has no target
            {
                NPC npc = Projectile.FindTargetWithinRange(450, false);
                if (npc != null && npc.Alive())
                {
                    Target = npc.whoAmI;
                    Projectile.netUpdate = true;
                }
            }
            else // has target, seek it out
            {
                NPC npc = Main.npc[(int)Target];
                if (npc != null && npc.Alive())
                {
                    Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, npc.Center, Projectile.velocity, 0.07f, 0.07f);
                    movement = true;
                }
                else
                    Target = -2;
            }
            if (!movement)
                Projectile.velocity *= 0.94f;

            if (Main.rand.NextBool(6))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PoisonStaff, Scale: 0.8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.3f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 auraPos = Projectile.Center;
            float radius = Projectile.width / 2;
            var target = Main.LocalPlayer;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.SmokyNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;
            var maxOpacity = Projectile.Opacity * ModContent.GetInstance<FargoClientConfig>().TransparentFriendlyProjectiles;


            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.TrawlerFogShader");
            borderShader.TrySetParameter("colorMult", 7.35f);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            borderShader.TrySetParameter("radius", radius);
            borderShader.TrySetParameter("anchorPoint", auraPos);
            borderShader.TrySetParameter("screenPosition", Main.screenPosition);
            borderShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            borderShader.TrySetParameter("playerPosition", target.Center);
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 240);
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(BuffID.Midas, 240);
        }
    }
}