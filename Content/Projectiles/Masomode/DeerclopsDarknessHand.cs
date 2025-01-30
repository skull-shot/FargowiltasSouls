using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class DeerclopsDarknessHand : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_965";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.InsanityShadowHostile];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = ProjectileID.Sets.TrailCacheLength[ProjectileID.InsanityShadowHostile];
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.InsanityShadowHostile);
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 900;
            Projectile.Opacity = 0f;
            Projectile.light = 0f;
        }

        bool spawned;
        bool fading = false;
        public ref float Timer => ref Projectile.ai[0];
        public ref float PlayerID => ref Projectile.ai[1];

        int telegraphTime = 120;
        int aimTime = 30;
        int chargeTime = 120;


        public override void AI()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, Projectile.Center);
            }

            if (fading && Timer > telegraphTime)
            {
                Projectile.Opacity -= 0.05f;
                if (Projectile.Opacity < 0.1f)
                {
                    Projectile.Kill();
                    return;
                }
            }

            if (++Timer < telegraphTime)
            {
                if (Projectile.Opacity < 1)
                    Projectile.Opacity += 0.05f;

                float progress = 1 - (Timer / (telegraphTime + aimTime));
                Projectile.rotation += MathHelper.TwoPi * progress / 15f;
                //Projectile.velocity += Projectile.rotation.ToRotationVector2() * MathF.Pow(progress, 0.5f) * 3f;
            }
            else if (Timer <= telegraphTime + aimTime)
            {
                if (!((int)PlayerID).IsWithinBounds(Main.maxPlayers))
                {
                    Projectile.Kill();
                    return;
                }
                Player player = Main.player[(int)PlayerID];

                if (!player.Alive())
                    return;

                Color light = Lighting.GetColor(player.Center.ToTileCoordinates());
                float lightLevel = light.R + light.G + light.B;
                if (lightLevel > 500)
                    fading = true;

                Projectile.rotation = Projectile.rotation.ToRotationVector2().RotateTowards(Projectile.DirectionTo(player.Center).ToRotation(), 0.1f).ToRotation();
                Projectile.velocity *= 0.96f;
                if (Timer == 90)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * 0.2f;
                }
                    
            }
            else if (Timer <= telegraphTime + aimTime + chargeTime)
            {
                if (!((int)PlayerID).IsWithinBounds(Main.maxPlayers))
                {
                    Projectile.Kill();
                    return;
                }
                Player player = Main.player[(int)PlayerID];
                if (!player.Alive())
                    return;
                Projectile.velocity += Projectile.DirectionTo(player.Center) * 0.1f;
                Projectile.velocity = Projectile.velocity.ClampLength(0f, 4f);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.velocity *= 0.96f;
                if (Projectile.velocity.Length() < 1f)
                    Projectile.Opacity -= 0.05f;
                if (Projectile.Opacity < 0.1f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool? CanDamage()
        {
            if (Timer < telegraphTime + aimTime)
                return false;
            return base.CanDamage();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            //target.AddBuff(BuffID.Frostburn, 90);
            //if (WorldSavingSystem.MasochistModeReal)
            //    target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 900);
            //target.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 1200);
        }

        //public override Color? GetAlpha(Color lightColor)
        //{
        //    return Color.White * Projectile.Opacity;
        //}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Color.Black * Projectile.Opacity;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.LightBlue * Projectile.Opacity * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale * 1.1f, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}