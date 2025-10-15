using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantEye : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantEye_April" :
            "Terraria/Images/Projectile_452";

        public virtual int TrailAdditive => 0;

        protected bool DieOutsideArena;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantasmal Eye");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.alpha = 0;
            CooldownSlot = ImmunityCooldownID.Bosses;

            //dont let others inherit this behaviour
            DieOutsideArena = Projectile.type == ModContent.ProjectileType<MutantEye>();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        private int ritualID = -1;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

            if (Projectile.localAI[0] < ProjectileID.Sets.TrailCacheLength[Projectile.type])
            {
                Projectile.localAI[0] += 0.1f;
            }
            else
                Projectile.localAI[0] = ProjectileID.Sets.TrailCacheLength[Projectile.type];

            Projectile.localAI[1] += 0.25f;

            if (DieOutsideArena)
            {
                if (ritualID == -1) //identify the ritual CLIENT SIDE
                {
                    ritualID = -2; //if cant find it, give up and dont try every tick

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<MutantRitual>())
                        {
                            ritualID = i;
                            break;
                        }
                    }
                }

                Projectile ritual = FargoSoulsUtil.ProjectileExists(ritualID, ModContent.ProjectileType<MutantRitual>());
                if (ritual != null && Projectile.Distance(ritual.Center) > 1200f) //despawn faster
                    Projectile.timeLeft = 0;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (target.FargoSouls().BetsyDashing)
                return;
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            Projectile.timeLeft = 0;
        }

        public override void OnKill(int timeleft)
        {
            SoundEngine.PlaySound(SoundID.Zombie103, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 144;
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;
            for (int index = 0; index < 2; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
                Main.dust[index3].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.7f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(FargoSoulsUtil.AprilFools ? Color.Yellow : Color.Cyan, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantEye_Glow").Value;
            int rect1 = glow.Height / Main.projFrames[Projectile.type];
            int rect2 = rect1 * Projectile.frame;
            Rectangle glowrectangle = new(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = FargoSoulsUtil.AprilFools ? new Color(255, 0, 0, TrailAdditive) : new Color(31, 187, 192, TrailAdditive);
            Vector2 drawCenter = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14;

            if (SoulConfig.Instance.PerformanceMode)
            {
                glowcolor *= 0.5f;
                Main.EntitySpriteDraw(glow, drawCenter - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, Projectile.scale, SpriteEffects.None, 0);
                return false;
            }

            glowcolor = Color.Lerp(
                glowcolor,
                Color.Transparent,
                0.74f);

            for (int i = 0; i < 3; i++) //create multiple transparent trail textures ahead of the projectile
            {
                Vector2 drawCenter2 = drawCenter + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 8).RotatedBy(MathHelper.Pi / 5 - i * MathHelper.Pi / 5); //use a normalized version of the projectile's velocity to offset it at different angles
                drawCenter2 -= Projectile.velocity.SafeNormalize(Vector2.UnitX) * 8; //then move it backwards
                float scale = Projectile.scale;
                scale += (float)Math.Sin(Projectile.localAI[1]) / 10;
                Main.EntitySpriteDraw(glow, drawCenter2 - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale, SpriteEffects.None, 0);
            }
            
            
            for (float i = Projectile.localAI[0] - 1; i > 0; i -= Projectile.localAI[0] / ProjectileID.Sets.TrailCacheLength[Projectile.type]) //trail grows in length as projectile travels
            {

                float lerpamount = 0.2f;
                if (i > 5 && i < 10)
                    lerpamount = 0.4f;
                if (i >= 10)
                    lerpamount = 0.6f;

                Color color27 = Color.Lerp(glowcolor, Color.Transparent, 0.1f + lerpamount);

                color27 *= (int)((Projectile.localAI[0] - i) / Projectile.localAI[0]) ^ 2;
                float scale = Projectile.scale * (float)(Projectile.localAI[0] - i) / Projectile.localAI[0];
                scale += (float)Math.Sin(Projectile.localAI[1]) / 10;
                Vector2 value4 = Projectile.oldPos[(int)i] - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14;
                Main.EntitySpriteDraw(glow, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 0.8f, SpriteEffects.None, 0);
            }
            
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

        }
    }
}