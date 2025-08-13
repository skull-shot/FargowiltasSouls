using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class IceShield : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureNPC(NPCID.Flocko);
        private Vector2 mousePos;
        private int syncTimer;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.timeLeft = 60 * 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.scale = 2;
        }
        public const int ShatterTime = 30;
        public bool Shattered => ShatterTimer >= ShatterTime;
        public ref float ShatterTimer => ref Projectile.ai[0];
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.HasEffect<IceShieldEffect>()))
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.timeLeft = 2;
            }

            if (!Shattered)
                Position(player);
            else
                Projectile.velocity = Vector2.Zero;

            /*
            for (int i = 0; i < 1; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, Scale: 1f);
                Main.dust[d].noGravity = true;
            }
            */

            if (!Shattered)
            {
                Main.projectile.Where(x => x.active && x.hostile && x.damage > 0 && x.Colliding(x.Hitbox, Projectile.Hitbox) && ProjectileLoader.CanDamage(x) != false && ProjectileLoader.CanHitPlayer(x, player) && FargoSoulsUtil.CanDeleteProjectile(x)).ToList().ForEach(x =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.Ice, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100);
                        Main.dust[dustId].noGravity = true;
                    }

                    x.Kill();

                    if (ShatterTimer < 1)
                        ShatterTimer = 1;
                });
            }

            if (ShatterTimer > 0)
            {
                ShatterTimer++;
                if (ShatterTimer == ShatterTime)
                {
                    SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
                    Projectile.position = Projectile.Center;
                    Projectile.width *= 3;
                    Projectile.height *= 3;
                    Projectile.Center = Projectile.position + player.DirectionTo(Projectile.position) * 100;
                    Projectile.Opacity = 0;

                    for (int i = 0; i < 40; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, Scale: 2f);
                        Main.dust[d].velocity = (Main.dust[d].position - player.Center).SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(7, 9);
                        Main.dust[d].noGravity = true;
                    }
                        
                }
                if (ShatterTimer > ShatterTime + 5)
                {
                    Projectile.Kill();
                    return;
                }
            }
        }

        private Vector2 MousePos(Player player) => player.Center + player.Center.SafeDirectionTo(Main.MouseWorld) * 150;

        private void Position(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                mousePos = MousePos(player);

                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }

            Vector2 desiredPos = mousePos;
            Projectile.velocity = (desiredPos - Projectile.Center) / 5;
        }

        public override bool? CanDamage()
        {
            if (Shattered)
                return base.CanDamage();
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
    }
}