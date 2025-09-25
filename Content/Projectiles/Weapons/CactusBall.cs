using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Weapons
{
    public class CactusBall : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Weapons", Name);
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 90;
            Projectile.penetrate = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            base.SetDefaults();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            }
            if (oldVelocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            }
            
                return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            if (Main.myPlayer == Projectile.owner) {
                for (int i = 0; i < 5; i++)
                {
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.NextFloat(2, 5), 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<CactusNeedle>(), Projectile.damage / 2, Projectile.knockBack / 3, Projectile.owner);
                    p.DamageType = DamageClass.Magic;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, lightColor with { A = 150 } * MathHelper.Lerp(1f, 0.5f, (float)i / Projectile.oldPos.Length), Projectile.oldRot[i], t.Size() / 2, MathHelper.Lerp(1f, 0.5f, (float)i / Projectile.oldPos.Length), SpriteEffects.None);  
            }
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, Main.rand.Next(100, 300));
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.X*2);
            if (Projectile.velocity.Y < 20 && Projectile.timeLeft <80 )
            {
                Projectile.velocity.Y += 0.3f;
            }
            base.AI();
        }
    }
}
