using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Nature
{
    public class NatureCloudRaining : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_238";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nature Cloud");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;

            Projectile.scale = 1.5f * 1.5f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.75f, 1f);

            float time = WorldSavingSystem.MasochistModeReal ? 12 : 20;
            if (++Projectile.ai[0] > time)
            {
                Projectile.ai[0] = 0;

                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.position.X + 14 + Main.rand.Next(Projectile.width - 28),
                        Projectile.position.Y + Projectile.height + 4, 0f, 5f,
                        ModContent.ProjectileType<NatureRain>(), Projectile.damage, 0f, Main.myPlayer);
                }
            }

            if (++Projectile.ai[1] > 450)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }

            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 5)
                    Projectile.frame = 0;
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