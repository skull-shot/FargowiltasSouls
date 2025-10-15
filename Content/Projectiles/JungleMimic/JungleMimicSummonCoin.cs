﻿using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.JungleMimic
{
    public class JungleMimicSummonCoin : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/JungleMimic", Name);
        public int coinType = -1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }

        public override bool PreAI()
        {
            if (coinType == -1)
            {
                coinType = (int)Projectile.ai[0];
                switch (coinType)
                {
                    case 0: AIType = ProjectileID.CopperCoin; Projectile.frame = 0; break;
                    case 1: AIType = ProjectileID.SilverCoin; Projectile.frame = 1; break;
                    case 2: AIType = ProjectileID.GoldCoin; Projectile.frame = 2; break;
                    default: AIType = ProjectileID.PlatinumCoin; Projectile.frame = 3; break;
                }
            }
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            var dusttype = coinType switch
            {
                0 => 9,
                1 => 11,
                2 => 19,
                _ => 11,
            };
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dusttype, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity -= Projectile.velocity * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Midas, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}