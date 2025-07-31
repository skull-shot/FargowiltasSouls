using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class PalmwoodShot : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Accessories/Souls", Name);
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.scale = 1f;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.penetrate = 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[2] = Main.rand.NextBool() ? 1 : 0;
        }
        public override void AI()
        {
            if (Projectile.localAI[2] == 0) // rotation
            {
                Projectile.localAI[2] = Main.rand.NextBool() ? 1 : -1;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            Projectile.rotation += Projectile.localAI[2] * Projectile.velocity.Length() * MathHelper.TwoPi / 240f;

            Projectile.velocity.Y += 0.2f;
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-4, 4));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.ai[2] == 0 ? TextureAssets.Projectile[Type].Value : FargoAssets.GetTexture2D("Content/Projectiles/Accessories/Souls", "PalmwoodShot2").Value;
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, tex);
            return false;
        }
    }
}