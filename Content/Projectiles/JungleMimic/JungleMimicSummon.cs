﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.JungleMimic
{
    public class JungleMimicSummon : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/JungleMimic", Name);
        public int counter;
        public bool trailbehind;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jungle Mimic");
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 10;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.minionSlots = 2f;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 26;
            Projectile.width = 38;
            Projectile.height = 40;
            AIType = ProjectileID.BabySlime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }
        private float realFrameCounter;
        private int realFrame;
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<JungleMimicSummonBuff>());
            }

            if (player.HasBuff(ModContent.BuffType<JungleMimicSummonBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            counter++;
            if (counter % 15 == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    NPC targetNPC = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1000, true));
                    if (targetNPC != null)
                    {
                        Vector2 shootVel = Projectile.SafeDirectionTo(targetNPC.Center);
                        SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel * 14f + targetNPC.velocity / 2, ModContent.ProjectileType<JungleMimicSummonCoin>(), Projectile.damage / 4, Projectile.knockBack, Main.myPlayer);
                    }
                    else
                        counter = 0;
                }
            }

            if (counter > 180)
            {
                if (counter > 300)
                    counter = 0;

                if (Projectile.owner == Main.myPlayer)
                {
                    NPC targetNPC = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1000, true));
                    if (targetNPC != null)
                    {
                        Projectile.frame++;
                        trailbehind = true;
                        if (++Projectile.frameCounter >= 10)
                        {
                            Projectile.frame = 6;
                            Projectile.frameCounter = 6;
                        }

                        for (int index = 0; index < 1000; ++index)
                        {
                            if (index != Projectile.whoAmI && Main.projectile[index].active && Main.projectile[index].owner == Projectile.owner && Main.projectile[index].type == Projectile.type && (double)Math.Abs((float)(Projectile.position.X - Main.projectile[index].position.X)) + (double)Math.Abs((float)(Projectile.position.Y - Main.projectile[index].position.Y)) < Projectile.width)
                            {
                                if (Projectile.position.X < Main.projectile[index].position.X)
                                {
                                    Projectile.velocity.X -= 0.05f;
                                }
                                else
                                {
                                    Projectile.velocity.X += 0.05f;
                                }
                                if (Projectile.position.Y < Main.projectile[index].position.Y)
                                {
                                    Projectile.velocity.Y -= 0.05f;
                                }
                                else
                                {
                                    Projectile.velocity.Y += 0.05f;
                                }
                            }
                        }

                        Vector2 dashVel = Projectile.SafeDirectionTo(targetNPC.Center);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, dashVel * 18, 0.03f);
                        Projectile.rotation = 0;
                        Projectile.tileCollide = false;
                        Projectile.direction = Math.Sign(Projectile.velocity.X);
                        Projectile.spriteDirection = -Projectile.direction;
                        return false;
                    }
                }
            }
            trailbehind = false;
            Projectile.tileCollide = true;
            return true;
        }
        public override void AI()
        {
            if (Projectile.velocity.X == 0)
                realFrame = 0;
            else if (counter < 180)
            {
                realFrameCounter += System.Math.Abs(Projectile.velocity.X);
                if (++realFrameCounter > 24)
                {
                    realFrameCounter = 0;
                    realFrame++;
                }
                if (realFrame > 5)
                    realFrame = 2;
            }
            Projectile.frame = realFrame;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type] && trailbehind; i++)
            {
                Color color27 = Projectile.GetAlpha(lightColor) * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                    color27, num165, origin2, Projectile.scale, Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY - 4),
                new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2,
                Projectile.scale, Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }
    }
}

