using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.Plantera
{
    public class DicerPlanteraSeed : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.SeedPlantera);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PoisonSeedPlantera);
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
            Projectile.width = Projectile.height = 24;
        }
        public override void AI()
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            if (recolor)
                Lighting.AddLight(Projectile.Center, 25f / 255, 47f / 255, 64f / 255);
            else
                Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.2f);

            if (Projectile.localAI[0] != 1)
            {
                //SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                Projectile.localAI[0] = 1;
            }
            if (Projectile.timeLeft < 4)
            {
                Projectile.Opacity -= 0.25f;
                Projectile.hostile = false;
            }
            if (++Projectile.frameCounter > 3)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<IvyVenomBuff>(), 240);
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[WorldSavingSystem.MasochistModeReal ? ProjectileID.PoisonSeedPlantera : ProjectileID.SeedPlantera].Value;
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture);
            return false;
        }
    }
}
