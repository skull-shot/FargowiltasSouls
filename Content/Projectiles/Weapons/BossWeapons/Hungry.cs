using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons
{
    public class Hungry : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/BossWeapons", Name);

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hungry");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 240;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            AIType = ProjectileID.Bullet;
        }

        /*public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(modifier);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            modifier = reader.ReadSingle();
        }*/

        bool spawned;

        public override bool PreAI()
        {
            if (!spawned)
            {
                spawned = true;
                if (Projectile.ai[2] == 1)
                {
                    Projectile.usesIDStaticNPCImmunity = false;
                    Projectile.idStaticNPCHitCooldown = 0;
                    Projectile.penetrate = 1;
                    Projectile.maxPenetrate = 1;
                }
            }

            return base.PreAI();
        }

        public override void AI()
        {
            /*if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                float minionSlotsUsed = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.Projectile[i].active && !Main.Projectile[i].hostile && Main.Projectile[i].owner == Projectile.owner && Main.Projectile[i].minion)
                        minionSlotsUsed += Main.Projectile[i].minionSlots;
                }

                modifier = Main.player[Projectile.owner].maxMinions - minionSlotsUsed;
                if (modifier < 0)
                    modifier = 0;
                if (modifier > 3)
                    modifier = 3;

                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.netUpdate = true;
                }
            }*/

            if (++Projectile.frameCounter > 4)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }

            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, DustID.RedTorch, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default, 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, DustID.RedTorch, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default, 2f);
            Main.dust[dustId3].noGravity = true;

            const int aislotHomingCooldown = 1;
            int homingDelay = Projectile.ai[2] == 0 ? 15 : 30;
            const float desiredFlySpeedInPixelsPerFrame = 60;

            float amountOfFramesToLerpBy = Projectile.ai[2] == 0 ? 40 : 30; // minimum of 1, please keep in full numbers even though it's a float!

            Projectile.ai[aislotHomingCooldown]++;
            if (Projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                Projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                float homingRange = Projectile.ai[2] == 0 ? 600 : 1200;
                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, homingRange, true));
                if (n.Alive())
                {
                    Vector2 desiredVelocity = Projectile.SafeDirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            return --Projectile.penetrate <= 0;
        }

        public override void OnKill(int timeleft)
        {
            for (int num468 = 0; num468 < 20; num468++)
            {
                int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.RedTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 2f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 2f;
                num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.RedTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[num469].velocity *= 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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
