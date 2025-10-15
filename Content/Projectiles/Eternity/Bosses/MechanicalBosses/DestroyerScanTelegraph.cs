﻿using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.MechanicalBosses
{
    public class DestroyerScanTelegraph : ModProjectile
    {
        public ref float Timer => ref Projectile.ai[0];

        public ref float ArcAngle => ref Projectile.ai[1];

        public ref float Width => ref Projectile.ai[2];
        public ref float maxTime => ref Projectile.localAI[2];

        // Can be anything.
        public override string Texture => "Terraria/Images/Extra_" + ExtrasID.MartianProbeScanWave;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 60 * 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.width = 1;
            Projectile.height = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(npc);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc = reader.Read7BitEncodedInt();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[2] = reader.ReadSingle();
        }
        int npc;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC parentNpc && parentNpc.type == NPCID.TheDestroyer)
            {
                npc = parentNpc.whoAmI;
                Projectile.velocity = parentNpc.velocity;
            }
        }

        public override void AI()
        {
            if (maxTime == 0)
                maxTime = Projectile.timeLeft;
            NPC parent = FargoSoulsUtil.NPCExists(npc);
            if (parent != null)
            {
                Projectile.Center = parent.Center;
                Projectile.velocity = parent.velocity;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            Timer++;
        }

        public override bool ShouldUpdatePosition() => false;


        public override bool PreDraw(ref Color lightColor)
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            Color color = Color.DeepSkyBlue;
            if (!recolor)
                color = Color.Red;

            Vector2 pos = Projectile.Center;
            float timeLerp = MathF.Pow(Projectile.timeLeft / maxTime, 0.5f);
            float radius = 500 + 500 * timeLerp;
            float arcAngle = Projectile.rotation;
            float arcWidth = ArcAngle * timeLerp;

            var blackTile = TextureAssets.MagicPixel;
            var noise = FargoAssets.Techno1Noise;
            if (!blackTile.IsLoaded || !noise.IsLoaded)
            {
                return false;
            }

            var maxOpacity = 0.25f;
            float fade = 0.5f;
            if (timeLerp > 1 - fade)
            {
                float fadeinLerp = (timeLerp - (1 - fade)) / fade;
                maxOpacity *= 1 - fadeinLerp;
            }

            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.DestroyerScanTelegraph");
            shader.TrySetParameter("colorMult", 7.35f);
            shader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            shader.TrySetParameter("radius", radius);
            shader.TrySetParameter("arcAngle", arcAngle.ToRotationVector2());
            shader.TrySetParameter("arcWidth", arcWidth);
            shader.TrySetParameter("anchorPoint", pos);
            shader.TrySetParameter("screenPosition", Main.screenPosition);
            shader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            shader.TrySetParameter("maxOpacity", maxOpacity);
            shader.TrySetParameter("color", color.ToVector4());

            Main.spriteBatch.GraphicsDevice.Textures[1] = noise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }
}
