﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Bosses.WallOfFlesh;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Deathrays
{
    public class ChainDeathrayWOFS : BaseDeathray
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Deathrays", "PhantasmalDeathrayWOF");
        public ChainDeathrayWOFS() : base(90, 0.5f) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Divine Deathray");
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            /*if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == NPCID.WallofFleshEye)
            {
                //Vector2 value21 = new Vector2(27f, 59f);
                //Vector2 fireFrom = new Vector2(Main.npc[(int)Projectile.ai[1]].Center.X, Main.npc[(int)Projectile.ai[1]].Center.Y);
                //Vector2 value22 = Utils.Vector2FromElipse(Main.npc[(int)Projectile.ai[1]].localAI[2].ToRotationVector2(), value21 * Main.npc[(int)Projectile.ai[1]].localAI[3]);
                //Projectile.position = fireFrom + value22 - new Vector2(Projectile.width, Projectile.height) / 2f;
                Vector2 offset;
                if (Projectile.ai[0] == 0f)
                    offset = new Vector2(Main.npc[(int)Projectile.ai[1]].width - 36, 6).RotatedBy(Main.npc[(int)Projectile.ai[1]].rotation + Math.PI);
                else
                    offset = new Vector2(Main.npc[(int)Projectile.ai[1]].width - 36, -6).RotatedBy(Main.npc[(int)Projectile.ai[1]].rotation);
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + offset;
            }
            else
            {
                Projectile.Kill();
                return;
            }*/
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            /*if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
            }*/
            float num801 = 0.2f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 30 && FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity * Projectile.ai[0],
                    ModContent.ProjectileType<WOFChain>(), Projectile.damage, 0f, Main.myPlayer);
            }
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 0.6f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            float num804 = Projectile.velocity.ToRotation();// Main.npc[(int)Projectile.ai[1]].rotation + 1.57079637f;
                                                            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            Projectile.rotation = num804 - MathHelper.PiOver2;
            //num804 += 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, DustID.CopperCoin, vector80.X, vector80.Y, 0, default, 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.CopperCoin, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            Projectile.position -= Projectile.velocity;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Confused, 300);
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }
}