using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomShip : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 590;
            Projectile.height = 574;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;

            Projectile.hide = true;
        }

        public override bool? CanDamage() => Projectile.scale >= 1 && !inBackground;

        Rectangle GetRectangleFromCenterPoint(Vector2 center, int width, int height)
        {
            int trueWidth = (int)(width * Projectile.scale);
            int trueHeight = (int)(height * Projectile.scale);
            return new Rectangle((int)center.X - trueWidth / 2, (int)center.Y - trueHeight / 2, trueWidth, trueHeight);
        }

        Vector2 GetOffsetFromCenterToSpriteCoords(float x, float y)
        {
            Vector2 offset = new Vector2(x, y) - new Vector2(295, 287);
            offset.X *= Projectile.direction;
            offset *= Projectile.scale;
            return offset;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Main.getGoodWorld) //lmao the entire sprite is a hitbox
                return base.Colliding(projHitbox, targetHitbox);

            Vector2 center = projHitbox.Center.ToVector2();
            Rectangle shipBack = GetRectangleFromCenterPoint(center + GetOffsetFromCenterToSpriteCoords(106, 413), 210, 240);
            Rectangle shipHull = GetRectangleFromCenterPoint(center + GetOffsetFromCenterToSpriteCoords(215, 455), 420, 150);

            Rectangle tipBase = GetRectangleFromCenterPoint(center + GetOffsetFromCenterToSpriteCoords(462, 412), 78, 24);
            Rectangle tipTip = GetRectangleFromCenterPoint(center + GetOffsetFromCenterToSpriteCoords(544, 400), 90, 16);
            //only enable tip hitbox once ramming begins
            if (BehaviorTimer > 0 && System.Math.Sign(Projectile.velocity.X) == Direction
                && (targetHitbox.Intersects(tipBase) || targetHitbox.Intersects(tipTip)))
                return true;

            return targetHitbox.Intersects(shipBack) || targetHitbox.Intersects(shipHull);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (inBackground)
                behindNPCsAndTiles.Add(index);
            else
                behindProjectiles.Add(index); //todo: fix this not actually working (ship draws over its own bullets anyway)
        }

        ref float Direction => ref Projectile.ai[0];
        ref float BehaviorTimer => ref Projectile.ai[1]; //is set negative by abom
        ref float Abom => ref Projectile.ai[2];
        ref float SpawnInTimer => ref Projectile.localAI[0];

        bool inBackground;
        float[] gunRotations = new float[4];

        // todo: delete this, unneeded
        public const float Y_OFFSET_TO_ACTUAL_SHIP_HITBOX_CENTER = 132f;

        Vector2[] GetGunPositions()
        {
            List<Vector2> guns = new List<Vector2>();
            for (int i = 0; i < 4; i++)
                guns.Add(Projectile.Center + GetOffsetFromCenterToSpriteCoords(86 + 69 * i, 452));
            return guns.ToArray();
        }

        public override void AI()
        {
            NPC abom = FargoSoulsUtil.NPCExists(Abom, ModContent.NPCType<AbomBoss>());
            if (abom == null)
            {
                Projectile.Kill();
                return;
            }

            if (SpawnInTimer == 0)
            {
                for (int i = 0; i < gunRotations.Length; i++)
                {
                    int flip = i % 2 == 0 ? -1 : 1;
                    gunRotations[i] = flip * MathHelper.ToRadians(60);
                    gunRotations[i] += flip * i * MathHelper.ToRadians(120) / gunRotations.Length;
                    gunRotations[i] += Main.rand.NextFloat(MathHelper.ToRadians(10));
                }
            }

            BehaviorTimer++;
            SpawnInTimer++;

            const int ramTime = 75;
            const int shrinkTime = 30;
            const int ballsTime = 180;

            Projectile.direction = Projectile.spriteDirection = (int)Direction;

            if (BehaviorTimer < 0) //spawn in, spray shots
            {
                Projectile.velocity = Vector2.Zero;

                const int timeToFinishSpawnAnim = 60;
                float ratio = System.Math.Min(1f, SpawnInTimer / timeToFinishSpawnAnim);
                Projectile.Center = abom.Center;
                Projectile.position.X += 145f * Projectile.direction;
                Projectile.position.X -= 1000f * (float)System.Math.Cos(MathHelper.PiOver2 * ratio);

                Projectile.scale = ratio;

                for (int i = 0; i < gunRotations.Length; i++)
                {
                    int flip = i % 2 == 0 ? -1 : 1;
                    flip *= Projectile.direction;
                    gunRotations[i] += flip * MathHelper.Pi * 1.75f / 240 * Main.rand.NextFloat(0.95f, 1.05f);
                }

                if (ratio >= 1f && BehaviorTimer % 15 == 0 && FargoSoulsUtil.HostCheck && abom.HasValidTarget)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 gunPos = GetGunPositions()[i];
                        Vector2 vel = 9f * gunRotations[i].ToRotationVector2();
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), gunPos, vel, ModContent.ProjectileType<AbomCannonball>(), Projectile.damage, 0f, Main.myPlayer, 0, 9000, 1);
                    }
                }
            }
            else if (BehaviorTimer == 0) //detach from abom, rear back
            {
                Projectile.netUpdate = true;
                Projectile.velocity = 24f * Vector2.UnitX * -Direction;
            }
            else if (BehaviorTimer < ramTime) //ram
            {
                Projectile.velocity.X += 1.3f * Direction;
            }
            else if (BehaviorTimer < ramTime + shrinkTime) //turn around, go into background
            {
                if (BehaviorTimer == ramTime)
                {
                    Projectile.netUpdate = true;
                    Direction *= -1;
                }
                Projectile.velocity *= 0.9f;

                Projectile.scale -= 0.7f / shrinkTime;
                if (Projectile.scale < 0.3f)
                    Projectile.scale = 0.3f;
            }
            else
            {
                Projectile.velocity.X = 24f * Direction;
                Projectile.scale = 0.3f;

                if (abom.HasValidTarget) //keep ship y pos near abom & player so its always visible as it passes
                {
                    float newCenterY = MathHelper.Lerp(Projectile.Center.Y, (abom.Center.Y + Main.player[abom.target].Center.Y) / 2, 0.04f);
                    Projectile.Center = new Vector2(Projectile.Center.X, newCenterY);

                    //wait so im on screen closer to player while attacking
                    if (BehaviorTimer > ramTime + 60)
                    {
                        //random bombardment
                        if (BehaviorTimer < ramTime + ballsTime && BehaviorTimer % (WorldSavingSystem.MasochistModeReal ? 1 : 2) == 0)
                            ShootCannonball(Main.player[abom.target].Center, true);

                        //directly target player, force movement
                        if (BehaviorTimer < ramTime + ballsTime + 30 && BehaviorTimer % 30 == 1)
                            ShootCannonball(Main.player[abom.target].Center, false);
                    }
                }
            }

            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }

            //debug dust to verify hitboxes
            /*Vector2 center = Projectile.Center;
            Rectangle shipBack = GetRectangleFromCenterPoint(center + GetOffsetFromCenterToSpriteCoords(106, 413), 210, 240);
            Rectangle shipHull = GetRectangleFromCenterPoint(center + GetOffsetFromCenterToSpriteCoords(215, 455), 420, 150);
            Rectangle tipBase = GetRectangleFromCenterPoint(center + GetOffsetFromCenterToSpriteCoords(462, 412), 78, 24);
            Rectangle tipTip = GetRectangleFromCenterPoint(center + GetOffsetFromCenterToSpriteCoords(544, 400), 90, 16);
            for (int i = 0; i < 50; i++)
            {
                int d = Dust.NewDust(shipBack.TopLeft(), shipBack.Width, shipBack.Height, DustID.Vortex);
                Main.dust[d].velocity = Vector2.Zero;
                Main.dust[d].noGravity = true;
                d = Dust.NewDust(shipHull.TopLeft(), shipHull.Width, shipHull.Height, DustID.Vortex);
                Main.dust[d].velocity = Vector2.Zero;
                Main.dust[d].noGravity = true;
                d = Dust.NewDust(tipBase.TopLeft(), tipBase.Width, tipBase.Height, DustID.Vortex);
                Main.dust[d].velocity = Vector2.Zero;
                Main.dust[d].noGravity = true;
                d = Dust.NewDust(tipTip.TopLeft(), tipTip.Width, tipTip.Height, DustID.Vortex);
                Main.dust[d].velocity = Vector2.Zero;
                Main.dust[d].noGravity = true;
            }*/
        }

        void ShootCannonball(Vector2 target, bool useRandomAim)
        {
            Vector2 gunPos = GetGunPositions()[Main.rand.Next(4)];
            if (useRandomAim)
            {
                const float targetingRadius = 1200;
                target += Main.rand.NextVector2Circular(targetingRadius, targetingRadius);
            }

            const float gravity = 0.2f;
            const int time = 120;

            Vector2 vel = (target - gunPos) / time;
            vel.Y -= 0.5f * gravity * time;

            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), target, Vector2.Zero, ModContent.ProjectileType<AbomReticle2>(), 0, 0f, Main.myPlayer, time);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), gunPos, vel, ModContent.ProjectileType<AbomCannonball>(), Projectile.damage, 0f, Main.myPlayer, gravity, time);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<AbomFangBuff>(), 240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            float scale = Projectile.scale * 1.02f;
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // todo: this could probably be prettied up more
            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.2f)
            {
                Color color27 = Color.Orange * 0.2f;
                color27.A = 30;
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}