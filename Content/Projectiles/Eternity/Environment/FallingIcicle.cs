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
            Projectile.scale = 1f;
            Projectile.netImportant = true;
        }

        private bool firstTick = true;
        private readonly bool CanBeDangerSensed = !Main.gamePaused && Main.instance.IsActive && Main.netMode != NetmodeID.Server && Main.LocalPlayer.dangerSense;

        public override void AI()
        {
            if (firstTick)
            {
                firstTick = false;
                Projectile.scale = 0.1f;
            }

            if (CanBeDangerSensed && Main.rand.NextBool(30))
            {
                // Dangersense
                int danger = Dust.NewDust(Projectile.position, 16, 32, DustID.RedTorch, 0f, 0f, 100, default, 0.3f);
                Main.dust[danger].fadeIn = 1f;
                Main.dust[danger].velocity *= 0.1f;
                Main.dust[danger].noLight = true;
                Main.dust[danger].noGravity = true;
            }

            Projectile.timeLeft = 2;
            Entity? target = null;
            if (Main.netMode == NetmodeID.SinglePlayer)
                target = Main.LocalPlayer;
            else
            {
                foreach (Player p in Main.player.Where(p => p.active && !p.dead))
                {
                    if (target == null)
                    {
                        target = p;
                    }
                    else if (target != null && target.Center.Distance(Projectile.Center) > p.Center.Distance(Projectile.Center))
                    {
                        target = p;
                    }
                }
            }

            if (target == null || Projectile.Center.Distance(target.Center) > 500)
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
                Projectile.netUpdate = true;
                return;
            }

            foreach (NPC nPC in Main.ActiveNPCs)
            {
                if (target != null && target.Center.Distance(Projectile.Center) > nPC.Center.Distance(Projectile.Center))
                {
                    target = nPC;
                }
            }

            //falling
            if (Projectile.ai[0] == 1)
            {
                Projectile.velocity.Y = Projectile.velocity.Y * 1.1f;
                Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, 4, 12);
                Projectile.netUpdate = true;
                return;
            }

            //waits for target to walk below
            if (target?.Center.Y > Projectile.Center.Y && (Math.Abs(target.Center.X - Projectile.Center.X) < 10) && Projectile.Center.Distance(target.Center) < 500)
            {
                //start falling
                Projectile.velocity.Y = 4f;
                Projectile.ai[0] = 1;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Frostburn, 120);
            Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 120);
            Projectile.Kill();
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
