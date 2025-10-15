﻿//using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.BanishedBaron;
using FargowiltasSouls.Content.Bosses.Lifelight;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class BloomLine : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles", Name);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bloom Line");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 1024;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;

            Projectile.hide = true;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public Color color = Color.White;

        public override bool? CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(counter);
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            counter = reader.ReadInt32();
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        private int counter;
        private readonly int drawLayers = 1;
        public override void AI()
        {
            int maxTime = 60;
            float alphaModifier = 3;



            switch ((int)Projectile.ai[0])
            {
                case -2: //lifelight line telegraphs, PINK TRACKING
                    {
                        color = Color.DeepPink;
                        alphaModifier = 1;
                        Projectile.scale = 0.6f;
                        maxTime = 60;
                        NPC lifelight = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<Lifelight>());
                        Player target = Main.player[lifelight.target];
                        if (lifelight != null && lifelight.active && target != null && target.active)
                        {
                            Projectile.rotation = lifelight.SafeDirectionTo(target.Center).ToRotation();
                            Projectile.Center = lifelight.Center;
                        }
                    }
                    break;
                case -1: //lifelight line telegraphs, YELLOW TRACKING
                    {
                        color = Color.Goldenrod;
                        alphaModifier = 1;
                        Projectile.scale = 0.6f;
                        maxTime = 80;
                        NPC lifelight = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<Lifelight>());
                        if (lifelight != null && lifelight.active)
                        {
                            Projectile.rotation = lifelight.As<Lifelight>().GunRotation;
                            Projectile.Center = lifelight.Center;
                        }
                    }
                    break;
                case 0: //lifelight line telegraphs, CYAN
                    {
                        color = Color.Cyan; //Color.Lerp(Color.DeepPink, Color.Magenta, 0.5f);
                        alphaModifier = 1;
                        Projectile.scale = 0.6f;
                        maxTime = 40;
                        Projectile.rotation = Projectile.ai[1];
                    }
                    break;
                case 1: //lifelight line telegraphs, PINK
                    {
                        color = Color.DeepPink; //Color.Lerp(Color.DeepPink, Color.Magenta, 0.5f);
                        alphaModifier = 1;
                        Projectile.scale = 0.6f;
                        maxTime = 60;
                        Projectile.rotation = Projectile.ai[1];
                    }
                    break;

                case 2: //banished baron dash telegraph
                    {
                        NPC baron = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<BanishedBaron>());
                        if (baron.Alive())
                        {
                            Projectile.rotation = baron.rotation;
                            Projectile.Center = baron.Center;
                        }
                        color = Color.Gray; //Color.Lerp(Color.DeepPink, Color.Magenta, 0.5f);
                        alphaModifier = 1;
                        Projectile.scale = 1f;
                        maxTime = 70;
                    }
                    break;
                case 3: //banished baron nuke telegraph
                    {
                        NPC baron = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<BanishedBaron>());
                        if (baron.Alive())
                        {
                            Projectile.rotation = baron.rotation;
                            Projectile.Center = baron.Center;
                        }
                        color = Color.Gray; //Color.Lerp(Color.DeepPink, Color.Magenta, 0.5f);
                        alphaModifier = 1;
                        Projectile.scale = 1f;
                        maxTime = 30;
                    }
                    break;
                case 4: //banished baron mine flurry telegraph
                    {
                        NPC baron = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<BanishedBaron>());
                        if (baron.Alive())
                        {
                            Projectile.rotation = baron.rotation;
                            Projectile.Center = baron.Center;
                        }
                        color = Color.Gray; //Color.Lerp(Color.DeepPink, Color.Magenta, 0.5f);
                        alphaModifier = 1;
                        Projectile.scale = 1f;
                        maxTime = 29;
                    }
                    break;

                case 5: //nebula pillar shot telegraph
                    {
                        Projectile.rotation = Projectile.ai[1];
                        color = Color.DeepPink;
                        alphaModifier = 1;
                        Projectile.scale = 1f;
                        maxTime = (int)Projectile.ai[2];
                        break;
                    }
                case 6: //sweeping nebula pillar shot telegraph
                    {
                        const int startAngle = 30;
                        ref float direction = ref Projectile.ai[1];
                        maxTime = (int)Projectile.ai[2];
                        float ratio = ((float)counter / maxTime);
                        float startRot = direction * MathHelper.ToRadians(startAngle);
                        float maxRot = MathHelper.ToRadians(150);
                        Projectile.rotation = (-Vector2.UnitY).RotatedBy(startRot).RotatedBy(direction * maxRot * ratio).ToRotation();
                        color = Color.DeepPink;
                        Projectile.scale = 1f;

                        break;
                    }
                case 7: //sweeping nebula pillar shot telegraph
                    {
                        NPC baron = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<BanishedBaron>());
                        if (baron.Alive())
                        {
                            Projectile.rotation = baron.rotation;
                            Projectile.Center = baron.Center + (baron.rotation.ToRotationVector2() * baron.width / 3);
                        }
                        color = Color.DeepPink;
                        alphaModifier = 1;
                        Projectile.scale = 1f;
                        maxTime = (int)Projectile.ai[2];

                        break;
                    }
                default:
                    Main.NewText("bloom line: you shouldnt be seeing this text, show terry or javyz");
                    break;
            }

            if (++counter > maxTime)
            {
                Projectile.Kill();
                return;
            }

            if (alphaModifier >= 0)
            {
                Projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * counter) * alphaModifier);
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            color.A = 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 3000f, 16f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D Texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = new(0, 0, Texture.Width, Texture.Height);
            Vector2 origin2 = rectangle.Size() / 2f;

            const int length = 3000;
            Vector2 offset = Projectile.rotation.ToRotationVector2() * length / 2f;
            Vector2 position = Projectile.Center - Main.screenLastPosition + new Vector2(0f, Projectile.gfxOffY) + offset;
            //const float resolutionCompensation = 128f / 24f; //i made the image higher res, this compensates to keep original display size
            Rectangle destination = new((int)position.X, (int)position.Y, length, (int)(rectangle.Height * Projectile.scale));

            Color drawColor = Projectile.GetAlpha(lightColor);

            for (int j = 0; j < drawLayers; j++)
                Main.EntitySpriteDraw(new DrawData(Texture, destination, new Rectangle?(rectangle), drawColor, Projectile.rotation, origin2, SpriteEffects.None, 0));

            //Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}
