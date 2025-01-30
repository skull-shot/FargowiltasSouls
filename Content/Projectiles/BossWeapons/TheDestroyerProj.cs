using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Linq;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using System;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class TheDestroyerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.RangeMultiplier = 0.6f;
            Projectile.WhipSettings.Segments = 50;
            Projectile.rotation += Projectile.ai[0];
        }
        int[] Cooldowns = new int[Main.maxProjectiles];
        public ref float LightningCounter => ref Projectile.ai[2];
        public override bool PreAI()
        {
            if (LightningCounter < 6)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Cooldowns[i] > 0)
                        Cooldowns[i]--;
                    Projectile p = Main.projectile[i];
                    if (p.Alive() && p.minion && p.owner == Projectile.owner && Cooldowns[p.whoAmI] <= 0 && Projectile.Colliding(Projectile.Hitbox, p.Hitbox))
                    {
                        NPC target = Projectile.FindTargetWithinRange(500, true);
                        //Projectile otherMinion = Main.projectile.Where(p2 => p2.Alive() && p2.whoAmI != p.whoAmI && p2.minion && Cooldowns[p2.whoAmI] <= 0 && p2.owner == Projectile.owner).OrderBy(x => x.Distance(Projectile.Center)).FirstOrDefault();
                        if (target != null)
                        {
                            SoundEngine.PlaySound(SoundID.Item33 with { Volume = 0.5f }, Projectile.Center);
                            if (FargoSoulsUtil.HostCheck)
                            {
                                int p2 = Projectile.NewProjectile(Projectile.InheritSource(Projectile), p.Center, p.DirectionTo(target.Center) * 8, ModContent.ProjectileType<PrimeLaser>(), Projectile.originalDamage / 3, Projectile.knockBack, Projectile.owner, ai2: p.Distance(target.Center));
                                if (p2.IsWithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[p2].DamageType = DamageClass.SummonMeleeSpeed;
                                    Main.projectile[p2].usesLocalNPCImmunity = true;
                                    Main.projectile[p2].localNPCHitCooldown = 60;
                                }
                            }
                                
                            Cooldowns[p.whoAmI] = 40;
                            LightningCounter++;
                        }
                    }
                }
            }
            return base.PreAI();
        }
        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Red);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = [];
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);


            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == 1)
                spriteEffects ^= SpriteEffects.FlipHorizontally;

            Texture2D value = TextureAssets.Projectile[Type].Value;
            Microsoft.Xna.Framework.Rectangle rectangle = value.Frame(1, 5);
            int height = rectangle.Height;
            rectangle.Height -= 2;
            Vector2 vector = rectangle.Size() / 2f;
            Vector2 vector2 = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                bool flag = false;
                Vector2 origin = vector;
                float scale = 1f;
                if (i == 0)
                {
                    origin.Y -= 4f;
                    flag = true;
                }
                else
                {
                    flag = true;
                    int num = 1;

                    if (i % 3 == 2)
                        num = 3;

                    rectangle.Y = height * num;
                }

                if (i == list.Count - 2)
                {
                    flag = true;
                    rectangle.Y = height * 4;
                    scale = 1.3f;
                    Projectile.GetWhipSettings(Projectile, out var timeToFlyOut, out var _, out var _);
                    float t = Projectile.ai[0] / timeToFlyOut;
                    float amount = Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true);
                    scale = MathHelper.Lerp(0.5f, 1.5f, amount);
                }

                Vector2 vector3 = list[i];
                Vector2 vector4 = list[i + 1] - vector3;
                if (flag)
                {
                    float rotation = vector4.ToRotation() - (float)Math.PI / 2f;
                    Microsoft.Xna.Framework.Color color = Lighting.GetColor(vector3.ToTileCoordinates());
                    Main.EntitySpriteDraw(value, vector2 - Main.screenPosition, rectangle, color, rotation, origin, scale, spriteEffects, 0f);
                }

                vector2 += vector4;
            }

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Whip damage falloff
            Projectile.damage = (int)(Projectile.damage * 0.7);
        }
    }
}
