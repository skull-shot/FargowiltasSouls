using System;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Solar
{
    public class SolarMeteor : CosmosMeteor
    {
        bool spawned = false;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 48;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Cosmos/CosmosMeteor";
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Projectile.CloneDefaults(ProjectileID.Meteor1);
            //Projectile.width = 36;
            //Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            CooldownSlot = ImmunityCooldownID.General;
        }

        public override void AI()
        {
            base.AI();

        }
        public override void PostAI()
        {
            base.PostAI();
            Projectile.velocity = (Vector2.UnitX * Projectile.ai[1]) + (Vector2.UnitY * Projectile.ai[2]);
            Projectile.rotation += (float)Math.PI / 15;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }
            else
            {
                Projectile.alpha = 0;
            }
        }

        public override void OnKill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            base.OnKill(timeLeft);
            /*SoundEngine.PlaySound(SoundID.Item89, Projectile.position);

            Projectile.position = Projectile.Center;
            Projectile.width = (int)(64 * (double)Projectile.scale);
            Projectile.height = (int)(64 * (double)Projectile.scale);
            Projectile.Center = Projectile.position;

            for (int index = 0; index < 4; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int index1 = 0; index1 < 16; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
                Main.dust[index3].noGravity = true;
            }
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[index2];
                gore.velocity *= 0.3f;
                Main.gore[index2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[index2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
            }

            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Utils.SelectRandom(Main.rand, new int[3] { 6, 259, 158 }), 2.5f * Projectile.direction, -2.5f, 0, new Color(), 1f);
                Main.dust[index2].alpha = 200;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 2.4f;
                Dust dust2 = Main.dust[index2];
                dust2.scale += Main.rand.NextFloat();
            }*/
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.BrokenArmor, 300);
            target.AddBuff(BuffID.OnFire, 300);
            Projectile.timeLeft = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return base.GetAlpha(lightColor);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}