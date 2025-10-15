﻿using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviSparklingLove : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/FinalUpgrades", "SparklingLove");

        public int scaleCounter;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        const int maxTime = 60 * 10;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = maxTime;
            Projectile.alpha = 250;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            //the important part
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<DeviBoss>());
            if (npc != null)
            {
                if (Projectile.localAI[0] == 0)
                {
                    Projectile.localAI[0] = 1;
                    if (WorldSavingSystem.MasochistModeReal)
                        Projectile.localAI[0] = 30;
                    Projectile.localAI[1] = Projectile.DirectionFrom(npc.Center).ToRotation();
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -17);
                }

                //not important part
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 4;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }

                //important again
                if (++Projectile.localAI[0] > 31)
                {
                    Projectile.localAI[0] = 1;
                    if (++scaleCounter < 3)
                    {
                        Projectile.position = Projectile.Center;

                        Projectile.width *= 2;
                        Projectile.height *= 2;
                        Projectile.scale *= 2;

                        Projectile.Center = Projectile.position;

                        MakeDust();

                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -16 + scaleCounter);

                        

                    }
                }

                Vector2 offset = new Vector2(Projectile.ai[1], 0).RotatedBy(npc.ai[3] + Projectile.localAI[1]);
                Projectile.Center = npc.Center + offset * Projectile.scale;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.timeLeft == 2)
            {
                //SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);

                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -14);

                if (!Main.dedServ && Main.LocalPlayer.active)
                    ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);
                
                    
                

                if (FargoSoulsUtil.HostCheck)
                {
                    float baseRotation = WorldSavingSystem.EternityMode ? Main.rand.NextFloat(MathHelper.TwoPi) : 0;

                    int max = 8;
                    if (WorldSavingSystem.EternityMode)
                        max = 12;
                    if (WorldSavingSystem.MasochistModeReal)
                        max = 8; //lowered because maso doubles down on it

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 target = 600 * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / max * i + baseRotation);
                        Vector2 speed = 2 * target / 90;
                        float acceleration = -speed.Length() / 90;
                        float rotation = speed.ToRotation();
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                            (int)(Projectile.damage * 0.75), 0f, Main.myPlayer, rotation + MathHelper.PiOver2, acceleration);

                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation);
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation + MathHelper.PiOver2);
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation - MathHelper.PiOver2);
                    }

                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 target = 300 * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / max * (i + 0.5f) + baseRotation);
                            Vector2 speed = 2 * target / 90;
                            float acceleration = -speed.Length() / 90;
                            float rotation = speed.ToRotation();
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                (int)(Projectile.damage * 0.75), 0f, Main.myPlayer, rotation + MathHelper.PiOver2, acceleration);

                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation);
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation + MathHelper.PiOver2);
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation - MathHelper.PiOver2);
                        }
                    }
                }
            }

            Projectile.direction = Projectile.spriteDirection = npc.direction;
            Projectile.rotation = npc.ai[3] + Projectile.localAI[1] + (float)Math.PI / 2 + (float)Math.PI / 4;
            if (Projectile.spriteDirection >= 0)
                Projectile.rotation -= (float)Math.PI / 2;
        }

        public override void OnKill(int timeLeft)
        {
            MakeDust();
        }

        public override void OnSpawn(IEntitySource source)
        {   
            if (FargoSoulsUtil.HostCheck)
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Siblings/Deviantt/DeviAxeSizeup" + Main.rand.Next(1, 3)), Projectile.Center);
            base.OnSpawn(source);
        }

        private void MakeDust()
        {
            Vector2 start = Projectile.width * Vector2.UnitX.RotatedBy(Projectile.rotation - (float)Math.PI / 4);
            if (Math.Abs(start.X) > Projectile.width / 2) //bound it so its always inside projectile's hitbox
                start.X = Projectile.width / 2 * Math.Sign(start.X);
            if (Math.Abs(start.Y) > Projectile.height / 2)
                start.Y = Projectile.height / 2 * Math.Sign(start.Y);
            int length = (int)start.Length();
            start = Vector2.Normalize(start);
            float scaleModifier = scaleCounter / 3f + 0.5f;
            for (int j = -length; j <= length; j += 80)
            {
                Vector2 dustPoint = Projectile.Center + start * j;
                dustPoint.X -= 23;
                dustPoint.Y -= 23;

                /*for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier * 2f);
                    Main.dust[dust].velocity *= 1.4f * scaleModifier;
                }*/

                for (int index1 = 0; index1 < 15; ++index1)
                {
                    int index2 = Dust.NewDust(dustPoint, 46, 46, DustID.GemAmethyst, 0f, 0f, 0, new Color(), scaleModifier * 2.5f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 16f * scaleModifier;
                    int index3 = Dust.NewDust(dustPoint, 46, 46, DustID.GemAmethyst, 0f, 0f, 0, new Color(), scaleModifier);
                    Main.dust[index3].velocity *= 8f * scaleModifier;
                    Main.dust[index3].noGravity = true;
                }

                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(dustPoint, 46, 46, DustID.GemAmethyst, 0f, 0f, 0, new Color(), Main.rand.NextFloat(1f, 2f) * scaleModifier);
                    Main.dust[d].velocity *= Main.rand.NextFloat(1f, 4f) * scaleModifier;
                }
            }
        }

        /*public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.velocity.X = target.Center.X < Main.npc[(int)Projectile.ai[0]].Center.X ? -15f : 15f;
            target.velocity.Y = -10f;
            target.AddBuff(ModContent.BuffType<Lovestruck>(), 120);
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = Vector2.Zero;
            if (Projectile.timeLeft > 30)
            {
                float amount = 4f * (1f - (Projectile.timeLeft / 180f));
                offset = Main.rand.NextVector2Circular(amount, amount);
            }
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<DeviBoss>());
            if (npc != null && npc.velocity != Vector2.Zero)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

                Terraria.Graphics.Shaders.ArmorShaderData shader = Terraria.Graphics.Shaders.GameShaders.Armor.GetShaderFromItemId(ItemID.PinkDye);
                shader.Apply(Projectile, new Terraria.DataStructures.DrawData?());

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = new(255, 255, 255, 50);
                    color27 *= 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            Texture2D texture2D14 = FargoAssets.GetTexture2D("Content/Items/Weapons/FinalUpgrades", "SparklingLove_glow").Value;
            Main.EntitySpriteDraw(texture2D14, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}