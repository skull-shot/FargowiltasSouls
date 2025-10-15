using Fargowiltas.Common.Configs;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Bosses.VanillaEternity.LunaticCultist;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.LunaticCultist
{
    public class CultistIceBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_464";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball");
            Main.projFrames[Projectile.type] = 1;
        }
        public const int Width = 92;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = Width;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.penetrate = -1;

            Projectile.Opacity = 0f;
        }
        public ref float Rotation => ref Projectile.ai[0];
        public ref float Duration => ref Projectile.ai[1];
        public ref float OwnerID => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];

        public float ConeWidth = MathHelper.PiOver2 * 0.375f;
        public override void AI()
        {
            int ownerID = (int)OwnerID;

            if (!ownerID.IsWithinBounds(Main.maxNPCs) || !Main.npc[ownerID].Alive() || (Main.npc[ownerID].type != NPCID.CultistBoss && Main.npc[ownerID].type != NPCID.CultistBossClone))
            {
                Projectile.active = false;
                return;
            }
            NPC npc = Main.npc[ownerID];
            if (npc.HasPlayerTarget)
            {
                float lerpStrength = WorldSavingSystem.MasochistModeReal ? 0.04f : 0.035f;
                Rotation = Vector2.Lerp(Rotation.ToRotationVector2(), npc.DirectionTo(Main.player[npc.target].Center), lerpStrength).ToRotation();
            }
            Projectile.velocity = (npc.Center + Rotation.ToRotationVector2() * 80) - Projectile.Center;

            Projectile.Opacity = 1f;

            Projectile.position = Projectile.Center;
            Projectile.scale = (float)Math.Min(Timer / Duration * 1.5f, 1f);
            Projectile.width = Projectile.height = (int)(Width * Projectile.scale);
            Projectile.Center = Projectile.position;

            Projectile.rotation += Projectile.scale * MathF.Tau / 40;

            Timer++;
            if (Timer >= Duration)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item120, Projectile.Center);
            for (int i = 0; i < 90; i++)
            {
                float rot = Rotation + Main.rand.NextFloat(-ConeWidth, ConeWidth);
                float spd = Main.rand.NextFloat(16f, 38f);
                Vector2 dir = rot.ToRotationVector2();
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + dir * Projectile.width / 3, dir * spd, ModContent.ProjectileType<CultistIceShard>(), Projectile.damage, 1f, Projectile.owner);
            }
            return;
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                Projectile.penetrate = -1;
                Projectile.position = Projectile.Center;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Projectile.width = Projectile.height = 176;
                Projectile.Center = Projectile.position;
                //Projectile.Damage();
                for (int index1 = 0; index1 < 4; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                }
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0.0f, 0.0f, 200, new Color(), 3.7f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity *= 3f;
                    int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index3].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Dust dust2 = Main.dust[index3];
                    dust2.velocity *= 2f;
                    Main.dust[index3].noGravity = true;
                    Main.dust[index3].fadeIn = 2.5f;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0.0f, 0.0f, 0, new Color(), 2.7f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 3f;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 0, new Color(), 1.5f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 3f;
                }
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    int index2 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Gore gore = Main.gore[index2];
                    gore.velocity *= 0.3f;
                    Main.gore[index2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                    Main.gore[index2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 auraPos = Projectile.Center;

            Color darkColor = Color.Lerp(Color.Blue, Color.Cyan, 0.5f);
            Color mediumColor = Color.Lerp(Color.Cyan, Color.White, 0.8f);

            float innerRadius = 30 * Projectile.scale;

            Vector2 nearbyPosition = Rotation.ToRotationVector2();
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargoAssets.CracksNoise;
            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;
            var maxOpacity = 0.7f * MathF.Pow(Projectile.Opacity * Projectile.scale, 0.5f);

            ManagedShader innerShader = ShaderManager.GetShader("FargowiltasSouls.CultistIceArcShader");
            innerShader.TrySetParameter("colorMult", 7.35f);
            innerShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            innerShader.TrySetParameter("radius", innerRadius);
            innerShader.TrySetParameter("anchorPoint", auraPos);
            innerShader.TrySetParameter("screenPosition", Main.screenPosition);
            innerShader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            innerShader.TrySetParameter("nearbyPosition", nearbyPosition);
            innerShader.TrySetParameter("arcWidth", ConeWidth * 2);
            innerShader.TrySetParameter("maxOpacity", maxOpacity);
            innerShader.TrySetParameter("darkColor", darkColor.ToVector4());
            innerShader.TrySetParameter("midColor", mediumColor.ToVector4());


            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, innerShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}