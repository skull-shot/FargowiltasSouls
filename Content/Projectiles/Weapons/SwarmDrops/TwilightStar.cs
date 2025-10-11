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
    public class TwilightStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_79";

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

        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0.9f;
            Projectile.light = 0.6f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.ghost)
                Projectile.Kill();

            timer++;
            Frame();
            switch (state)
            {
                case 0:
                    if (!player.channel)
                        Projectile.Kill();
                    if (timer > 20)
                    {
                        SoundEngine.PlaySound(SoundID.Item29 with { Pitch = -1f, Volume = 0.5f }, Projectile.Center);
                        state = 1;
                    }
                    break;
                case 1:
                    if (!player.channel && player.HeldItem.type == ModContent.ItemType<TwilightTome>() && !player.controlUseItem)
                    {
                        SoundEngine.PlaySound(SoundID.Item43 with { Pitch = -0.5f }, Projectile.Center);
                        timer = 0;
                        state = 2;
                        return;
                    }
                    else
                    {
                        if (timer % 30 == 0) // occasional twinkle
                            for (int i = 0; i < 3; i++)
                            {
                                float randRot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                                new SparkParticle(Projectile.Center + 5 * Vector2.UnitX.RotatedBy(randRot), 2 * Vector2.UnitX.RotatedBy(randRot), Color.SkyBlue, 0.2f, 6).Spawn();
                                new SmallSparkle(Projectile.Center + 5 * Vector2.UnitX.RotatedBy(randRot), 2 * Vector2.UnitX.RotatedBy(randRot), Color.SkyBlue, 1f, 7).Spawn();
                            }
                    }
                    break;
                case 2:
                    if (timer > 3) // let unformed stars despawn
                    {
                        // scale projectile count with star number
                        projCount = 2 * player.ownedProjectileCounts[Projectile.type];

                        timer = 0;
                        state = 3;
                    }
                    break;
                case 3:
                    if (timer % 15 == 14 && projCount > 0)
                    {
                        float rot = (Main.MouseWorld - Projectile.Center).ToRotation();
                        Vector2 vel = new Vector2(10f, 0f).RotatedBy(rot);
                        FargoSoulsUtil.DustRing(Projectile.Center, 20, DustID.HallowSpray, 3f, scale: 1.5f);
                        SoundEngine.PlaySound(SoundID.Item75 with { Pitch = -0.5f }, Projectile.Center);

                        // Random spread
                        float spread = MathHelper.Pi / 24;
                        float randRot = Main.rand.NextFloat(-spread, spread);

                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel.RotatedBy(randRot),
                                ModContent.ProjectileType<TwilightStarSpawn>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                        projCount--;
                        if (projCount <= 0)
                        {
                            state = 4;
                        }
                        Projectile.netUpdate = true;
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
            
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opac = Projectile.Opacity;
            if (state == 0)
                opac = timer / 20;

            Texture2D text = TextureAssets.Projectile[16].Value;
            Rectangle frame = text.Frame();

            float count = 10;
            for (int i = 0; i < count; i++)
            {
                float scale = (1 - (i / count));
                Color c = Color.SkyBlue * opac * (i/count);
                Main.EntitySpriteDraw(text, Projectile.position - Main.screenPosition + frame.Size() / 2, frame, c, 0, frame.Size() / 2, Projectile.scale * scale, SpriteEffects.None);
            }
            //FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor * opac, rotation: 0);
            return false;
        }
    }
}
