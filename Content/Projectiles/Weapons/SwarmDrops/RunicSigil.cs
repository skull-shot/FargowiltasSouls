using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops
{
    public class RunicSigil : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_240";

        #region References
        /*
         * 0 = initial deployment
         * 1 = wind up
         * 2 = firing
         * 3 = despawning
         */
        public ref float state => ref Projectile.ai[0];
        public ref float projCount => ref Projectile.ai[1];
        public ref float timer => ref Projectile.ai[2];
        #endregion

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 90;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0.9f;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.ghost)
                Projectile.Kill();
            float rot = (Main.MouseWorld - Projectile.Center).ToRotation();
            Projectile.rotation = rot;

            timer++;
            Frame();
            switch (state)
            {
                case 0:
                    if (!player.channel)
                        Projectile.Kill();
                    if (timer > 45)
                    {
                        SoundEngine.PlaySound(SoundID.Item103, Projectile.Center);
                        state = 1;
                    }
                    break;
                case 1:
                    if (!player.channel && player.HeldItem.type == ModContent.ItemType<RunicBook>() && !player.controlUseItem)
                    {
                        SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                        timer = 0;
                        state = 2;
                        return;
                    }
                    else
                    {
                        float randRot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        new SparkParticle(Projectile.Center + 5 * Vector2.UnitY + 20 * Vector2.UnitX.RotatedBy(randRot), -4 * Vector2.UnitX.RotatedBy(randRot), Color.Purple, 0.2f, 3).Spawn();
                    }
                    break;
                case 2:
                    if (timer > 3) // let unformed portals despawn
                    {
                        projCount = 2 * player.ownedProjectileCounts[Projectile.type];
                        timer = 0;
                        state = 3;
                    }
                    break;
                case 3:
                    if (timer % 15 == 14 && projCount > 0)
                    {
                        Vector2 vel = new Vector2(10f, 0f).RotatedBy(rot);
                        FargoSoulsUtil.DustRing(Projectile.Center, 20, DustID.PinkTorch, 3f, scale: 1.5f);
                        SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);

                        // Random spread
                        float spread = MathHelper.Pi / 24;
                        float randRot = Main.rand.NextFloat(-spread, spread);

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel.RotatedBy(randRot),
                            ModContent.ProjectileType<RunicBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                        projCount--;
                        if (projCount <= 0)
                        {
                            state = 4;
                        }
                    }
                    break;
                case 4:
                    Projectile.timeLeft = Math.Min(Projectile.timeLeft, 20);
                    Projectile.Opacity -= 1 / 40f;
                    if (Projectile.Opacity <= 0)
                        Projectile.Kill();
                    break;
            }

            if (timer % 22 == 0)
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = 0.4f }, Projectile.Center);
            Lighting.AddLight(Projectile.Center, TorchID.Shimmer);
        }

        void Frame()
        {
            if (timer % 4 == 0)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opac = 1;
            if (state == 0)
                opac = timer / 60;

            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor * opac, rotation: 0);
            return false;
        }
    }
}
