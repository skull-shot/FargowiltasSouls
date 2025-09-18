using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class OpticFlamethrower : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_85";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 15)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X -= 30;
            hitbox.Y -= 30;
            hitbox.Width += 60;
            hitbox.Height += 60;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 60);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color = Projectile.GetAlpha(lightColor);
            Color color1 = Projectile.GetAlpha(new Color(155, 255, 155, 0));
            Color color2 = Projectile.GetAlpha(new Color(30, 180, 30, 50));
            Color color3 = Projectile.GetAlpha(new Color(0, 30, 0, 30));
            float lerp = 1 - (Projectile.timeLeft / 120f);
            if (lerp < 0.01f)
                color = Color.Lerp(Color.Transparent, color1, Utils.GetLerpValue(0f, 0.01f, lerp, true));
            else if (lerp < 0.05f)
                color = color1;
            else if (lerp < 0.35f)
                color = Color.Lerp(color1, color2, Utils.GetLerpValue(0.05f, 0.55f, lerp, true));
            else if (lerp < 0.75f)
                color = Color.Lerp(color2, color3, Utils.GetLerpValue(0.55f, 0.9f, lerp, true));
            else if (lerp < 1f)
                color = Color.Lerp(color3, Color.Transparent, Utils.GetLerpValue(0.9f, 1f, lerp, true));

            float scale = Projectile.scale;
            Vector2 value4 = Projectile.Center;
            if (Projectile.velocity != Vector2.Zero && !Projectile.velocity.HasNaNs())
                value4 -= Vector2.Normalize(Projectile.velocity) * 40f;
            float num165 = Projectile.rotation + Main.GlobalTimeWrappedHourly * 1.2f;

            Vector2 previousPosOffset = Projectile.oldPos[2] - Projectile.position;
            float prevPosRotation = Projectile.oldRot[2] + Main.GlobalTimeWrappedHourly * 1.2f;
            /*Main.EntitySpriteDraw(texture2D13, previousPosOffset + value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color, prevPosRotation, origin2, scale, effects, 0);*/
            Main.EntitySpriteDraw(texture2D13, previousPosOffset + Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color, prevPosRotation, origin2, scale, effects, 0);

            Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color, num165, origin2, scale, effects, 0);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color, num165, origin2, scale, effects, 0);
            return false;
        }
    }
}
