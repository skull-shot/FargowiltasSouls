using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Nature
{
    public class NatureCloudMoving : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_237";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nature Cloud");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;

            CooldownSlot = 1;

            Projectile.scale = 1.5f;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.75f, 1f);

            Projectile.rotation = Projectile.rotation + Projectile.velocity.X * 0.02f;
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 3)
                    Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NatureCloudRaining>(), Projectile.damage, 0f, Main.myPlayer);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Wet, 300);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Frostburn, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Projectile.GetTexture();
            Vector2 drawPos = Projectile.GetDrawPosition();
            Rectangle frame = Projectile.GetDefaultFrame();
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12).ToRotationVector2() * 2f * Projectile.scale;
                Color glowColor = Color.Gray;

                Main.EntitySpriteDraw(texture, drawPos + afterimageOffset, frame, Projectile.GetAlpha(glowColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects);
            }
            Main.spriteBatch.ResetToDefault();
            Main.EntitySpriteDraw(texture, drawPos, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, spriteEffects);
            return false;
        }
    }
}