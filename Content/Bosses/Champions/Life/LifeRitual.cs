using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Life
{
    public class LifeRitual : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_467";

        public LifeRitual() : base(MathHelper.Pi / 140f, 1000f, ModContent.NPCType<LifeChampion>(), 87) { }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Life Seal");
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 4;
        }

        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] != 2f && npc.ai[0] != 8f)
            {
                Projectile.velocity = (npc.Center - Projectile.Center) / 30;
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
        }

        public override void AI()
        {
            base.AI();

            Projectile.rotation += 0.77f;
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 3)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<PurifiedBuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 auraPos = Projectile.Center;

            float leeway = Projectile.width / 2 * Projectile.scale;
            leeway *= 0.75f;
            float radius = threshold - leeway;

            var target = Main.LocalPlayer;

            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargosTextureRegistry.WavyNoise;

            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;

            var maxOpacity = Projectile.Opacity;
            float scale = Projectile.scale / 2;

            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.WoFAuraShader");
            borderShader.TrySetParameter("colorMult", 7.35f);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly * 2);
            borderShader.TrySetParameter("radius", radius * scale);
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
    }
}