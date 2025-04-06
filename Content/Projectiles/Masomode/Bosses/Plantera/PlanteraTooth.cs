using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode.Bosses.Plantera
{
    public class PlanteraTooth : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 10;

            Projectile.light = 1;
            Projectile.scale = 1;
        }
        public enum Alts
        {
            BigNormal,
            BigAlt,
            Small
        }
        ref float Alt => ref Projectile.ai[1];
        ref float Timer => ref Projectile.ai[2];
        //ref float OriginalVelX => ref Projectile.ai[0];
        //ref float OriginalVelY => ref Projectile.ai[1];
        public override void AI()
        {
            
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            if (recolor)
                Lighting.AddLight(Projectile.Center, 25f / 255, 47f / 255, 64f / 255);
            else
                Lighting.AddLight(Projectile.Center, .4f, 1.2f, .4f);


            if (Projectile.localAI[0] == 0) //random flip, decide alt
            {
                Projectile.localAI[0] = 1;
                Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;

                if (Alt == (int)Alts.BigNormal && Main.rand.NextBool())
                    Alt = (int)Alts.BigAlt;

                if (Alt == (int)Alts.Small)
                {
                    Projectile.position = Projectile.Center;
                    Projectile.width = Projectile.height = 15;
                    Projectile.Center = Projectile.position;
                }
            }
            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            float MaxSpeed = Projectile.ai[0];
            if (Projectile.velocity.Length() < MaxSpeed)
                Projectile.velocity *= 1.035f;

            Projectile.tileCollide = Timer > 40;
            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.hostile = false;
                Projectile.Opacity -= 0.01f;
                if (Projectile.Opacity < 0.05)
                    Projectile.Kill();
            }
            /*
            const int turnTime = 80;
            if (Timer < turnTime)
            {
                OriginalVelX = Projectile.velocity.X;
                OriginalVelY = Projectile.velocity.Y;
            }
            const int turnDuration = 20;
            if (Timer >= turnTime && Timer <= turnTime + turnDuration)
            {
                
                Projectile.velocity.X = (float)Utils.Lerp(OriginalVelX, -OriginalVelX, (Timer - turnTime) / turnDuration);
                Projectile.velocity.Y = (float)Utils.Lerp(OriginalVelY, -OriginalVelY, (Timer - turnTime) / turnDuration);
            }
            */
            Timer++;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            string suffix = "";
            if (!recolor)
                suffix += "Vanilla";
            suffix += (Alts)Alt switch
            {
                Alts.BigAlt => "2",
                Alts.Small => "Small",
                _ =>  ""
            };
            Texture2D texture = ModContent.Request<Texture2D>($"FargowiltasSouls/Content/Projectiles/Masomode/Bosses/Plantera/PlanteraTooth{suffix}").Value;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color2 = recolor ? Color.DimGray : Color.LimeGreen * 0.75f;
                color2 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2;
                float rot = Projectile.oldRot[i];
                FargoSoulsUtil.GenericProjectileDraw(Projectile, color2, texture: texture, drawPos: pos, rotation: rot);
            }

            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture: texture);
            return false;
        }
    }
}
