using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.OOA
{
    public class WyvernClone : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles", "Empty");

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
        }

        public ref float type => ref Projectile.ai[0];

        public ref float target => ref Projectile.ai[1];

        public ref float timer => ref Projectile.ai[2];

        public override void AI()
        {
            Entity entity;
            if (target > 255)
            {
                entity = EModeDD2Event.GetEterniaCrystal();
                if (entity == null)
                    Projectile.Kill();
            }
            else
            {
                entity = Main.player[(int)target];
                if (!entity.active)
                    Projectile.Kill();
            }
            Vector2 targetPos = entity.Center;

            float initVel = 7;
            float accel = 0.3f;
            if (timer == 0)
            {
                Projectile.rotation = (Projectile.Center - targetPos).ToRotation();
                Projectile.velocity = initVel * Vector2.UnitX.RotatedBy(Projectile.rotation);
                Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Smoke);
            }
            timer++;

            Projectile.velocity -= accel * Vector2.UnitX.RotatedBy(Projectile.rotation);
            if (timer > (initVel/accel) + 3)
            {
                Vector2 pos = Projectile.Center + 20 * Vector2.UnitX.RotatedBy(Projectile.rotation) + Main.rand.Next(-4, 5) * Vector2.UnitY.RotatedBy(Projectile.rotation);
                new SmallSparkle(pos, Vector2.Zero, Color.Purple, 1f, 7).Spawn();
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);
            for (int i = 0; i < 20; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = TextureAssets.Npc[(int)type].Value;
            Projectile.frame = 4;
            int frames = Main.projFrames[Type];
            Rectangle frame = new Rectangle(0, Projectile.frame * (t.Height / frames), t.Width, t.Height / frames);
            SpriteEffects flip = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Color color = lightColor * 0.5f;
            Main.EntitySpriteDraw(t, Projectile.position - Main.screenPosition + frame.Size()/4, frame, color, Projectile.rotation, frame.Size()/2, Projectile.scale, flip);
            return false;
        }
    }
}
