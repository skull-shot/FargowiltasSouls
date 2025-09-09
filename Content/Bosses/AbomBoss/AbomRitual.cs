using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomRitual : BaseArena
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/AbomBoss/AbomDeathScythe";

        private const float realRotation = MathHelper.Pi / 180f;
        public float VisualScale = 0f;

        public AbomRitual() : base(realRotation, 1400f, ModContent.NPCType<AbomBoss>(), 87, visualCount: 64) { }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hide = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }
        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] < 9 && npc.ai[0] != -3 && npc.ai[0] != 5)
            {
                Projectile.velocity = npc.Center - Projectile.Center;
                Projectile.velocity /= npc.ai[0] == 8 ? 10f : 40f; //snaps to abom faster when preparing for p2 attack

                rotationPerTick = realRotation;
            }
            else //remains still in higher AIs
            {
                Projectile.velocity = Vector2.Zero;

                rotationPerTick = -realRotation / 10f; //denote arena isn't moving
            }
        }

        public override void AI()
        {
            base.AI();
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], npcType);
            Projectile.rotation += 1f;
            if (Projectile.Opacity < 0.5f && npc != null)
                Projectile.Opacity = 0.5f;
            if (VisualScale < 1)
                VisualScale += 0.01f;

            if (!WorldSavingSystem.MasochistModeReal)
            {
                Player player = Main.LocalPlayer;
                if (player.active && !player.dead && !player.ghost)
                {
                    float distance = player.Distance(Projectile.Center);
                    if (distance > threshold && distance < threshold * 1.4f)
                    {
                        player.AddBuff(BuffID.Bleeding, 240);
                        if (WorldSavingSystem.EternityMode)
                            player.AddBuff(ModContent.BuffType<AbomFangBuff>(), 240);
                    }
                    else
                    {

                    }
                }
            }
        }
        public override bool CanHitPlayer(Player target)
        {
            return WorldSavingSystem.MasochistModeReal ? base.CanHitPlayer(target) : false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            target.AddBuff(BuffID.Bleeding, 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<AbomFangBuff>(), 240);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCsAndTiles.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
           // base.PreDraw(ref lightColor);
            Vector2 auraPos = Projectile.Center;

            float leeway = Projectile.width / 2 * Projectile.scale;
            leeway *= 0.75f;
            float radius = threshold - leeway;

            var target = Main.LocalPlayer;
             
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.WavyNoise;

            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;

            var maxOpacity = Projectile.Opacity;
            float scale = MathF.Sqrt(VisualScale);

            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.AbomRitualShader");
            borderShader.TrySetParameter("colorMult", 7.35f);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
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