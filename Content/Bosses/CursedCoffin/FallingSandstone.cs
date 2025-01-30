using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
    public class FallingSandstone : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;

            Projectile.Opacity = 0;

            Projectile.hide = true;
        }
        public ref float TimeUntilFall => ref Projectile.ai[0];
        public ref float RotationSpeed => ref Projectile.ai[1];
        public override bool? CanDamage() => TimeUntilFall > 0 ? false : base.CanDamage();
        public override void AI()
        {
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1, 0.05f);
            if (TimeUntilFall > 0)
            {
                if (TimeUntilFall % 4 == 0)
                    Projectile.rotation = Main.rand.NextFloat(-MathF.PI / 3, MathF.PI / 3);
                
                if (TimeUntilFall % 7 == 0)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SandstormInABottle, 0, Main.rand.NextFloat(2, 4), Scale: Main.rand.NextFloat(0.7f, 1.5f));
                }
            }
            else if (TimeUntilFall == 0)
            {
                RotationSpeed = Main.rand.NextFloat(-MathF.PI / 14, MathF.PI / 14);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            }
            else
            {
                Projectile.velocity.Y += 0.3f;
            }
            if (TimeUntilFall < -20)
            {
                Projectile.tileCollide = true;
            }
            TimeUntilFall--;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SandstormInABottle, Main.rand.NextFloat(2, 4), Main.rand.NextFloat(2, 4), Scale: Main.rand.NextFloat(0.7f, 1.5f));
            }
            return true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCsAndTiles.Add(index);
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}
