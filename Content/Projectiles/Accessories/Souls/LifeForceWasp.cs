using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class LifeForceWasp : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.GiantBee);
        public int Identity => FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, (int)Projectile.ai[0], ModContent.ProjectileType<BeeFlower>());
        public bool FlowerDied = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.GiantBee];
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = ProjAIStyleID.SmallFlying;
            Projectile.extraUpdates = 1;
        }
        public override bool PreAI()
        {
            Projectile.rotation = Projectile.velocity.X * 0.1f;
            if (Projectile.velocity.X > 0f) Projectile.spriteDirection = 1;
            else if (Projectile.velocity.X < 0f) Projectile.spriteDirection = -1;
            if (Projectile.frameCounter++ >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type]) Projectile.frame = 0;

            if (Identity != -1 && !FlowerDied) // flower active
            {
                if (Projectile.Center.Distance(Main.projectile[Identity].Center) > 48)
                    Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, Main.projectile[Identity].Center, Projectile.velocity, 0.3f, 0.3f);
                Projectile.timeLeft++;
                return false;
            }
            else // flower has been killed, disperse
            {
                FlowerDied = true;
                return true;
            }
        }
        public override void AI()
        {
            if (Identity == -1) FlowerDied = true; //for every proj that uses this as a base

            if (FlowerDied && Projectile.aiStyle == ProjAIStyleID.SmallFlying)
                Projectile.extraUpdates = 1; //constantly set since vanilla aistyle constantly sets to 0
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FlowerDied) Projectile.ai[2]++;
            if (Projectile.ai[2] >= 3) Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                int b = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Bee, Projectile.velocity.X, Projectile.velocity.Y, 50);
                Main.dust[b].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (ProjectileID.Sets.TrailingMode[Projectile.type] != -1)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = color * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
                }
            }

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, color, Projectile.rotation, origin2, Projectile.scale, effects);
            return false;
        }
    }
}