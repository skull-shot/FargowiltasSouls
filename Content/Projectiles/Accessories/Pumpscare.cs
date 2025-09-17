using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories
{
    public class Pumpscare : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureNPC(NPCID.Pumpking);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.Pumpking];
        }

        int maxtime = 60;

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = maxtime;
            Projectile.Opacity = 0f;
        }

        ref float Timer => ref Projectile.localAI[0];

        public override void AI()
        {
            //if (Timer == 0)
                //SoundEngine.PlaySound(FargosSoundRegistry.AbomSpawnSound, Projectile.Center);
            Timer += 2;
            Projectile.scale = 0f;
            for (int i = 0; i < Timer; i++)
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 3f, 0.05f);
            Projectile.Opacity = Math.Min(1f, 1.25f * (float)Math.Sin(MathHelper.Pi / maxtime * Timer));
            Projectile.Opacity *= 0.5f;
            Projectile.frame = 4;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            origin2.Y += 16;

            SpriteEffects effects = SpriteEffects.None;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * Projectile.Opacity;
        }
    }
}