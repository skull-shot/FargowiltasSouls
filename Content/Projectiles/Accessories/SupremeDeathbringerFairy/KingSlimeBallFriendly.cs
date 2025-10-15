using System;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses.KingSlime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.SupremeDeathbringerFairy
{
    public class KingSlimeBallFriendly : KingSlimeBall
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/MutantBoss/MutantSlimeBall_2";
        public int BounceCount = 0;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = false;
            Projectile.DamageType = DamageClass.Generic;
        }

        public override void AI()
        {
            base.AI();
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 10 && Projectile.velocity.Y < 0)
                Projectile.velocity.Y -= 0.3f; //ignore gravity for first few frames
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[2] == 1)
                return Color.Lerp(Color.DarkRed, Color.Transparent, 0.5f);
            return base.GetAlpha(lightColor);
        }

        public override bool? CanCutTiles() => false;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (BounceCount < 1 || oldVelocity.Y > 10)
            {
                if (Projectile.ai[1] > 10) //dont waste bounce if it bounces on spawning
                    BounceCount += 1;
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X / 2;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y / 2;
                SoundEngine.PlaySound(SoundID.Item154 with {Volume = 0.5f}, Projectile.position);
                return false;
            }
            else return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}