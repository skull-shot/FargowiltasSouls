using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class PlanteraSpikevine : ModProjectile
    {
        const int TrailSaveLength = 1000;
        Vector2[] Trail = new Vector2[TrailSaveLength];
        public int TrailLength = 800;
        public override void SetStaticDefaults()
        {
            //ProjectileID.Sets.TrailCacheLength[Type] = 240;
            //ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }
        const int SpriteLength = 20;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 30;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;

            Projectile.light = 1;
            Projectile.scale = 1;
            Projectile.Opacity = 0;
            Projectile.hide = true;
        }
        ref float Timer => ref Projectile.ai[0];
        ref float Target => ref Projectile.ai[1];
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 position = Projectile.position;
            int index = 0;
            Vector2 difference = Trail[index + 1] - Trail[index];
            float lengthLeft = difference.Length();

            Texture2D spikeTexture = ModContent.Request<Texture2D>(Texture + "End").Value;
            if (targetHitbox.Intersects(projHitbox))
                return true;

            for (int i = 0; i < TrailLength; i++)
            {
                position += difference.SafeNormalize(Vector2.Zero) * SpriteLength * Projectile.scale;
                for (int j = 0; j < 1000; i++)
                {
                    lengthLeft -= SpriteLength * Projectile.scale;
                    if (lengthLeft < 0)
                    {
                        index++;
                        if (Trail[index + 1] != Vector2.Zero && Timer > index)
                        {
                            Vector2 dif = Trail[index + 1] - position;
                            if (dif != Vector2.Zero)
                            {
                                difference = dif;
                                lengthLeft = difference.Length();
                            }
                        }
                        else
                            return false;
                    }
                    else
                        break;
                }
                Rectangle hitbox = new((int)(position.X - projHitbox.Width / 2), (int)(position.Y - projHitbox.Height / 2), projHitbox.Width, projHitbox.Height);
                if (targetHitbox.Intersects(hitbox))
                    return true;
            }
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Trail[0] = Projectile.Center;
        }
        public override void AI()
        {
            if (Timer == 0)
            {
                Target = Player.FindClosest(Projectile.Center, 0, 0);
                Projectile.Opacity = 1;
            }
            int playerID = (int)Target;
            if (!playerID.IsWithinBounds(Main.maxProjectiles))
            {
                Projectile.Kill();
                return;
            }
            Player target = Main.player[playerID];
            if (!target.Alive())
            {
                Projectile.Kill();
                return;
            }
            if (Timer == 60)
                SoundEngine.PlaySound(SoundID.Item63 with { Pitch = -1f, Volume = 10 }, Projectile.Center);
            if (Timer < 60)
            {
                Projectile.velocity *= 0.97f;
                Projectile.velocity = Projectile.velocity.RotateTowards(Projectile.DirectionTo(target.Center).ToRotation(), 0.1f);
            }
            else if (Timer < 110)
            {
                Projectile.tileCollide = true;
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Projectile.velocity += dir * 1f;
                if (Projectile.velocity.Length() > 40)
                    Projectile.velocity = dir * 40;
            }

            if (Timer > 30)
            {
                TrailLength -= 4;
                if (TrailLength <= 0)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.velocity != Vector2.Zero)
            {
                for (int i = TrailSaveLength - 1; i >= 0; i--)
                {
                    if (i == 0)
                        Trail[i] = Projectile.Center;
                    else
                        Trail[i] = Trail[i - 1];
                }
            }
            Timer++;
            if (WorldSavingSystem.SwarmActive && Main.GameUpdateCount % 2 == 0)
            {
                Timer++;
                if (Timer == 61)
                    SoundEngine.PlaySound(SoundID.Item63 with { Pitch = -1f, Volume = 10 }, Projectile.Center);
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCs.Add(index);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Center += Projectile.velocity;
            Projectile.velocity *= 0;
            SoundEngine.PlaySound(SoundID.Item49, Projectile.Center);
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;

            Vector2 position = Projectile.Center;
            int index = 0;
            Vector2 difference = Trail[index + 1] - Trail[index];
            float lengthLeft = difference.Length();
            string variant = recolor ? "" : "Vanilla";
            Texture2D spikeTexture = ModContent.Request<Texture2D>(Texture + "End" + variant).Value;
            Texture2D texture = ModContent.Request<Texture2D>(Texture + variant).Value;

            int height = spikeTexture.Height;
            Rectangle rectangle = new(0, 0, spikeTexture.Width, height);
            Vector2 origin = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color color = Lighting.GetColor(position.ToTileCoordinates());
            Main.EntitySpriteDraw(spikeTexture, position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(color),
                (-difference).ToRotation(), origin, Projectile.scale, spriteEffects, 0);

            for (int i = 0; i < TrailLength; i++)
            {
                float opacity = 1f;
                int bound = 25;
                if (Timer > 200 && i > TrailLength - bound)
                    opacity *= 1 - ((i - (TrailLength - bound)) / bound);
                position += difference.SafeNormalize(Vector2.Zero) * SpriteLength * Projectile.scale;
                for (int j = 0; j < 1000; i++)
                {
                    lengthLeft -= SpriteLength * Projectile.scale;
                    if (lengthLeft < 0)
                    {
                        index++;
                        if (Trail[index + 1] != Vector2.Zero && Timer > index)
                        {
                            Vector2 dif = Trail[index + 1] - position;
                            if (dif != Vector2.Zero)
                            {
                                difference = dif;
                                lengthLeft = difference.Length();
                            }
                        }
                        else
                            return false;
                    }
                    else
                        break;
                }
               
                int frame = i % 4;
                height = texture.Height / 4;
                rectangle = new(0, height * frame, texture.Width, height);
                origin = rectangle.Size() / 2f;
                color = Lighting.GetColor(position.ToTileCoordinates());
                Main.EntitySpriteDraw(texture, position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(color) * opacity,
                    (-difference).ToRotation(), origin, Projectile.scale, spriteEffects, 0);
            }
            return false;
        }
    }
}
