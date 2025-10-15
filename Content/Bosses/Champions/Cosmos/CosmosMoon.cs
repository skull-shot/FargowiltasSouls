using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosMoon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Moon");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 410;
            Projectile.height = 410;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.netImportant = true;

            Projectile.extraUpdates = 0;
            CooldownSlot = ImmunityCooldownID.Bosses;

            Projectile.FargoSouls().TimeFreezeImmune = true;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.penetrate = -1;

            //Projectile.scale = 0.5f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= Projectile.width / 2;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

                Projectile.rotation = Projectile.ai[0];
            }

            if (Projectile.localAI[0] == 2)
            {
                Projectile.damage = 0;
                Projectile.velocity.Y += 0.2f;
                Projectile.tileCollide = true;
            }
            else
            {
                NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<CosmosChampion>());
                if (npc != null)
                {
                    Projectile.timeLeft = 600;

                    const float maxAmplitude = 850;
                    float offset = Math.Abs(maxAmplitude * (float)Math.Sin(npc.ai[2] * 2 * (float)Math.PI / 200));
                    offset += 150;
                    Projectile.ai[0] += 0.01f;
                    Projectile.Center = npc.Center + offset * Projectile.ai[0].ToRotationVector2();
                }
                else
                {
                    Projectile.localAI[0] = 2;
                    Projectile.velocity = Projectile.position - Projectile.oldPos[1];
                }
            }

            Projectile.rotation += 0.04f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = (int)(Projectile.width / Math.Sqrt(2));
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnKill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);

            if (!Main.dedServ && Main.LocalPlayer.active)
                ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 30);

            if (FargoSoulsUtil.HostCheck)
            {
                const int max = 8;
                const float baseRotation = MathHelper.TwoPi / max;
                for (int i = 0; i < max; i++)
                {
                    float rotation = baseRotation * (i + Main.rand.NextFloat(-0.5f, 0.5f));
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordMoonBlast>(), 0, Projectile.knockBack, Projectile.owner, rotation, 3);
                }

                if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore))
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordMoonBlast>(), 0, Projectile.knockBack, Projectile.owner, -Vector2.UnitY.ToRotation(), 32);
            }

            Vector2 size = new(500, 500);
            Vector2 spawnPos = Projectile.Center;
            spawnPos.X -= size.X / 2;
            spawnPos.Y -= size.Y / 2;

            for (int num615 = 0; num615 < 30; num615++)
            {
                int num616 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                Main.dust[num616].velocity *= 1.4f;
            }

            for (int num617 = 0; num617 < 20; num617++)
            {
                int num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[num618].noGravity = true;
                Main.dust[num618].velocity *= 7f;
                num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[num618].velocity *= 3f;
            }

            for (int num619 = 0; num619 < 2; num619++)
            {
                float scaleFactor9 = 0.4f;
                if (num619 == 1) scaleFactor9 = 0.8f;
                int num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore97 = Main.gore[num620];
                gore97.velocity.X++;
                Gore gore98 = Main.gore[num620];
                gore98.velocity.Y++;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore99 = Main.gore[num620];
                gore99.velocity.X--;
                Gore gore100 = Main.gore[num620];
                gore100.velocity.Y++;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore101 = Main.gore[num620];
                gore101.velocity.X++;
                Gore gore102 = Main.gore[num620];
                gore102.velocity.Y--;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore103 = Main.gore[num620];
                gore103.velocity.X--;
                Gore gore104 = Main.gore[num620];
                gore104.velocity.Y--;
            }


            for (int k = 0; k < 20; k++) //make visual dust
            {
                Vector2 dustPos = spawnPos;
                dustPos.X += Main.rand.Next((int)size.X);
                dustPos.Y += Main.rand.Next((int)size.Y);

                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(dustPos, 32, 32, DustID.Smoke, 0f, 0f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(dustPos, 32, 32, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(dustPos, 32, 32, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                float scaleFactor9 = 0.5f;
                for (int j = 0; j < 2; j++)
                {
                    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), dustPos, default, Main.rand.Next(61, 64));
                    Main.gore[gore].velocity *= scaleFactor9;
                    Main.gore[gore].velocity.X += 1f;
                    Main.gore[gore].velocity.Y += 1f;
                }
            }


            const int num226 = 30;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 40f;
                vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * 6.28318548f / num226, default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Torch, 0f, 0f, 0, default, 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 2f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(BuffID.BrokenArmor, 300);
                target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 300);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color glow = new Color(Main.DiscoR + 210, Main.DiscoG + 210, Main.DiscoB + 210) * Projectile.Opacity;
            Color glow2 = new Color(Main.DiscoR + 50, Main.DiscoG + 50, Main.DiscoB + 50) * Projectile.Opacity;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.ResetToDefault();
            return false;
        }
    }
}