using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantTrueEyeS : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantTrueEye_April" :
            "Terraria/Images/Projectile_650";

        private float localAI0;
        private float localAI1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 42;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            Projectile.localAI[0]++;
            switch ((int)Projectile.ai[1])
            {
                case 0: //true eye movement code
                    Vector2 newVel = target.Center - Projectile.Center + new Vector2(-200f * Projectile.localAI[1], -200f);
                    if (newVel != Vector2.Zero)
                    {
                        newVel.Normalize();
                        newVel *= 24f;
                        Projectile.velocity.X = (Projectile.velocity.X * 29 + newVel.X) / 30;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 29 + newVel.Y) / 30;
                    }
                    if (Projectile.Distance(target.Center) < 150f)
                    {
                        if (Projectile.Center.X < target.Center.X)
                            Projectile.velocity.X -= 0.25f;
                        else
                            Projectile.velocity.X += 0.25f;

                        if (Projectile.Center.Y < target.Center.Y)
                            Projectile.velocity.Y -= 0.25f;
                        else
                            Projectile.velocity.Y += 0.25f;
                    }

                    if (Projectile.localAI[0] > 60f)
                    {
                        Projectile.localAI[0] = 0f;
                        Projectile.ai[1]++;
                        Projectile.netUpdate = true;
                    }
                    break;

                case 1: //slow down
                    Projectile.velocity *= 0.9f;
                    if (Projectile.velocity.Length() < 1f) //stop, FIRE LASER
                    {
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localAI[0] = 0f;
                        Projectile.ai[1]++;
                        Projectile.netUpdate = true;
                    }
                    break;

                case 2: //shoot
                    if (Projectile.localAI[0] == 7f)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
                        ShootBolts(target);
                    }
                    else if (Projectile.localAI[0] == 14f)
                    {
                        ShootBolts(target);
                    }
                    else if (Projectile.localAI[0] > 21f)
                    {
                        Projectile.localAI[0] = 0f;
                        Projectile.ai[1]++;
                    }
                    break;

                default:
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 0, default, 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 8f;
                    }
                    SoundEngine.PlaySound(SoundID.Zombie102, Projectile.Center);
                    Projectile.Kill();
                    break;
            }

            if (Projectile.rotation > 3.14159274101257)
                Projectile.rotation = Projectile.rotation - 6.283185f;
            Projectile.rotation = Projectile.rotation <= -0.005 || Projectile.rotation >= 0.005 ? Projectile.rotation * 0.96f : 0.0f;
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            if (Projectile.ai[1] != 2f) //custom pupil when attacking
                UpdatePupil();
        }

        private void ShootBolts(Player target)
        {
            Vector2 spawn = Projectile.Center - Vector2.UnitY * 6f;
            Vector2 vel = target.Center + target.velocity * 15f - spawn;
            if (vel != Vector2.Zero)
            {
                vel.Normalize();
                vel *= 8f;
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), spawn, vel, ProjectileID.PhantasmalBolt, Projectile.damage, 0f, Projectile.owner);
            }
        }

        private void UpdatePupil()
        {
            float f1 = (float)(localAI0 % 6.28318548202515 - 3.14159274101257);
            float num13 = (float)Math.IEEERemainder(localAI1, 1.0);
            if (num13 < 0.0)
                ++num13;
            float num14 = (float)Math.Floor(localAI1);
            float max = 0.999f;
            int num15 = 0;
            float amount = 0.1f;
            float f2;
            float num18;
            float num19;
            f2 = Projectile.AngleTo(Main.player[(int)Projectile.ai[0]].Center);
            num15 = 2;
            num18 = MathHelper.Clamp(num13 + 0.05f, 0.0f, max);
            num19 = num14 + Math.Sign(-12f - num14);
            Vector2 rotationVector2 = f2.ToRotationVector2();
            localAI0 = (float)(Vector2.Lerp(f1.ToRotationVector2(), rotationVector2, amount).ToRotation() + num15 * 6.28318548202515 + 3.14159274101257);
            localAI1 = num19 + num18;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
        }

        public override bool? CanCutTiles()
        {
            return false;
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

            Color color26 = Projectile.GetAlpha(lightColor);

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 1f;
            scale *= Projectile.scale;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27.A = 0;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Texture2D pupil = FargoAssets.GetTexture2D("Content/Projectiles/Weapons/Minions", "TrueEyePupil").Value;
            Vector2 pupilOffset = new Vector2(localAI1 / 2f, 0f).RotatedBy(localAI0);
            pupilOffset += new Vector2(0f, -6f).RotatedBy(Projectile.rotation);
            Vector2 pupilOrigin = pupil.Size() / 2f;
            Main.EntitySpriteDraw(pupil, pupilOffset + Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(pupil.Bounds), color26, 0f, pupilOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}