using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.Minions
{
    public class SaucerRocket : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_448";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rocket");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0) //when first spawned just move straight
            {
                Projectile.timeLeft++; //don't expire while counting down

                if (--Projectile.ai[1] == 0) //do for one tick right before homing
                {
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() + 6f);
                    Projectile.netUpdate = true;
                }
            }
            else //start homing
            {
                if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs && Main.npc[(int)Projectile.ai[0]].CanBeChasedBy()) //have target
                {
                    double num4 = (double)(Main.npc[(int)Projectile.ai[0]].Center - Projectile.Center).ToRotation() - (double)Projectile.velocity.ToRotation();
                    if (num4 > Math.PI)
                        num4 -= 2.0 * Math.PI;
                    if (num4 < -1.0 * Math.PI)
                        num4 += 2.0 * Math.PI;
                    Projectile.velocity = Projectile.velocity.RotatedBy(num4 * 0.2f, new Vector2());
                }
                else //retarget
                {
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000, true);
                    Projectile.netUpdate = true;
                    if (Projectile.ai[0] == -1) //no valid targets, selfdestruct
                        Projectile.Kill();
                }

                Projectile.tileCollide = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
            Vector2 vector21 = Vector2.UnitY.RotatedBy(Projectile.rotation, new Vector2()) * 8f * 2;
            int index21 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 1f);
            Main.dust[index21].position = Projectile.Center + vector21;
            Main.dust[index21].noGravity = true;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }

            if (Projectile.ai[2] >= 9 && Projectile.wet) //liquid rockets
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            int explosionsize = 64;
            Point point = Projectile.Center.ToTileCoordinates();
            explosionsize = Projectile.ai[2] switch
            {
                1 or 2 or 7 or 8 => 64,
                3 or 4 => 100,
                5 or 6 => 125,
                >= 9 => 24,
                _ => 64,
            };
            switch (Projectile.ai[2])
            {
                case 2:
                case 4:
                case 6:
                case 8: //nightmare vanilla tile breaking explosion code
                    int explrad = Projectile.ai[2] == 6 ? 7 : Projectile.ai[2] == 4 ? 5 : 3;
                    int minI = (int)(Projectile.position.X / 16f - explrad);
                    int maxI = (int)(Projectile.position.X / 16f + explrad);
                    int minJ = (int)(Projectile.position.Y / 16f - explrad);
                    int maxJ = (int)(Projectile.position.Y / 16f + explrad);
                    if (minI < 0) minI = 0; if (maxI > Main.maxTilesX) maxI = Main.maxTilesX; if (minJ < 0) minJ = 0; if (maxJ > Main.maxTilesY) maxJ = Main.maxTilesY;
                    Projectile.ExplodeTiles(Projectile.position, explrad, minI, maxI, minJ, maxJ, Projectile.ShouldWallExplode(Projectile.position, explrad, minI, maxI, minJ, maxJ));
                    break;

                case 9: FuckUpWorld(point, 3.5f, DelegateMethods.SpreadDry); break;
                case 10: FuckUpWorld(point, 3f, DelegateMethods.SpreadWater); break;
                case 11: FuckUpWorld(point, 3f, DelegateMethods.SpreadLava); break;
                case 12: FuckUpWorld(point, 3f, DelegateMethods.SpreadHoney); break;
            }

            if (Projectile.penetrate > -1)
            {
                Projectile.penetrate = -1;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = explosionsize;
                Projectile.position.X -= Projectile.width / 2;
                Projectile.position.Y -= Projectile.height / 2;
                for (int i = 0; i < 2; ++i)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                for (int i = 0; i < 10; ++i)
                {
                    int i2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 2.5f);
                    Main.dust[i2].noGravity = true;
                    Main.dust[i2].velocity *= 3f;
                    int i3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Alpha: 100, Scale: 1.5f);
                    Main.dust[i3].velocity *= 2f;
                    Main.dust[i3].noGravity = true;
                }
                Projectile.Damage();

                if (Projectile.ai[2] == 7 || Projectile.ai[2] == 8) //cluster
                {
                    for (float i = 0f; i < 0.25f; i += 1f / 6f)
                    {
                        Vector2 vel = (Main.rand.NextFloat() * ((float)Math.PI * 2f) + i * ((float)Math.PI * 2f)).ToRotationVector2() * (4f + Main.rand.NextFloat() * 2f);
                        int c = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel + Vector2.UnitY * -1f, Projectile.ai[2] == 8 ? ProjectileID.ClusterSnowmanFragmentsII : ProjectileID.ClusterSnowmanFragmentsI, Projectile.damage / 2, 0f, Projectile.owner);
                        Main.projectile[c].timeLeft -= Main.rand.Next(30);
                        Main.projectile[c].usesIDStaticNPCImmunity = true;
                        Main.projectile[c].idStaticNPCHitCooldown = 10; //todo: shared static
                    }
                }
            }
        }

        private static void FuckUpWorld(Point pt, float size, Utils.TileActionAttempt plot)
        {
            Tile tile = Main.tile[pt.X, pt.Y];
            if (tile != null && !tile.IsActuated && tile.IsHalfBlock)
            {
                int num = pt.Y - 1;
                if (num >= 0)
                {
                    tile = Main.tile[pt.X, num];
                    if (!WorldGen.SolidOrSlopedTile(tile))
                    {
                        pt.Y--;
                    }
                }
            }
            DelegateMethods.v2_1 = pt.ToVector2();
            DelegateMethods.f_1 = size;
            Utils.PlotTileArea(pt.X, pt.Y, plot);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}