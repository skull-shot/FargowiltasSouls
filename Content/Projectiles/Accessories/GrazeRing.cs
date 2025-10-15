using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories
{
    public class GrazeRing : GlowRingHollow
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles", "GlowRingHollow");
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.alpha = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            color = Color.HotPink;

            Projectile.FargoSouls().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            if (!player.active || player.dead || player.ghost || Projectile.owner == Main.myPlayer && (!fargoPlayer.Graze || !player.HasEffect<MasoGrazeRing>()))
            {
                Projectile.Kill();
                return;
            }

            if (fargoPlayer.CirnoGraze)
                color = Color.Cyan;
            else if (fargoPlayer.DeviGraze)
                color = Color.HotPink;

            float radius = Player.defaultHeight + fargoPlayer.GrazeRadius;

            Projectile.timeLeft = 2;
            Projectile.Center = player.Center;

            Projectile.Opacity = Main.mouseTextColor / 255f;
            Projectile.Opacity *= Projectile.Opacity;

            Projectile.scale = radius * 2f / 1000f;

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = (int)(1000 * Projectile.scale);
            Projectile.Center = Projectile.position;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return base.GetAlpha(lightColor) * 0.8f * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Projectile.owner != Main.myPlayer)
                return false;
            return base.PreDraw(ref lightColor);
        }
    }
    public class MasoGrazeRing : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SparklingAdoration>();
        
    }
}