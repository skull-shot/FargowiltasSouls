using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
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
    public class CosmosMeteor : ModProjectile, IPixelatedPrimitiveRenderer
    {
        private bool spawned;
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/Souls", "MeteorEnchantMeatball");
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Meteor");
            //Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Meteor1);
            AIType = 0; // ProjectileID.Meteor1;

            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = true;

            CooldownSlot = ImmunityCooldownID.Bosses;

            Projectile.timeLeft = 120 * Projectile.MaxUpdates;
            Projectile.tileCollide = false;

            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.scale = (24f / 80);
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                //Projectile.frame = Main.rand.Next(3);
            }
            Projectile.tileCollide = false;

            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = Main.rand.NextBool() ? 1 : -1;

            Projectile.rotation += Projectile.localAI[1] * MathHelper.TwoPi / 24f;

            /*
            if (++Projectile.frameCounter > 4)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            */

            if (++Projectile.localAI[0] % 2 == 1)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y);
                Main.dust[d].noGravity = true;

                int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y);
                Main.dust[d2].noGravity = true;
            }

            if (Main.rand.NextBool(10))
            {
                float scaleFactor9 = 0.4f;
                int i = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + Main.rand.NextVector2FromRectangle(new(0, 0, Projectile.width, Projectile.height)) * 0.75f, default, Main.rand.Next(61, 64));
                Gore gore = Main.gore[i];
                gore.position -= Projectile.velocity * 4;
                gore.velocity *= scaleFactor9;
                gore.velocity = -Projectile.velocity * Main.rand.NextFloat(0.5f, 0.9f);
            }
        }

        public override void OnKill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);

            Projectile.position = Projectile.Center;
            Projectile.width = (int)(64 * (double)Projectile.scale);
            Projectile.height = (int)(64 * (double)Projectile.scale);
            Projectile.Center = Projectile.position;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (FargoSoulsUtil.HostCheck)
            {
                int i = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<MeteorExplosion>(), 0, 0, Projectile.owner);
                float scale = Projectile.scale / 2f;
                if (i.IsWithinBounds(Main.maxProjectiles))
                {
                    Projectile p = Main.projectile[i];
                    p.position = p.Center;
                    p.scale *= scale;
                    p.width = (int)(p.width * scale);
                    p.height = (int)(p.height * scale);
                    p.Center = p.position;
                }
            }

            if (!Main.dedServ)
            {
                int gores = 5;
                for (int j = 0; j < gores; j++)
                {
                    int i = j % 5;
                    Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                    Vector2 vel = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(4, 8);
                    int type = i + 1;
                    if (!Main.dedServ)
                        Gore.NewGore(Projectile.GetSource_FromThis(), pos, vel, ModContent.Find<ModGore>(Mod.Name, $"MeteorGore{type}").Type, Projectile.scale);
                }
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<CosmosChampion>()))
            {
                modifiers.ScalingArmorPenetration += 0.25f;
            }
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
            return Color.White * Projectile.Opacity;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 5f;
            float ratio = MathF.Pow(completionRatio, 1.5f);
            return MathHelper.SmoothStep(baseWidth, 25f * Projectile.scale, ratio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(MeteorEnchantMeatball.OrangeColor, Color.SkyBlue, completionRatio);
            float opacity = 0.7f;
            return color * opacity;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargoAssets.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            MeteorEnchantMeatball.DrawMeteor(Projectile, Texture, false, ref lightColor);
            return false;
        }
    }
}