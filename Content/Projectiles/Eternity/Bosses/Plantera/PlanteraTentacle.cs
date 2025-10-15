using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.Plantera
{
    public class PlanteraTentacle : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_264";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.PlanterasTentacle];
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            //CooldownSlot = ImmunityCooldownID.Bosses;

            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 360 * (Projectile.extraUpdates + 1);

            Projectile.hide = true;

            Projectile.FargoSouls().DeletionImmuneRank = 1;

            Projectile.FargoSouls().GrazeCheck =
                projectile =>
                {
                    float num6 = 0f;
                    if (CanDamage() == true && Collision.CheckAABBvLineCollision(Main.LocalPlayer.Hitbox.TopLeft(), Main.LocalPlayer.Hitbox.Size(),
                        new Vector2(Projectile.localAI[0], Projectile.localAI[1]), Projectile.Center, 22f * Projectile.scale + Main.LocalPlayer.FargoSouls().GrazeRadius * 2 + Player.defaultHeight, ref num6))
                    {
                        return true;
                    }
                    return false;
                };
        }

        public override bool? CanDamage()
        {
            return counter >= attackTime;
        }

        private int counter;
        private int attackTime = 150;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.Plantera);
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = Projectile.DirectionFrom(npc.Center).ToRotation() + MathHelper.Pi;
            Projectile.localAI[0] = npc.Center.X;
            Projectile.localAI[1] = npc.Center.Y;

            counter++;
            if (WorldSavingSystem.SwarmActive)
            {
                attackTime = 100;
            }
            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.frame = 0;
                //Projectile.timeLeft--;
            }
            else
            {
                if (counter == 1)
                    SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
                if (counter < attackTime)
                {
                    Projectile.position += npc.velocity / 3;

                    Vector2 target = npc.Center + (150f + counter * 1.5f) * Projectile.ai[1].ToRotationVector2();
                    Vector2 distance = target - Projectile.Center;
                    float length = distance.Length();
                    if (length > 10f)
                    {
                        distance /= 8f;
                        Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
                    }
                    else
                    {
                        if (Projectile.velocity.Length() < 12f)
                            Projectile.velocity *= 1.05f;
                    }
                }
                else if (counter == attackTime)
                {
                    Projectile.velocity = 32f * Projectile.ai[1].ToRotationVector2();
                    SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
                }
                else
                {
                    if (npc.HasPlayerTarget && Projectile.Distance(npc.Center) > npc.Distance(Main.player[npc.target].Center))
                    {
                        Tile tile = Framing.GetTileSafely(Projectile.Center);
                        if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon, -Projectile.velocity.X * 0.1f, -Projectile.velocity.Y * 0.1f, Scale: 2.5f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity *= 4f;
                            }

                            Projectile.velocity = Vector2.Zero;
                        }
                    }
                }

                if (++Projectile.frameCounter > 3 * (Projectile.extraUpdates + 1))
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);

            if (Projectile.localAI[0] != 0 && Projectile.localAI[1] != 0)
            {
                Vector2 planteraCenter = new(Projectile.localAI[0], Projectile.localAI[1]);

                int length = (int)Projectile.Distance(planteraCenter);
                const int increment = 512;
                for (int i = 0; i < length; i += increment)
                {
                    if (!Main.dedServ)
                        Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.SafeDirectionTo(planteraCenter) * (i + Main.rand.NextFloat(increment)), Vector2.Zero,
                        Main.rand.NextBool() ? 386 : 387, Projectile.scale);
                }
            }

            //Gore.NewGore(Projectile.Center, Vector2.Zero, mod.GetGoreSlot("Assets/Gores/Plantera/Gore_" + (Main.rand.NextBool() ? "388" : "389")), Projectile.scale);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<IvyVenomBuff>(), 240);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                    new Vector2(Projectile.localAI[0], Projectile.localAI[1]), Projectile.Center);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCs.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] != 0 && Projectile.localAI[1] != 0)
            {
                Texture2D texture = TextureAssets.Chain27.Value;
                Vector2 position = Projectile.Center;
                Vector2 mountedCenter = new(Projectile.localAI[0], Projectile.localAI[1]);
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
                float num1 = texture.Height;
                Vector2 vector24 = mountedCenter - position;
                float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                bool flag = true;
                if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                    flag = false;
                if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                    flag = false;
                while (flag)
                    if (vector24.Length() < num1 + 1.0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Vector2 vector21 = vector24;
                        vector21.Normalize();
                        position += vector21 * num1;
                        vector24 = mountedCenter - position;
                        Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                        color2 = Projectile.GetAlpha(color2);
                        Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);
                    }
            }

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPCID.PlanterasTentacle].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);

            // glow overlay
            float fadeInTime = attackTime;
            float fadeDelay = 60;
            float fadeOutTime = 30f;
            if (counter < fadeInTime + fadeDelay + fadeOutTime)
            {

                float glowOpacity;
                if (counter < fadeInTime)
                    glowOpacity = counter / fadeInTime;
                else if (counter < fadeInTime + fadeDelay)
                    glowOpacity = 1;
                else
                    glowOpacity = 1 - ((counter - fadeInTime - fadeDelay) / fadeOutTime);
                Color glowColor = Color.White * glowOpacity;

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                if (Projectile.localAI[0] != 0 && Projectile.localAI[1] != 0)
                {
                    Texture2D texture = TextureAssets.Chain27.Value;
                    Vector2 position = Projectile.Center;
                    Vector2 mountedCenter = new(Projectile.localAI[0], Projectile.localAI[1]);
                    Rectangle? sourceRectangle = new Rectangle?();
                    Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
                    float num1 = texture.Height;
                    Vector2 vector24 = mountedCenter - position;
                    float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                    bool flag = true;
                    if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                        flag = false;
                    if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                        flag = false;
                    while (flag)
                        if (vector24.Length() < num1 + 1.0)
                        {
                            flag = false;
                        }
                        else
                        {
                            Vector2 vector21 = vector24;
                            vector21.Normalize();
                            position += vector21 * num1;
                            vector24 = mountedCenter - position;
                            Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, glowColor, rotation, origin, 1f, SpriteEffects.None, 0);
                        }
                }
                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, Projectile.rotation, origin2, Projectile.scale, effects, 0);
                Main.spriteBatch.ResetToDefault();
            }
            return false;
        }
    }
}