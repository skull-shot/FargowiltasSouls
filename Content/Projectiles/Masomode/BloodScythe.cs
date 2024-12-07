using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BloodScythe : ModProjectile, IPixelatedPrimitiveRenderer
    {
        int randomize;
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/BloodScytheVanilla1";

        public bool recolor => SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode && Projectile.ai[2] != 1;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            randomize += Main.rand.Next(1, 4);
            base.OnSpawn(source);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DemonSickle);
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }
            Projectile.rotation += 0.8f;
            if (++Projectile.localAI[1] > 30 && Projectile.localAI[1] < 120)
                Projectile.velocity *= 1.03f;

            //for (int i = 0; i < 3; i++)
            //{
            Vector2 offset = new Vector2(0, -20).RotatedBy(Projectile.rotation);
            offset = offset.RotatedByRandom(MathHelper.Pi / 6);
            int d = Dust.NewDust(Projectile.Center, 0, 0, recolor ? DustID.Vortex : DustID.BloodWater, 0f, 0f, 150);
            Main.dust[d].position += offset;
            float velrando = Main.rand.Next(20, 31) / 10;
            Main.dust[d].velocity = Projectile.velocity / velrando;
            Main.dust[d].noGravity = true;
            Main.dust[d].scale = 1.2f;

            if (!Projectile.active)
            {
                Main.dust[d].scale = 0f;
            }
            //}

            if (Projectile.timeLeft < 180)
                Projectile.tileCollide = true;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(recolor ? Color.Teal : Color.DarkRed , Color.Transparent, completionRatio) * 0.7f;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 25);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {   
            Texture2D texture = recolor ? ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Masomode/BloodScythe" + randomize).Value : ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Masomode/BloodScytheVanilla" + randomize).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing").Value;

            Vector2 glowDrawPosition = Projectile.Center + Projectile.velocity / 10f;
            glowDrawPosition += Main.rand.NextVector2Circular(5, 5);

            Main.EntitySpriteDraw(glowTexture, glowDrawPosition - Main.screenPosition, null, recolor ? Color.Teal : Color.DarkRed, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None, 0);
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture: texture);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            /*Particle p = new SparkParticle(Projectile.Center, Main.rand.NextVector2Circular(5, 5), recolor? Color.White : Color.Red, 1f, 25, false, null);
            p.Spawn();
            p.Spawn();
            p.Spawn();*/
            for (int i = 0; i < 4; i++)
            {
            int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.SnowSpray, 0f, 0f, 150);
            Main.dust[d].velocity = Main.rand.NextVector2Circular(5,5);
            Main.dust[d].noGravity = true;
            Main.dust[d].scale = 2f;
            Main.dust[d].color = Color.White;
            }
            base.OnKill(timeLeft);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 300);
                target.AddBuff(BuffID.Bleeding, 600);
                target.AddBuff(BuffID.Obstructed, 15);
            }

            target.AddBuff(ModContent.BuffType<BerserkedBuff>(), 300);
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 120);
        }
    }
}
