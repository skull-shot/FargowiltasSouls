using System.Threading;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class BeetleEnchantBeetle : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Life/ChampionBeetle";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public static int HitCooldown => 45;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;

            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.noEnchantmentVisuals = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = HitCooldown;

            Projectile.ContinuouslyUpdateDamageStats = true;

        }
        public ref float Target => ref Projectile.ai[0];
        public ref float Cooldown => ref Projectile.ai[1];
        private float IdleTimer;
        private Vector2 IdleLocation;
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            //Main.NewText($"Beetles: {player.FargoSouls().Beetles} BeetleCharge: {player.FargoSouls().BeetleCharge}");
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers) || !Main.player[Projectile.owner].Alive())
            {
                Projectile.Kill();
                return;
            }
            if (player.HasEffect<BeetleEffect>())
                Projectile.timeLeft = 2;
            if (Projectile.Distance(player.Center) > 3000)
                Projectile.Center = player.Center;

            if (Projectile.velocity.X > 0)
                Projectile.spriteDirection = 1;
            else if (Projectile.velocity.X < 0)
                Projectile.spriteDirection = -1;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            if (Cooldown == 0)
            {
                Target = -2;
                Projectile.netUpdate = true;
            }
            if (Cooldown >= 0)
                Cooldown--;
            if (Cooldown <= -1) // seeking out enemy
            {
                if (Target < 0) // has no target
                {
                    if (player.FargoSouls().BeetleAttackCD <= 0)
                    {
                        NPC npc = Projectile.FindTargetWithinRange(800, true);
                        if (npc != null && npc.Alive())
                        {
                            Target = npc.whoAmI;
                            Projectile.netUpdate = true;
                            player.FargoSouls().BeetleAttackCD = (int)((float)HitCooldown / player.ownedProjectileCounts[Type]);
                        }
                    }
                }
                else // has target, seek it out
                {
                    NPC npc = Main.npc[(int)Target];
                    if (npc != null && npc.Alive())
                    {
                        HomingMovement(npc.Center);
                        if (Projectile.Colliding(Projectile.Hitbox, npc.Hitbox))
                        {
                            Target = -1;
                            Cooldown = HitCooldown;
                            Projectile.netUpdate = true;
                        }
                    }
                    else
                        Target = -2;
                }
            }
            if (Cooldown > 0 || Target < 0)
            {
                if (Target == -2) // idle movement
                {
                    if (Projectile.Distance(player.Center) < 80)
                        Projectile.spriteDirection = player.direction;
                    if (IdleTimer > 0)
                        IdleTimer--;
                    if (IdleTimer == 0)
                    {
                        IdleLocation = player.Center + Main.rand.NextVector2Circular(48, 32);
                        IdleTimer = Main.rand.Next(20, 40);
                    }
                    if (Projectile.Distance(IdleLocation) > 16)
                        Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, IdleLocation, Projectile.velocity, Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(0.5f, 1f));
                }
                if (Target == -1) // returning to player after attacking
                {
                    HomingMovement(Main.rand.NextVector2FromRectangle(player.Hitbox));
                    if (Cooldown > 0 && Cooldown < 3 && Projectile.Distance(player.Center) > 80)
                        Cooldown = 2;
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Cooldown < 0 && Target >= 0)
                return base.CanHitNPC(target);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath47 with {MaxInstances = 1, Volume = 0.5f}, Projectile.Center);
            for (int k = 0; k < 20; k++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ambient_DarkBrown, 0, -1f);
                Main.dust[d].scale += 0.5f;
                Main.dust[d].noGravity = true;
            }
        }
        public void HomingMovement(Vector2 destination)
        {
            float speedMod = 1.25f;
            float accel = 1f * speedMod;
            float decel = 1.5f * speedMod;
            float resistance = Projectile.velocity.Length() * accel / (35f * speedMod);
            Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, destination, Projectile.velocity, accel - resistance, decel + resistance);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
