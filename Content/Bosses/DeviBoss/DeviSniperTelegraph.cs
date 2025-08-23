using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Common.Graphics.Particles;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
	public class DeviSniperTelegraph : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureItem(ItemID.SniperRifle);
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = false;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.scale = 0;
            Projectile.Opacity = 1;
        }
        ref float NPCID => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1]; // starts at -telegraphtime
        ref float TelegraphStart => ref Projectile.ai[2];
        public override void AI()
        {
            float width = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Width;
            if (TelegraphStart == 0)
                TelegraphStart = Timer;
            int npcID = (int)NPCID;
            if (!npcID.IsWithinBounds(Main.maxNPCs))
            {
                Projectile.Kill();
                return;
            }
            NPC npc = Main.npc[npcID];
            if (!npc.TypeAlive<DeviBoss>() || !npc.HasPlayerTarget)
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[npc.target];
            if (player == null || !player.Alive())
            {
                Projectile.Kill();
                return;
            }
            Timer++;
            if (Timer <= 0) // telegraphing
            {
                float progress = MathF.Abs(Timer - TelegraphStart) / MathF.Abs(TelegraphStart);
                progress *= 1.25f;
                progress = Math.Clamp(progress, 0, 1);
                float endRot = npc.DirectionTo(player.Center).ToRotation();
                float startRot = MathHelper.Lerp(-endRot, Vector2.UnitY.ToRotation(), 0.5f);
                Projectile.rotation = MathHelper.SmoothStep(startRot, endRot, progress);
                Projectile.scale = MathHelper.SmoothStep(0, 2, progress);

            }
            else
            {
                float fadeTime = 15;
                float progress = Timer / fadeTime;
                float startRot = npc.DirectionTo(player.Center).ToRotation();
                float endRot = MathHelper.Lerp(-startRot, Vector2.UnitY.ToRotation(), 0.5f);
                Projectile.rotation = MathHelper.SmoothStep(startRot, endRot, progress);
                Projectile.scale = MathHelper.SmoothStep(2, 0, progress);
                if (progress >= 1)
                {
                    Projectile.Kill();
                    return;
                }
            }
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Projectile.Center = npc.Center + dir * width / 2;
            Projectile.spriteDirection = dir.X.NonZeroSign();

            if (Timer == 0)
            {
                Vector2 sparkpos = Projectile.Center + dir * width;
                for (int i = 0; i < 20; i++)
                {
                    Vector2 vel = dir.RotateRandom(MathHelper.PiOver2 * 0.3f) * Main.rand.NextFloat(8f, 12f);
                    Dust.NewDust(sparkpos - Vector2.One * 5, 10, 10, DustID.Torch, vel.X, vel.Y);
                }
                for (int i = 0; i < 5; i++)
                {
                    Vector2 vel = dir.RotateRandom(MathHelper.PiOver2 * 0.35f) * Main.rand.NextFloat(8f, 20f);
                    SparkParticle p = new(sparkpos - Vector2.One * 5 + Main.rand.NextVector2Square(10, 10), vel, Color.OrangeRed, 0.5f, 15);
                    p.Spawn();
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}