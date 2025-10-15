using FargowiltasSouls.Assets.Textures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Hell
{
    public class ImpFireOrb : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.Fireball);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = DemonFireball.TrailLength;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public int baseWidth = 0;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Fireball);
            Projectile.aiStyle = -1;
            AIType = 0;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 8;
            Projectile.Opacity = 1f;
            Projectile.alpha = 0;
            baseWidth = Projectile.width;
        }
        public ref float TargetID => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float OwnerID => ref Projectile.ai[2];
        public override void AI()
        {
            Projectile.rotation += MathHelper.TwoPi / 7f;
            Timer++;
            float chargeTime = 60 * 5;
            float homeTime = Main.hardMode ? 60 : 120;
            float maxScale = 2f;

            if (Timer <= chargeTime)
            {
                if (!Main.npc[(int)OwnerID].TypeAlive(NPCID.FireImp))
                {
                    Projectile.Kill();
                    return;
                }
            }

            if (Timer < chargeTime)
            {

                Projectile.position = Projectile.Center;
                Projectile.scale = 1f + (maxScale - 1) * Timer / chargeTime;
                Projectile.width = Projectile.height = (int)(baseWidth * Projectile.scale);
                Projectile.Center = Projectile.position;
                Projectile.velocity = Vector2.Zero;
            }
            else if (Timer == chargeTime)
            {
                Player player = Main.player[(int)TargetID];
                Vector2 dir = Main.npc[(int)OwnerID].direction * Vector2.UnitX;
                if (player != null && player.Alive())
                {
                    dir = Projectile.DirectionTo(player.Center);
                }
                float spd = MathHelper.Lerp(7f, 15f, Projectile.scale / maxScale);
                if (!Main.hardMode)
                    spd /= 2;
                Projectile.velocity = dir * spd;
                SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);
            }
            else
            {
                if (Timer < chargeTime + homeTime)
                {
                    Player player = Main.player[(int)TargetID];
                    if (player != null && player.Alive())
                    {
                        Projectile.velocity = Projectile.velocity.RotateTowards(Projectile.DirectionTo(player.Center).ToRotation(), 0.01f);
                    }
                }
                
            }
            for (int i = 0; i < 4; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= MathF.Pow(Projectile.scale, 0.5f);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float radius = Projectile.width / 2;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.WavyNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;

            Vector2 auraPos = Projectile.Center;
            var maxOpacity = Projectile.Opacity * 0.7f;

            ManagedShader borderShader = ShaderManager.GetShader("FargowiltasSouls.HellFireballShader");
            borderShader.TrySetParameter("colorMult", 7.35f);
            borderShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            borderShader.TrySetParameter("radius", radius);
            borderShader.TrySetParameter("screenPosition", Main.screenPosition);
            borderShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());


            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);

            Vector2[] anchorPoints = new Vector2[ProjectileID.Sets.TrailCacheLength[Type]];
            float[] opacities = new float[ProjectileID.Sets.TrailCacheLength[Type]];

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                var oldOpacity = maxOpacity * (ProjectileID.Sets.TrailCacheLength[Type] - (float)i) / (float)ProjectileID.Sets.TrailCacheLength[Type];
                Vector2 oldCenter = Projectile.oldPos[i] + Projectile.Size / 2;
                anchorPoints[i] = oldCenter;
                opacities[i] = oldOpacity;
            }

            borderShader.TrySetParameter("anchorPoints", anchorPoints);
            borderShader.TrySetParameter("opacities", opacities);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, borderShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}