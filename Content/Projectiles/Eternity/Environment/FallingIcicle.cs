using Fargowiltas.Content.Projectiles;
using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Environment
{
    public class FallingIcicle : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Environment", "FallingIcicle");
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.1f;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;
            Player target = Main.player[Projectile.owner];

            if (Vector2.Distance(Projectile.Center, target.Center) > 500)
            {
                Projectile.Kill();
            }

            //size increases over time until max size
            if (Projectile.scale < 1f)
            {
                Projectile.scale += 0.05f;

                if (Projectile.scale == 1f)
                {
                    //do a twinkle or sound or something tm
                    FargoSoulsUtil.DustRing(Projectile.Center, 5, DustID.SnowflakeIce, 1);
                }

                return;
            }

            //falling
            if (Projectile.ai[0] == 1)
            {
                Projectile.velocity.Y = Projectile.velocity.Y * 1.1f;
                Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, 4, 12);
                return;
            }

            //waits for player to walk below
            if (target.Center.Y > Projectile.Center.Y && (Math.Abs(target.Center.X - Projectile.Center.X) < 10) && Vector2.Distance(Projectile.Center, target.Center) < 300)
            {
                //start falling
                Projectile.velocity.Y = 4f;
                Projectile.ai[0] = 1;
            }

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

                FargoSoulsUtil.DustRing(Projectile.Center, 5, DustID.SnowflakeIce, 1);
            }
            
        }

    }
}
