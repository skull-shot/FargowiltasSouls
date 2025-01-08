using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace FargowiltasSouls.Content.Patreon.Tiger
{
    public class CirnoIceCube : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60; //
            Projectile.penetrate = 1;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.tileCollide = true;
        }

        public override void OnKill(int timeLeft)
        {
            //crystal bullet tm
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.position);

            for (int num596 = 0; num596 < 5; num596++)
            {
                int num597 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 68);
                Main.dust[num597].noGravity = true;
                Dust dust2 = Main.dust[num597];
                dust2.velocity *= 1.5f;
                dust2 = Main.dust[num597];
                dust2.scale *= 0.9f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int num598 = 0; num598 < 2; num598++)
                {
                    float num599 = (0f - Projectile.velocity.X) * (float)Main.rand.Next(40, 70) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.4f;
                    float num600 = (0f - Projectile.velocity.Y) * (float)Main.rand.Next(40, 70) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + num599, Projectile.position.Y + num600, num599, num600, ModContent.ProjectileType<CirnoIceChunk>(), (int)((double)Projectile.damage * 0.5), 0f, Projectile.owner);
                }
            }

            base.OnKill(timeLeft);
        }
    }
}
