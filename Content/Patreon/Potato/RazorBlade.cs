using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Potato
{
    public class RazorBlade : ModProjectile
    {
        private Vector2 mousePos;
        private int syncTimer;

        int MaxDistance = 100;
        public bool Retreating => Projectile.ai[0] == 2 && MathF.Abs(Projectile.velocity.ToRotation() - Projectile.DirectionTo(Main.player[Projectile.owner].Center).ToRotation()) % MathF.Tau < MathF.PI;

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20; //
            Projectile.penetrate = -1;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }


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

            if (!player.Alive() || !player.GetModPlayer<PatreonPlayer>().RazorContainer  || Projectile.hostile) // if it was turned hostile by something
                Projectile.Kill();

            if (Main.projectile.Any(p => p.TypeAlive(Type) && p.owner == Projectile.owner && p.whoAmI < Projectile.whoAmI)) // if this is a duplicate
                Projectile.Kill();

            Projectile.timeLeft++;
            Projectile.rotation += 0.3f;

            if (Projectile.ai[0] != 1)
                Projectile.ai[1] = 0;

            if (Projectile.Distance(player.Center) > 750) // Maximum chain extension
                Projectile.Center = player.Center + player.DirectionTo(Projectile.Center) * 750;

            Projectile.tileCollide = true;

            switch (Projectile.ai[0])
            {
                //default, follow mouse, but limited radius around player
                case 0:

                    // if WAY too far, teleport
                    if (Projectile.Distance(player.Center) > 1200)
                        Projectile.Center = player.Center;

                    if (Projectile.Distance(player.Center) > MaxDistance * 1.5f)
                        Projectile.ai[0] = 2;

                    if (player.whoAmI == Main.myPlayer)
                    {
                        mousePos = Main.MouseWorld;

                        if (++syncTimer > 20)
                        {
                            syncTimer = 0;
                            Projectile.netUpdate = true;
                        }
                    }

                    float distance = MathF.Min(MaxDistance, player.Distance(mousePos));
                    Vector2 angle = Vector2.Normalize(mousePos - player.Center);
                    Vector2 desiredPos = player.Center + (angle * distance);
                    Vector2 desiredVel = (desiredPos - Projectile.Center);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVel, 0.08f);
                    Projectile.velocity = Projectile.velocity.ClampLength(0, 14);

                    Projectile.velocity += player.velocity / 3;
                    break;
                //after hit by sword, just fly straight 
                case 1:
                    if (Projectile.ai[1]++ > 10)
                    {
                        Projectile.ai[0] = 2;
                    }

                    break;
                //returning to player
                case 2:
                    if (Retreating)
                        Projectile.tileCollide = false;
                    Vector2 desiredReturnVel = Vector2.Normalize(player.Center - Projectile.Center) * 25;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredReturnVel, 0.08f);
                    distance = (int)Vector2.Distance(Projectile.Center, player.Center);

                    if (distance <= MaxDistance && (!Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height)) || distance < Projectile.width / 2)
                    {
                        Projectile.ai[0] = 0;
                    }

                    break;
            }
            //if (Collision.SolidTiles(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height))
                //Projectile.tileCollide = false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.5f }, Projectile.Center);
            }
            Projectile.soundDelay = 10;
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.5f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.5f;
            }
            return false;
        }
        public virtual Asset<Texture2D> ChainTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Content/Patreon/Potato/RazorChain");

        public override bool PreDraw(ref Color lightColor)
        {
            //dont draw when right on the player
            Player player = Main.player[Projectile.owner];

            if (Vector2.Distance(player.Center, Projectile.Center) < 5)
            {
                return true;
            }

            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 origin = new(ChainTexture.Width() * 0.5f, ChainTexture.Height() * 0.5f);
            float textureHeight = ChainTexture.Height();
            Vector2 mountedPosition = mountedCenter - position;
            float rotation = (float)Math.Atan2(mountedPosition.Y, mountedPosition.X) - 1.57f;
            bool invalidPosition = !(float.IsNaN(position.X) && float.IsNaN(position.Y) ||
                                     float.IsNaN(mountedPosition.X) && float.IsNaN(mountedPosition.Y));

            int numLoops = 0;

            while (invalidPosition)
            {
                if (numLoops++ > 100)
                {
                    break;
                }

                if (mountedPosition.Length() < textureHeight + 1.0)
                    invalidPosition = false;
                else
                {
                    Vector2 mountedClone = mountedPosition;
                    mountedClone.Normalize();
                    position += mountedClone * textureHeight;
                    mountedPosition = mountedCenter - position;
                    Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));

                    Main.spriteBatch.Draw(((Texture2D)ChainTexture), position - Main.screenPosition, null, color2, rotation, origin,
                        1f, SpriteEffects.None, 0.0f);
                }
            }

            return true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //bonus dmg when being launched
            if (Projectile.ai[0] == 1)
            {
                modifiers.SetCrit();
                modifiers.ArmorPenetration += 10;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!(Main.player[Projectile.owner].Alive() && Retreating))
                Projectile.velocity *= -0.5f;
            if (Projectile.ai[0] == 1)
                Projectile.ai[0] = 2;
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
