using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	public class RectangleParticle : Particle
	{
        public override string AtlasTextureName => "FargowiltasSouls.RectangleParticle";
        public Color BloomColor;
        public readonly bool UseBloom;
        public readonly bool UseGravity;

		public override BlendState BlendState => BlendState.Additive;

        public RectangleParticle(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, bool Gravity = false, bool useBloom = true, Color? bloomColor = null)
		{
			Position = worldPosition;
			Velocity = velocity;
			DrawColor = drawColor;
			Scale = new(scale);
			Lifetime = lifetime;
			UseBloom = useBloom;
            UseGravity = Gravity;
			bloomColor ??= Color.White;
			BloomColor = bloomColor.Value;
		}

		public override void Update()
		{
            // Shrink, fade, and slow over time.
            if (UseGravity)
            {
                Velocity.Y += 0.33f;
            }
            
			Velocity *= 0.95f;
			Scale *= 0.95f;
			Opacity = FargoSoulsUtil.SineInOut(1f - LifetimeRatio);
			Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Vector2 scale = new Vector2(0.5f, 1.6f) * Scale;
			spriteBatch.Draw(Texture, Position - Main.screenPosition, null, DrawColor, Rotation, null, scale, 0);
			spriteBatch.Draw(Texture, Position - Main.screenPosition, null, DrawColor, Rotation, null, scale * new Vector2(0.45f, 1f), 0);

			if (UseBloom)
				spriteBatch.Draw(Texture, Position - Main.screenPosition, null, BloomColor * 0.5f, Rotation, null, scale, 0);
		}
	}
}
