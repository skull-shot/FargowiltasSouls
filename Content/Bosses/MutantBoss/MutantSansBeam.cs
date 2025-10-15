﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantSansBeam : BaseDeathray
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Deathrays", "GolemBeam");
        public MutantSansBeam() : base(420) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hide = true;
        }
        public override bool CanHitPlayer(Player target)
        {
            return target.hurtCooldowns[1] == 0;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > descendTime;

        public override void AI()
        {
            Projectile.alpha = 0;

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            Projectile head = FargoSoulsUtil.ProjectileExists(FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, Projectile.ai[1]), ModContent.ProjectileType<MutantSansHead>());
            if (head != null)
            {
                Projectile.Center = head.Center + Projectile.velocity * 16 * 3;
                if (Projectile.whoAmI < head.whoAmI)
                    Projectile.position += head.velocity;
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/VanillaEternity/Golem/GolemBeam"), Projectile.Center);
            }
            float num801 = 1.3f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 2400f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 1800f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;

            if (Projectile.localAI[0] <= descendTime)
            {
                float targetLength = Math.Max(num807, 250 + 70);
                Projectile.localAI[1] = MathHelper.Lerp(0, targetLength, Projectile.localAI[0] / descendTime);

                if (++Projectile.frameCounter > 3)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindProjectiles.Add(index);
        }

        const int descendTime = 50;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
        }

        Rectangle Frame(Texture2D tex)
        {
            int frameHeight = tex.Height / Main.projFrames[Projectile.type];
            return new Rectangle(0, frameHeight * Projectile.frame, tex.Width, frameHeight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            SpriteEffects spriteEffects = SpriteEffects.None;

            Main.spriteBatch.UseBlendState(BlendState.Additive);

            Texture2D texture2D19 = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D20 = ModContent.Request<Texture2D>($"{Texture}2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D21 = ModContent.Request<Texture2D>($"{Texture}3", AssetRequestMode.ImmediateLoad).Value;

            float rayLength = Projectile.localAI[1];
            Texture2D arg_ABD8_1 = texture2D19;
            Vector2 arg_ABD8_2 = Projectile.Center - Main.screenPosition;
            Rectangle sourceRectangle2 = texture2D19.Bounds;
            Texture2D arg_AE2D_1 = texture2D21;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color trailColor = Projectile.GetAlpha(Color.LightSkyBlue) * 2f;
                trailColor *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 oldPos = Projectile.oldPos[i];
                float oldRot = Projectile.oldRot[i];
                Vector2 oldCenter = oldPos + Projectile.Size / 2f;

                Main.EntitySpriteDraw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, lightColor, Projectile.rotation, sourceRectangle2.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
                rayLength -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
                oldCenter += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;
                if (rayLength > 0f)
                {
                    float num224 = 0f;
                    Rectangle rectangle7 = texture2D20.Bounds;
                    while (num224 < rayLength)
                    {
                        Main.EntitySpriteDraw(texture2D20, oldCenter - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), trailColor, oldRot, rectangle7.Size() / 2, Projectile.scale, spriteEffects, 0);
                        num224 += rectangle7.Height * Projectile.scale;
                        oldCenter += Projectile.velocity * rectangle7.Height * Projectile.scale;
                    }
                }
                Vector2 oldDrawPos = oldCenter - Main.screenPosition;
                sourceRectangle2 = texture2D21.Bounds;
                Main.EntitySpriteDraw(arg_AE2D_1, oldDrawPos, sourceRectangle2, trailColor, oldRot, sourceRectangle2.Size() / 2, Projectile.scale, spriteEffects, 0);
            }
            Main.spriteBatch.ResetToDefault();


            Main.EntitySpriteDraw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, lightColor, Projectile.rotation, sourceRectangle2.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            rayLength -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center;
            value20 += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;
            if (rayLength > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = texture2D20.Bounds;
                while (num224 < rayLength)
                {
                    Main.EntitySpriteDraw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), Lighting.GetColor((int)value20.X / 16, (int)value20.Y / 16), Projectile.rotation, rectangle7.Size() / 2, Projectile.scale, spriteEffects, 0);
                    num224 += rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * rectangle7.Height * Projectile.scale;
                }
            }
            Vector2 arg_AE2D_2 = value20 - Main.screenPosition;
            sourceRectangle2 = texture2D21.Bounds;
            Main.EntitySpriteDraw(arg_AE2D_1, arg_AE2D_2, sourceRectangle2, Lighting.GetColor((int)value20.X / 16, (int)value20.Y / 16), Projectile.rotation, sourceRectangle2.Size() / 2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}