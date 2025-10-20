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
        public ref float state => ref Projectile.ai[1];
        public ref float ai0 => ref Projectile.ai[0];
        public ref float timer => ref Projectile.ai[2];
        #endregion

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.Opacity = 0.9f;
            //Projectile.light = 0.6f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.ghost)
                Projectile.Kill();

            timer++;

            Color trailColor = Color.Lerp(Color.SkyBlue, Color.Blue, 0.6f);
            float velLen = Projectile.velocity.Length() / 5f;

            switch (state)
            {
                case 0: // spawning
                    if (!player.channel)
                        Projectile.Kill();
                    if (timer > 20)
                    {
                        SoundEngine.PlaySound(SoundID.Item29 with { Pitch = -1f, Volume = 0.5f }, Projectile.Center);
                        state = 1;
                    }
                    break;
                case 1: // following mouse
                    if (!player.channel && player.HeldItem.type == ModContent.ItemType<TwilightTome>() && !player.controlUseItem)
                    {
                        SoundEngine.PlaySound(SoundID.Item43 with { Pitch = -0.5f }, Projectile.Center);
                        timer = 0;
                        state = 2;
                        return;
                    }
                    else
                    {
                        Movement(Main.MouseWorld);
                        Projectile.netUpdate = true;
                    }
                    break;
                case 2: // prep to fire
                    if (timer > 3) // let unformed stars despawn
                    {
                        // scale projectile count with star number
                        ai0 = 2 * Main.projectile.Where(p => p.active && p.type == Type && p.ai[1] < 4).Count();

                        timer = 0;
                        state = 3;
                    }
                    break;
                case 3: // firing
                    Projectile.velocity *= 0.9f;
                    if (timer % 15 == 14)
                    {
                        if (ai0 > 0)
                        {
                            float rot = (Main.MouseWorld - Projectile.Center).ToRotation();
                            Vector2 vel = new Vector2(25f, 0f).RotatedBy(rot);
                            float sparkRot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                            for (int i = 0; i < 5; i++)
                            {
                                new SmallSparkle(Projectile.Center, 2 * Vector2.UnitX.RotatedBy(sparkRot + (MathHelper.TwoPi * (i / 5f))), trailColor, 1f, 14).Spawn();
                            }

                            SoundEngine.PlaySound(SoundID.Item75 with { Pitch = -0.5f }, Projectile.Center);

                            if (FargoSoulsUtil.HostCheck)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel,
                                    ModContent.ProjectileType<TwilightStarSpawn>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                            Projectile.netUpdate = true;
                        }
                        else if (ai0 < 0)
                        {
                            FargoSoulsUtil.DustRing(Projectile.Center, 20, DustID.HallowSpray, 3f);
                            SoundEngine.PlaySound(SoundID.Item29, Projectile.Center);
                            ai0 = -1;
                            timer = 0;
                            state = 4;
                        }

                        ai0--;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 4: // homing charge
                    if (ai0 < 0) // no target
                    {
                        ai0 = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2000f);
                    }
                    else
                    {
                        Projectile.extraUpdates = 1;
                        NPC target = Main.npc[(int)ai0];
                        if (target.active)
                            Movement(target.Center);
                    }
                    break;
            }

            if (timer % 22 == 0)
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = 0.5f }, Projectile.Center);
            Lighting.AddLight(Projectile.Center, new Vector3(0.8f, 0.8f, 1f));
        }

        public void Movement(Vector2 targetPos)
        {
            bool enemy = state == 4;

            Color tColor = Color.Lerp(Color.SkyBlue, Color.Blue, 0.6f);
            float velLen = Projectile.velocity.Length();

            Vector2 posToIdle = targetPos - Projectile.Center;
            float dist = posToIdle.Length();
            Projectile.velocity += 0.25f * Vector2.UnitX.RotatedBy(posToIdle.ToRotation());

            if (Math.Abs((posToIdle.ToRotation() - Projectile.velocity.ToRotation()) / MathHelper.TwoPi) > 0.5f)
            {
                Projectile.velocity *= enemy ? 0.90f : 0.98f;
            }

            float speedCap = enemy ? 8f : 12f;
            if (velLen > speedCap)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= speedCap;
            }

            new AlphaBloomParticle(Projectile.Center, Projectile.velocity * 0.5f, tColor, Vector2.Zero, (velLen / 5f) * Vector2.One, 25).Spawn();

            float spread = 0.5f;
            if (timer % 10 == 0)
            {
                float randRot = Projectile.velocity.ToRotation() + Main.rand.NextFloat(-spread, spread);
                new SmallSparkle(Projectile.Center, (velLen / 5f) * Vector2.UnitX.RotatedBy(randRot), tColor, (velLen / 5f) * 0.35f, 24).Spawn();
            }

            Projectile.netUpdate = true;
        }

        public override bool? CanHitNPC(NPC target) => state >= 4;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FargoSoulsUtil.DustRing(target.Center, 20, DustID.HallowSpray, 3f);

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opac = Projectile.Opacity;
            if (state == 0)
                opac = Projectile.Opacity * timer / 20;

            Texture2D text = TextureAssets.Projectile[16].Value;
            Rectangle frame = text.Frame();

            float count = 10;
            for (int i = 0; i < count; i++)
            {
                float scale = (1 - (i / count));
                Color c = Color.SkyBlue * opac * (i/count);
                Main.EntitySpriteDraw(text, Projectile.Center - Main.screenPosition, frame, c, 0, frame.Size() / 2, Projectile.scale * scale, SpriteEffects.None);
            }
            //FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor * opac, rotation: 0);
            return false;
        }
    }
}
