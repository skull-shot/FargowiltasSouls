using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class LifeRuneExplosion : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1;
            Projectile.timeLeft = 6000;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
            return Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;
        }
        private Color GetColor()
        {
            int i = (int)Projectile.ai[1];
            if (i % 3 == 0) //cyan
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.UltraBrightTorch);
                return Color.Cyan;
            }
            else if (i % 3 == 1) //yellow
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.YellowTorch);
                return Color.Goldenrod;
            }
            else //pink
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.PinkTorch);
                return Color.DeepPink;
            }
        }
        public int Timer = 0;
        public const int ExplosionSize = 200;
        public override void AI()
        {

            int i = (int)Projectile.ai[1];
            float explosionTime = Projectile.ai[2];

            if (Timer < explosionTime)
            {
                Projectile.Opacity = Timer / explosionTime;
            }
            if (Timer == explosionTime)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = ExplosionSize;
                Projectile.Center = Projectile.position;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                SoundEngine.PlaySound(LifeChallenger.RuneSound1 with { PitchRange = (-0.6f, -0.4f) }, Projectile.Center);

                for (int j = 0; j < 32; j++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                    Particle p = new SmallSparkle(
                        worldPosition: Projectile.Center + offset,
                        velocity: Vector2.Zero,
                        drawColor: GetColor(),
                        scale: 1f,
                        lifetime: Main.rand.Next(20, 80),
                        rotation: 0,
                        rotationSpeed: Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)
                        );
                    p.Spawn();
                }
                
            }

            if (Timer > explosionTime + 10)
                Projectile.Kill();

            Timer++;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.SmiteBuff>(), 60 * 6);
        }
        

        public float WidthFunction(float progress) => Projectile.scale * 2 * ExplosionSize;

        public Color ColorFunction(float progress)
        {
            return GetColor() * MathF.Sqrt(Projectile.Opacity) * 0.75f;
        }

        public static Matrix GetWorldViewProjectionMatrix()
        {
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(Main.graphics.GraphicsDevice.Viewport.Width / 2, Main.graphics.GraphicsDevice.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0, 1000);
            return view * projection;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            if (Timer >= Projectile.ai[2])
                return false;

            
            //draw glow ring
            Color RingColor = ColorFunction(1);
            Texture2D ringTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            float RingScale = Projectile.scale * (1f / 0.7f) * ExplosionSize / ringTexture.Height;
            Rectangle ringrect = new(0, 0, ringTexture.Width, ringTexture.Height);
            Vector2 ringorigin = ringrect.Size() / 2f;
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(ringrect), RingColor, Projectile.rotation, ringorigin, RingScale, SpriteEffects.None, 0);

            RingColor = Color.White * MathF.Sqrt(Projectile.Opacity) * 0.25f;
            ringTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRingHollow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            RingScale = Projectile.scale * 1f * ExplosionSize / ringTexture.Height;
            ringrect = new(0, 0, ringTexture.Width, ringTexture.Height);
            ringorigin = ringrect.Size() / 2f;
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(ringrect), RingColor, Projectile.rotation, ringorigin, RingScale, SpriteEffects.None, 0);
            return false;

            /*
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.Vertex_CircleTelegraph");

            FargoSoulsUtil.SetTexture1(ModContent.Request<Texture2D>("Terraria/Images/Extra_193").Value);
            Color color = GetColor();
            shader.TrySetParameter("mainColor", color);
            shader.TrySetParameter("uWorldViewProjection", GetWorldViewProjectionMatrix());
            shader.Apply();

            VertexStrip vertexStrip = new();
            List<Vector2> positions = [];
            List<float> rotations = [];
            float initialRotation = Projectile.rotation - MathHelper.Pi * 0.5f;
            for (float i = 0; i < 1; i += 0.005f)
            {
                float rotation = initialRotation + MathHelper.TwoPi * i;
                positions.Add(rotation.ToRotationVector2() * Projectile.scale * 2 * ExplosionSize + Projectile.Center);
                rotations.Add(rotation + MathHelper.PiOver2);
            }
            vertexStrip.PrepareStrip(positions.ToArray(), rotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
            vertexStrip.DrawTrail();
            Main.spriteBatch.EnterDefaultRegion();
            return false;
            */
        }
    }
}
