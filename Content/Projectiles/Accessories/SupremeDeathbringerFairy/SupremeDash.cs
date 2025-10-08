using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.SupremeDeathbringerFairy
{
    public class SupremeDash : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/SupremeDeathbringerFairy", Name);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Player.defaultHeight;
            Projectile.height = Player.defaultHeight;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 15;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().TimeFreezeImmune = true;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            if (player.HasBuff(ModContent.BuffType<Buffs.Souls.TimeFrozenBuff>()))
            {
                Projectile.Kill();
                return;
            }

            player.dashDelay = 5;
            player.FargoSouls().IsDashingTimer = 0;

            player.Center = Projectile.Center - Projectile.velocity;
            if (Projectile.timeLeft > 1) //trying to avoid wallclipping
                player.position += Projectile.velocity;
            player.velocity = Projectile.velocity * .5f;
            if (Projectile.velocity.X != 0)
                player.direction = Projectile.velocity.X > 0 ? 1 : -1;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            //player.controlMount = false;

            //if (player.mount.Active)
                //player.mount.Dismount(player);

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
                /*for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Bone, 0, 0, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }*/
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Main.myPlayer];
            Projectile.Kill();
            if (Projectile.numHits < 1)
            {
                player.immune = true;
                player.immuneNoBlink = true;
                player.immuneTime += 4;
                player.velocity.X = -player.velocity.X;
                player.velocity.Y = -player.velocity.Y * 1.2f; //higher to counteract gravity slightly
                SoundEngine.PlaySound(SoundID.Item175, Projectile.Center);
                if (player.HasEffect<AgitatingLensInstall>())
                    player.AddBuff(ModContent.BuffType<BerserkerInstallBuff>(), 120);
            }
            /*if (target.knockBackResist == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    int p = Projectile.NewProjectile(player.GetSource_EffectItem<SupremeDashEffect>(), player.Center, 16f * Vector2.Normalize(Projectile.velocity.RotatedBy((0.3f * i) -0.15f)), ProjectileID.SharpTears, Projectile.damage / 2, 12f, player.whoAmI, 0f, 1f);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].DamageType = DamageClass.Melee;
                        Main.projectile[p].usesLocalNPCImmunity = false;
                        Main.projectile[p].usesIDStaticNPCImmunity = true;
                        Main.projectile[p].idStaticNPCHitCooldown = 10;
                        Main.projectile[p].FargoSouls().noInteractionWithNPCImmunityFrames = true;
                        Main.projectile[p].FargoSouls().DeletionImmuneRank = 1;
                    }
                }
            }*/
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 3;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Main.player[Projectile.owner].Center, Projectile.width, Projectile.height, DustID.Bone, 0, 0, 0, default, 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
                Main.dust[d].velocity += Projectile.velocity;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //dont kill proj when hits tiles
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] != 0)
            {
                Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
                int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;

                Color color26 = Projectile.GetAlpha(lightColor);

                SpriteEffects effects = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                float rotationOffset = Projectile.direction < 0 ? MathHelper.Pi : 0;
                float scale = Projectile.scale * 1f;
                Vector2 posOffset = Vector2.Zero;
                if (Projectile.velocity != Vector2.Zero)
                    posOffset = 16f * Projectile.velocity.SafeNormalize(Vector2.Zero);

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = color26 * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i] + rotationOffset;
                    Main.EntitySpriteDraw(texture2D13, posOffset + value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
                }

                Main.EntitySpriteDraw(texture2D13, posOffset + Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation + rotationOffset, origin2, scale, effects, 0);
            }
            return false;
        }
    }
}