using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Bosses.Champions.Life
{
    public class CactusMine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactus Mine");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = WorldSavingSystem.MasochistModeReal ? 50 : 90;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            float speed = Projectile.velocity.Length();
            speed += Projectile.ai[1];
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * speed;

            Projectile.rotation += Projectile.velocity.Length() * 0.015f * System.Math.Sign(Projectile.velocity.X);
        }

        public override void OnKill(int timeLeft)
        {
            FargoSoulsUtil.XWay(16, Terraria.Entity.InheritSource(Projectile), Projectile.Center, ProjectileID.PineNeedleHostile, 5, Projectile.damage, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f;

                DrawData data = new(texture2D13, Projectile.Center + afterimageOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.DeepPink).UseSecondaryColor(Color.Pink);
                GameShaders.Misc["LCWingShader"].Apply(data);
                data.Draw(Main.spriteBatch);
            }
            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}