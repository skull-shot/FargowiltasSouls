using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Buffs;

namespace FargowiltasSouls.Content.Projectiles.Masomode.Accessories.PureHeart
{
    public class GelicWingSpike : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_920";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Spike");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.QueenSlimeMinionBlueSpike];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.QueenSlimeMinionBlueSpike);
            AIType = ProjectileID.QueenSlimeMinionBlueSpike;
            Projectile.tileCollide = true;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Generic;

            FargowiltasSouls.MutantMod.Call("LowRenderProj", Projectile);
        }

        public override bool PreAI()
        {
            if (Projectile.ai[1] == 0)
                SoundEngine.PlaySound(SoundID.Item154 with {Volume = 0.5f}, Projectile.Center);
                Projectile.ai[1] = 1;
                return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == 1)
                target.AddBuff(ModContent.BuffType<SublimationBuff>(), 30);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Main.myPlayer];
            if (Projectile.ai[2] == 1)
            {
                Texture2D pureSpikeTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Masomode/Accessories/PureHeart/PureHeartCrystal_Spike", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, pureSpikeTexture);
                return false;
            }
            return base.PreDraw(ref lightColor);
        }
    }
}