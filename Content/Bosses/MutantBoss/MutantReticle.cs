using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantReticle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = WorldSavingSystem.MasochistModeReal ? 120 : 150;
            //CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())
                && !Main.npc[EModeGlobalNPC.mutantBoss].dontTakeDamage)
            {
                if (Projectile.localAI[0] == 0)
                {
                    Projectile.localAI[0] = Main.rand.NextBool() ? -1 : 1;
                    Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2);
                }

                int modifier = Math.Min(60, 90 - Projectile.timeLeft);

                Projectile.scale = 1.5f - 0.5f / 60f * modifier; //start big, shrink down
                if (Projectile.ai[2] == 1)
                    Projectile.scale *= 0.9f;

                Projectile.velocity = Vector2.Zero;
                Projectile.rotation += MathHelper.ToRadians(6) * Projectile.localAI[0];
            }
            else
            {
                Projectile.Kill();
            }

            if (Projectile.timeLeft < 15)
                Projectile.alpha += 17;

            else
            {
                Projectile.alpha -= 4;
                if (Projectile.alpha < 0) //fade in
                    Projectile.alpha = 0;

                if (Projectile.timeLeft % 15 == 0 && !Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ReticleBeep"), Projectile.Center);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Projectile.ai[2] == 0
                ? Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value
                : ModContent.Request<Texture2D>($"{Texture}_Queen", AssetRequestMode.ImmediateLoad).Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}