using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Hell
{
    public class ImpFireOrb : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.Fireball);

        public override void SetStaticDefaults()
        {
            
        }
        public int baseWidth = 0;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Fireball);
            Projectile.aiStyle = -1;
            AIType = 0;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60 * 8;
            Projectile.Opacity = 1f;
            Projectile.alpha = 0;
            baseWidth = Projectile.width;
        }
        public ref float TargetID => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float OwnerID => ref Projectile.ai[2];
        public override void AI()
        {
            Projectile.rotation += MathHelper.TwoPi / 7f;
            Timer++;
            float chargeTime = 60 * 4;
            float homeTime = 60;
            float maxScale = 3f;
            if (Timer < chargeTime)
            {
                if (!Main.npc[(int)OwnerID].TypeAlive(NPCID.FireImp))
                {
                    Projectile.Kill();
                    return;
                }

                Projectile.position = Projectile.Center;
                Projectile.scale = 1f + (maxScale - 1) * Timer / chargeTime;
                Projectile.width = Projectile.height = (int)(baseWidth * Projectile.scale);
                Projectile.Center = Projectile.position;
                Projectile.velocity = Vector2.Zero;
            }
            else if (Timer == chargeTime)
            {
                Player player = Main.player[(int)TargetID];
                if (player != null && player.Alive())
                {
                    Projectile.velocity = Projectile.DirectionTo(player.Center) * MathHelper.Lerp(8f, 16f, Projectile.scale / maxScale);
                }
            }
            else
            {
                if (Timer < chargeTime + homeTime)
                {
                    Player player = Main.player[(int)TargetID];
                    if (player != null && player.Alive())
                    {
                        Projectile.velocity = Projectile.velocity.RotateTowards(Projectile.DirectionTo(player.Center).ToRotation(), 0.01f);
                    }
                }
                
            }
            for (int i = 0; i < 4; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= MathF.Pow(Projectile.scale, 0.5f);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.localAI[0] == 2 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}