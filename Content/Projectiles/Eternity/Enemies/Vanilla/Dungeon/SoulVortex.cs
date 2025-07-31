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

namespace FargowiltasSouls.Content.Projectiles.Masomode.Enemies.Vanilla.Dungeon
{
    public class SoulVortex : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {

            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.scale = 0.1f;
            Projectile.ignoreWater = true;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * 0.5f, Projectile.rotation * 0.8f, t.Size() / 2, Projectile.scale * 1.7f, SpriteEffects.None);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * 0.5f, Projectile.rotation * 0.9f, t.Size() / 2, Projectile.scale * 1.7f, SpriteEffects.None);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * 1, Projectile.rotation * 1.5f, t.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * 1, -Projectile.rotation * 1.25f, t.Size() / 2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * 1, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.rotation -= MathHelper.ToRadians(2);
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.35f, 0.4f));
            if (!(owner.active && (owner.type == NPCID.RaggedCaster || owner.type == NPCID.RaggedCasterOpenCoat)))
            {
                Projectile.ai[1]--;
                float x = Projectile.ai[1] / 120f;
                //ease out quad but reversed because ai[1] goes down so ease in quad
                Projectile.scale = MathHelper.Lerp(0, 1, 1 - (1 - x) * (1 - x));
                Projectile.rotation -= MathHelper.ToRadians(MathHelper.Lerp(2, 1, Projectile.scale));
                if (Projectile.scale <= 0) Projectile.Kill();
            }
            else
            {
                if (owner.HasValidTarget && Projectile.ai[1] >= 120)
                {
                    Player target = Main.player[owner.target];
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.AngleTo(target.Center).ToRotationVector2() * 3, 0.04f);
                }

                if (Projectile.scale < 1)
                {
                    Projectile.ai[1]++;
                    float x = Projectile.ai[1] / 120f;
                    //ease out quad
                    Projectile.scale = MathHelper.Lerp(0, 1, 1 - (1 - x) * (1 - x));
                    Projectile.rotation -= MathHelper.ToRadians(MathHelper.Lerp(2, 1, Projectile.scale));
                    if (Projectile.ai[1] == 120 && owner.HasValidTarget)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, Projectile.Center);
                        Projectile.velocity = Projectile.AngleTo(Main.player[owner.target].Center).ToRotationVector2() * 10;
                    }
                }
            }


            base.AI();
        }
    }
}
