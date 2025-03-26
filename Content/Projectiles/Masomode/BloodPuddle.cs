using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BloodPuddle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            Main.projFrames[Type] = 5;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 8;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1600;
            Projectile.scale = Main.rand.NextFloat(0.8f, 1.4f);
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 20)
            {
                Projectile.frame++;
                Projectile.localAI[0] = 0;
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 0;
                }
            }

            
            Player target = null;
            if (Projectile.ai[0] == 0) {
                int p = Player.FindClosest(Projectile.Center, 1, 1);
                if (p >= 0) target = Main.player[p];
                
                if (target != null && target.Hitbox.Intersects(Projectile.Hitbox) && Projectile.ai[0] == 0)
                {
                    Projectile.ai[2] = target.whoAmI;
                    Projectile.ai[0] = 1;
                    SoundEngine.PlaySound(SoundID.NPCDeath11, Projectile.Center);
                }
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1] += 2;
                Projectile.timeLeft = 20;
                target = Main.player[(int)Projectile.ai[2]];
                target.AddBuff(ModContent.BuffType<BleedingOut>(), 2);
                
                if (Projectile.ai[1] >= 160)
                {
                    Projectile.ai[1] = -40;
                }
                if (Projectile.Distance(target.Center) > 300)
                {
                    for (int i = 0; i < Projectile.Distance(target.Center); i += 3)
                    {
                        Dust.NewDustDirect(target.Center + target.AngleTo(Projectile.Center).ToRotationVector2() * i, 1, 1, DustID.Blood);
                    }
                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit9, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Asset<Texture2D> vein = ModContent.Request<Texture2D>("Terraria/Images/Chain12");
            int frameHeight = t.Height() / Main.projFrames[Type];
           // Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, frameHeight * Projectile.frame, t.Width(), frameHeight), lightColor, Projectile.rotation, new Vector2(t.Width(), frameHeight)/2, Projectile.scale, SpriteEffects.None);

            if (Projectile.ai[0] == 1)
            {
                Player target = Main.player[(int)Projectile.ai[2]];
                if (target != null && target.active)
                {
                    Vector2 bigPoint = Vector2.Lerp(Projectile.Center, target.Center, 1 - Projectile.ai[1] / 120f);
                    //Dust d = Dust.NewDustDirect(bigPoint, 1, 1, DustID.Terra);
                    //d.velocity *= 0;
                    for (int i = 0; i < Projectile.Distance(target.Center); i += (int)(vein.Height()/3 * Projectile.scale))
                    {
                        float y = Projectile.Distance(target.Center) /300;
                        Vector2 shakeOffset = new Vector2(MathHelper.Lerp(0, 1f, y*y), 0).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 pos = Projectile.Center + Projectile.AngleTo(target.Center).ToRotationVector2() * i;
                        float x = 1 - pos.Distance(bigPoint) / Projectile.Distance(target.Center);
                        float scaleAdd = MathHelper.Lerp(0, 0.5f, (x == 0 ? 0 : MathF.Pow(2, 10 * x - 10)));
                        Main.EntitySpriteDraw(vein.Value, pos - Main.screenPosition + shakeOffset, new Rectangle(0, (int)(vein.Height()/3 * ( i / 9 % (vein.Height() / 9))), vein.Width(), vein.Height()/3), lightColor, Projectile.AngleTo(target.Center) + MathHelper.PiOver2, new Vector2(vein.Width(), vein.Height()/3)/2, Projectile.scale + scaleAdd, SpriteEffects.None);
                    }

                }
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, frameHeight * Projectile.frame, t.Width(), frameHeight), lightColor, Projectile.rotation, new Vector2(t.Width(), frameHeight) / 2, Projectile.scale, SpriteEffects.None);

            return false;   
        }
    }
}