using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class SandstoneBoulder : ModProjectile
    {
        private bool launched = false;
        private int bounceCount = 0;

        public override void SetDefaults() 
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.penetrate = 10;
        }

        public ref float timer => ref Projectile.ai[0];
        public ref float rot => ref Projectile.ai[1];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!launched)
            {
                timer++;
                if (timer >= 30 && !player.channel && !player.CCed && !player.noItems)
                {
                    float angle = Projectile.Center.AngleTo(Main.MouseWorld);
                    Projectile.velocity = new Vector2(20f, 0f).RotatedBy(angle);
                    Projectile.velocity += player.velocity / 1.25f;
                    launched = true;
                    Projectile.hide = false;
                    Projectile.tileCollide = true;
                    Projectile.friendly = true;
                    timer = 0;
                    SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.8f }, Projectile.Center);
                    return;
                }
                if (timer <= 30)
                {
                    rot = player.direction * (MathHelper.Pi - (timer * MathHelper.Pi / 30f));
                    if (Projectile.owner == Main.myPlayer)
                        FargoSoulsUtil.ScreenshakeRumble(1.5f);
                }
                Vector2 holdOffset = new Vector2(0f, -25f).RotatedBy(rot);
                player.itemRotation = rot + MathHelper.Pi;

                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);
                Projectile.Center = player.Center + holdOffset;

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rot + MathHelper.Pi);
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, rot + MathHelper.Pi);
                Projectile.direction = Main.MouseWorld.DirectionTo(player.Center).X < 0 ? 1 : -1;
                player.ChangeDir(Projectile.direction);
                Projectile.spriteDirection = Projectile.direction;

                Projectile.timeLeft++;
                Projectile.friendly = false;

            }
            else
            {
                if (timer++ < 7)
                {
                    rot += player.direction * MathHelper.Pi / 7f;
                    rot = MathHelper.Min(rot, MathHelper.Pi);
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rot + MathHelper.Pi);
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, rot + MathHelper.Pi);
                }
                Projectile.velocity.Y += 0.75f;
                Projectile.rotation += 0.2f;
                
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.life > Projectile.damage && Projectile.penetrate > 1)
                Projectile.penetrate = 1;

            float bonus = 0.2f * Projectile.MaxUpdates * Projectile.velocity.Length() / 20f;
            bonus = MathHelper.Clamp(bonus, 0, 0.2f);
            modifiers.SourceDamage *= 1 + bonus;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (bounceCount++ < 2 && oldVelocity.Y > 0 && Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) // if hitting ground, bounce, up to twice
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                return false;
            }
            Projectile.Kill();
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item70, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Projectile.Center, 25, 25, DustID.t_Honey);
            }
            int shrapnelCount = 8;
            int damage = Projectile.damage / 2;
            for (int i = 0; i < shrapnelCount; i++)
            {
                float direction = Main.rand.NextFloatDirection();
                if (Main.myPlayer == Projectile.owner) {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0f, -14f).RotatedBy(direction), ModContent.ProjectileType<SandstoneShrapnel>(), damage, 0.25f, Projectile.owner, ai1: Main.rand.Next(1,5));
                    ScreenShakeSystem.StartShake(2, shakeStrengthDissipationIncrement: 0.1f);
                } 
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation;
            SpriteEffects flip = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), lightColor, rot, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, flip);
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < 20; i++)
            {
                float xPos = player.Bottom.X + Main.rand.Next(-20, 20);
                float ySpeed = -2 * Main.rand.NextFloat();
                Vector2 dustPosition = new Vector2(xPos, player.Bottom.Y);
                Dust.NewDustPerfect(dustPosition, DustID.t_Honey, new Vector2(0, ySpeed));
            }
            SoundEngine.PlaySound(SoundID.Item69, Projectile.Center);
            base.OnSpawn(source);
        }
    }
}
