using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosRitual : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        private const float maxSize = 1200f;
        private const float minSize = 600f;

        public CosmosRitual() : base(MathHelper.Pi / 140f, 1000f, ModContent.NPCType<CosmosChampion>()) { }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Seal");
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.hide = true;
        }
        protected override void Movement(NPC npc)
        {
            Projectile.Center = npc.Center;

            float scaleModifier = npc.life / (npc.lifeMax * 0.2f);
            if (scaleModifier > 1f)
                scaleModifier = 1f;
            if (scaleModifier < 0f)
                scaleModifier = 0f;

            float targetSize = minSize + (maxSize - minSize) * scaleModifier;
            if (threshold > targetSize)
            {
                threshold -= 4;
                if (threshold < targetSize)
                    threshold = targetSize;
            }
            if (threshold < targetSize)
            {
                threshold += 4;
                if (threshold > targetSize)
                    threshold = targetSize;
            }
        }
        float speed = 17;
        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], npcType);
            if (npc != null)
            {
                Projectile.alpha -= increment;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                Movement(npc);

                targetPlayer = npc.target;

                Player player = Main.LocalPlayer;
                if (player.active && !player.dead && !player.ghost && Projectile.Center != player.Center && Projectile.Distance(player.Center) < 3000)
                {
                    float mult = 0.5f;
                    mult += 0.5f * LumUtils.InverseLerp(1200, 600, threshold);

                    // scales 0 to 1 as life goes down
                    float phaseHealthRatioRemaining = 1f - MathHelper.Min(npc.life, npc.lifeMax / 5f) / (npc.lifeMax / 5f);
                    float dragMultiplier = 1f + 0.5f * phaseHealthRatioRemaining;

                    float dragSpeed = mult * dragMultiplier * Projectile.Distance(player.Center) / 55;
                    player.position += Projectile.DirectionFrom(player.Center) * dragSpeed;
                    player.AddBuff(ModContent.BuffType<LowGroundEridanusBuff>(), 2);
                    player.wingTime = 60;
                }
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.alpha += increment;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.timeLeft = 2;
            Projectile.scale = (1f - Projectile.alpha / 255f) * 2f;
            Projectile.ai[0] += rotationPerTick;
            if (Projectile.ai[0] > MathHelper.Pi)
            {
                Projectile.ai[0] -= 2f * MathHelper.Pi;
                Projectile.netUpdate = true;
            }
            else if (Projectile.ai[0] < -MathHelper.Pi)
            {
                Projectile.ai[0] += 2f * MathHelper.Pi;
                Projectile.netUpdate = true;
            }

            Projectile.localAI[0] = threshold;
        }
        public override void PostAI()
        {
            base.PostAI();
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCs.Add(index);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < 220;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
            target.AddBuff(BuffID.Electrified, 360);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 auraPos = Projectile.Center + new Vector2(0f, Projectile.gfxOffY);
            float mult = 0.25f;
            mult += 1f * LumUtils.InverseLerp(1200, 600, threshold);
            float radius = threshold * mult * 0.3f;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.WavyNoise;
            if (!blackTile.IsLoaded) // || !diagonalNoise.IsLoaded)
                return false;
            var maxOpacity = Projectile.Opacity;

            Vector4 shaderColor = Color.Cyan.ToVector4();
            shaderColor.W = 1;
            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.BlackHoleShader");
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly / 2);
            borderShader.TrySetParameter("radius", radius);
            borderShader.TrySetParameter("midColor", shaderColor);
            borderShader.TrySetParameter("anchorPoint", auraPos);
            borderShader.TrySetParameter("screenPosition", Main.screenPosition);
            borderShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            borderShader.TrySetParameter("maxOpacity", maxOpacity);
            borderShader.TrySetParameter("mults", new Vector2(0.125f, 14));

            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, borderShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}