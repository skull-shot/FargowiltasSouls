using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Security.Policy;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops
{
    public class RockeaterLauncherHeld : ModProjectile
    {

        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/SwarmDrops", "EaterLauncher");
        public override void SetDefaults()
        {
            Projectile.width = 102;
            Projectile.height = 62;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = ProjAIStyleID.HeldProjectile;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
        }
        public float ShootTimer;
        public override bool? CanDamage() => false;
        bool canShoot = true;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center = player.RotatedRelativePoint(player.MountedCenter, true);

            if (player.dead || !player.active || !player.channel)
                Projectile.Kill();

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = player.DirectionTo(Main.MouseWorld);
            }

            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            Vector2 centerPos = new Vector2(Projectile.direction == 1 ? 4 : -5, 0) + Projectile.rotation.ToRotationVector2();

            Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity), Vector2.Normalize(Projectile.DirectionTo(centerPos)), 0.12f);

            if (player.channel && !player.noItems && !player.CCed)
            {
                Projectile.timeLeft++;
                player.SetDummyItemTime(25);
                player.itemTime = 60;
                player.itemAnimation = 60;
                player.heldProj = Projectile.whoAmI;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

                Projectile.direction = Main.MouseWorld.DirectionTo(player.Center).X < 0 ? 1 : -1;
                Projectile.spriteDirection = Projectile.direction;

                player.ChangeDir(Projectile.direction);
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);

                int aura = ModContent.ProjectileType<RockeaterAuraProj>();
                if (player.ownedProjectileCounts[aura] <= 0)
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Center, player.velocity, aura, 0, 0);

                Vector2 velocity = Projectile.velocity * 24f;
                
                Vector2 position = Projectile.Center + new Vector2(12, player.direction > 0 ? 22 : -22).RotatedBy(Projectile.rotation) + Vector2.Normalize(velocity) * Projectile.width * 0.9f;

                if (FargoSoulsUtil.HostCheck)
                {
                    ShootTimer += player.FargoSouls().AttackSpeed;
                    if (ShootTimer > 5 && canShoot)
                    {
                        canShoot = false;
                        SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.4f }, Projectile.Center); 
                        Projectile.velocity += new Vector2(6, 0).RotatedBy(Projectile.rotation);
                        for (int i = 0; i < 3; i++)
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), position, velocity.RotatedByRandom(MathHelper.ToRadians(18)) * Main.rand.NextFloat(0.9f, 1.1f), ModContent.ProjectileType<EaterRocket>(), Projectile.damage, 6f);
                    }

                    if (ShootTimer >= 25)
                    {
                        canShoot = true;
                        ShootTimer = 0;
                    }
                }
                
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + MathHelper.Pi;
            int direction = Main.player[Projectile.owner].direction;
            Rectangle frame = new(0, 0, texture.Width, texture.Height);
            SpriteEffects flip = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            if (player.channel)
                Main.EntitySpriteDraw(texture, drawPos, null, lightColor, rot, new Vector2(12, player.direction > 0 ? 48 : 12), Projectile.scale, flip);
            return false;
        }
    }
}
