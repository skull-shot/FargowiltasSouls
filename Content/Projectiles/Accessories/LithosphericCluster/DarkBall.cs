using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.SkyAndRain;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Buffs
{
    public class DarkBall : LightBall, IPixelatedPrimitiveRenderer
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/LithosphericCluster", "DarkBall");
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }
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
            ++Projectile.localAI[0];
            for (int i = 0; i < 2; i++)
            {
                if (Projectile.velocity.X != 0)
                    Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
                Projectile.rotation += 0.05f * Projectile.direction;
            }

            bool slowdown = true;
            if (Projectile.localAI[0] > 90f)
            {
                Player p = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
                if (p.active && !p.dead && !p.ghost)
                {
                    int distance = p.HasEffect<IronEquippedEffect>() ? 80 + IronEquippedEffect.GrabRangeBonus(p) / 2 : 80;
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

                            if (p.HasEffect<LithosphericEffect>() && p.HasEffect<PungentMinion>())
                            {
                                for (int i = 0; i < Main.maxProjectiles; i++)
                                {
                                    if (Main.projectile[i].type == ModContent.ProjectileType<PungentEyeballMinion>() && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].active)
                                    {
                                        Main.projectile[i].localAI[0] += 30; //charge
                                        break;
                                    }
                                }
                            }
                            return;
                        }
                    }
                }
            }
            if (slowdown)
                Projectile.velocity *= 0.95f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 200, Scale: 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2f;
                Main.dust[d].fadeIn = 0.5f;
            }
        }
    }
}