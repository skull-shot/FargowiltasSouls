using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Linq;
using FargowiltasSouls.Content.Projectiles.Deathrays;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class ElectricWhipProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.RangeMultiplier = 0.6f;
            Projectile.WhipSettings.Segments = 25;
            Projectile.rotation += Projectile.ai[0];
        }
        int[] Cooldowns = new int[Main.maxProjectiles];
        public ref float LightningCounter => ref Projectile.ai[2];
        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Red);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = [];
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);


            Main.DrawWhip_WhipBland(Projectile, list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Whip damage falloff
            Projectile.damage = (int)(Projectile.damage * 0.7);

            // Lightning
            if (LightningCounter < 4)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Cooldowns[i] > 0)
                        Cooldowns[i]--;
                    Projectile p = Main.projectile[i];
                    if (p.Alive() && p.minion && p.owner == Projectile.owner && Cooldowns[p.whoAmI] <= 0 && p.Distance(target.Center) < 300)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.Center);
                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), p.Center, p.DirectionTo(target.Center), ModContent.ProjectileType<ElectricWhipLightning>(), Projectile.originalDamage / 2, Projectile.knockBack, Projectile.owner, ai2: p.Distance(target.Center));
                        Cooldowns[p.whoAmI] = /*Cooldowns[otherMinion.whoAmI] = */45;
                        LightningCounter++;
                        if (LightningCounter >= 4)
                            return;
                    }
                }
            }
        }
    }
}
