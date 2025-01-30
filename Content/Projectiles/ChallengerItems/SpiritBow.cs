using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class SpiritBow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults() 
        {
            Projectile.width = 30;
            Projectile.height = 68;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        private const int chargeMax = 4;
        public ref float charge => ref Projectile.ai[0];
        public ref float soundTimer => ref Projectile.ai[1];
        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile.frame = (int)charge;
            Player player = Main.player[Projectile.owner];

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = player.DirectionTo(Main.MouseWorld);
            }

            float rot = Projectile.velocity.ToRotation();

            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            float offset = 0f * Projectile.scale;
            Projectile.Center = playerRotatedPoint + Projectile.rotation.ToRotationVector2() * offset;

            if (player.channel && !player.noItems && !player.CCed)
            {
                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);
                Projectile.timeLeft++;

                Projectile.rotation = rot + MathHelper.Pi;
                
                Projectile.direction = Main.MouseWorld.DirectionTo(player.Center).X < 0 ? 1 : -1;
                Projectile.spriteDirection = Projectile.direction;

                player.ChangeDir(Projectile.direction);
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rot - MathHelper.PiOver2);

                if (charge <= chargeMax) {
                    charge += 1 / 30f;
                }

                if (charge >= chargeMax - 0.2f)
                {
                    float shake = 0.8f;
                    Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
                }
                if (charge < chargeMax && charge > chargeMax - (1 / 30f) * 1.5f)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Particle p = new SmallSparkle(Projectile.Center, player.velocity - Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.Pi * 0.1f) * Main.rand.NextFloat(4f, 7f), Color.Magenta, Main.rand.NextFloat(0.25f, 0.75f), 20);
                        p.Spawn();
                    }
                    
                    SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
                }
                    

                if (soundTimer-- == 0)
                {
                    //SoundEngine.PlaySound(SoundID.NPCDeath6 with {Pitch = 0.1f + ((charge - 1) / 3) }, player.Center);
                    soundTimer = 10;
                }
            }
            else
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootspeed = new(10f * charge, 0f);
                    SoundEngine.PlaySound(SpiritLongbow.ReleaseSound with { Pitch = -0.5f }, player.Center);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, shootspeed.RotatedBy(rot), ModContent.ProjectileType<SpiritArrow>(), (int)(Projectile.damage * charge), 3f, Projectile.owner);
                }
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + MathHelper.Pi;
            int textureHeight = texture.Height / Main.projFrames[Type];
            Rectangle frame = new(0, textureHeight * (int)charge, texture.Width, textureHeight);
            int direction = Main.player[Projectile.owner].direction;
            SpriteEffects flip = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, drawPos, frame, lightColor, rot, new Vector2(frame.Width / 2, frame.Height / 2), Projectile.scale, flip);
            return false;
        }
    }
}
