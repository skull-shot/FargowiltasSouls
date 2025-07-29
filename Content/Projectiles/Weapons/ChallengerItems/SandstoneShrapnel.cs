using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using FargowiltasSouls.Assets.Textures;

namespace FargowiltasSouls.Content.Projectiles.Weapons.ChallengerItems
{
    public class SandstoneShrapnel : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons/ChallengerItems", "SandstoneShrapnel1");
        public override void SetDefaults() 
        {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] == 2)
            {
                Projectile.width = 16;
                Projectile.height = 16;
            }
            else if (Projectile.ai[1] == 3)
            {
                Projectile.width = 12;
                Projectile.height = 20;
            }
            else if (Projectile.ai[1] == 4)
            {
                Projectile.width = 18;
                Projectile.height = 20;
            }
            base.OnSpawn(source);
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.4f;
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += 0.2f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.Center, 10, 10, DustID.t_Honey);
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex;
            if (Projectile.ai[1] != 0)
            {
                tex = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Weapons/ChallengerItems/" + GetType().Name + Projectile.ai[1]).Value;
            }
            else
            {
                tex = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Weapons/ChallengerItems/" + GetType().Name + 1).Value;
            }
            FargoSoulsUtil.DrawTexture(Main.spriteBatch, tex, 0, Projectile, lightColor, true);
            return false;
        }
    }
}
