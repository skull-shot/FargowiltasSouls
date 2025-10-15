using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.SkyAndRain;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.Projectiles.Accessories.BionomicCluster
{
    public class FlightBall : LightBall
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Enemies/Vanilla/SkyAndRain", "LightBall");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 0;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }

            for (int i = 0; i < 2; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.X *= 0.6f;
                Main.dust[d].velocity.Y *= 0.6f;

                if (Projectile.velocity.X != 0)
                    Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
                Projectile.rotation += 0.3f * Projectile.direction;
            }

            bool slowdown = true;
            if (Projectile.localAI[0] > 90f)
            {
                Player p = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
                if (p.active && !p.dead && !p.ghost)
                {
                    int distance = p.HasEffect<IronEquippedEffect>() ? 80 + (IronEquippedEffect.GrabRangeBonus(p) / 2) : 80;
                    if (p.Distance(Projectile.Center) < distance)
                    {
                        slowdown = false;
                        Projectile.velocity = Projectile.SafeDirectionTo(p.Center) * 9f;
                        Projectile.timeLeft++;
                        if (Projectile.Colliding(Projectile.Hitbox, p.Hitbox))
                        {
                            p.wingTime = p.wingTimeMax;
                            p.rocketTime = p.rocketTimeMax;
                            p.RefreshExtraJumps();
                            FargoGlobalItem.OnRetrievePickup(p);
                            Projectile.Kill();
                            return;
                        }
                    }
                }
            }
            if (slowdown)
                Projectile.velocity *= 0.95f;
        }
    }
}