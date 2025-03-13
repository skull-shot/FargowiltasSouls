using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
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

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class EmpressRitual : BaseArena
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        private const float realRotation = MathHelper.Pi / 180f;
        public float VisualScale = 0f;
        public int Timer = 0;

        public EmpressRitual() : base(realRotation, 600f, NPCID.HallowBoss, DustID.HallowedTorch, visualCount: 64) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hide = true;
        }
        protected override void Movement(NPC npc)
        {
            Projectile.velocity = Vector2.Zero;
            if (npc.TypeAlive(NPCID.HallowBoss))
            {
                Projectile.velocity = npc.GetGlobalNPC<EmpressofLight>().targetPos - Projectile.Center;
            }
            else
            {
                Projectile.Kill();
            }
            
        }
        public override void AI()
        {
            base.AI();
            float maxTime = Projectile.ai[2];
            float endTime = 30f;
            Projectile.scale = 1f;
            VisualScale = 1f;
            //NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], npcType);
            /*
            if (npc.TypeAlive(NPCID.HallowBoss))
            {
                if (npc.GetGlobalNPC<EmpressofLight>().Ritual)
                {
                    Timer = 0;
                }
                else
                {
                    if (Timer < maxTime - endTime)
                        Timer = (int)(maxTime - endTime);
                }
            }
            else
            {
                Projectile.Kill();
            }
            */
            Projectile.rotation += 1f;

            /*
            Player player = Main.LocalPlayer;
            if (player.active && !player.dead && !player.ghost)
            {
                float distance = player.Distance(Projectile.Center);
                if (distance > threshold && distance < threshold * 4f)
                {
                    
                }
            }
            */
            if (Projectile.Opacity >= 1f)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.TypeAlive(ProjectileID.FairyQueenLance) && p.Distance(Projectile.Center) > threshold + 5)
                    {
                        p.Kill();
                    }
                }
            }
            Timer++;
            if (Timer >= maxTime - endTime)
            {
                Projectile.Opacity = 1f - LumUtils.InverseLerp(maxTime - endTime, maxTime, Timer);
                if (Timer >= maxTime)
                {
                    Projectile.Kill();
                }
            }
        }
        public override bool CanHitPlayer(Player target) => base.CanHitPlayer(target);
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
            var diagonalNoise = FargosTextureRegistry.WavyNoise;

            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;

            var maxOpacity = Projectile.Opacity;
            float scale = MathF.Sqrt(VisualScale);

            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.EmpressRitualShader");
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