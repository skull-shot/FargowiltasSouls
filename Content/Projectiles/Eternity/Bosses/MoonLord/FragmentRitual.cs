using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.MoonLord
{
    public class FragmentRitual : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Bosses/MoonLord", Name);
        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 140f;
        private const float threshold = 600f;

        public int RitualTexture = 0;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.MoonLordCore);
            if (npc != null && npc.ai[0] != 2f)
            {
                Projectile.hide = false;

                Projectile.alpha -= 2;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                Projectile.Center = npc.Center;

                Projectile.localAI[0] = (int)npc.GetGlobalNPC<MoonLordCore>().VulnerabilityTimer / 56.25f; //number to hide
                Projectile.localAI[0]--;

                //The numbers may seem random but they correspond to each Lunar Fragment's final ItemID number.
                RitualTexture = npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState switch //match ML vulnerability to fragment
                {
                    Content.Bosses.VanillaEternity.MoonLord.ClassState.Melee => 8, // Solar
                    Content.Bosses.VanillaEternity.MoonLord.ClassState.Ranged => 6, // Vortex
                    Content.Bosses.VanillaEternity.MoonLord.ClassState.Magic => 7, // Nebula
                    Content.Bosses.VanillaEternity.MoonLord.ClassState.Summon => 9, // Stardust
                    _ => 0, // All Class
                };
            }
            else
            {
                Projectile.hide = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.alpha += 2;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.timeLeft = 2;
            Projectile.scale = (1f - Projectile.alpha / 255f) * 1.5f + (Main.mouseTextColor / 200f - 0.35f) * 0.25f; //throbbing
            if (Projectile.scale < 0.1f) //clamp scale
                Projectile.scale = 0.1f;
            Projectile.ai[0] += rotationPerTick;
            if (Projectile.ai[0] > PI)
            {
                Projectile.ai[0] -= 2f * PI;
                Projectile.netUpdate = true;
            }
            Projectile.rotation = Projectile.ai[0];

            /*Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
            }*/
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Type].Value;
            if (RitualTexture != 0)
            {
                texture2D13 = Main.Assets.Request<Texture2D>($"Images/Item_345{RitualTexture}", AssetRequestMode.ImmediateLoad).Value;
            }
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Projectile.GetAlpha(lightColor);

            const int max = 32;
            for (int x = 0; x < max; x++)
            {
                if (x < Projectile.localAI[0])
                    continue;
                Vector2 drawOffset = new(threshold * Projectile.scale / 2f, 0);//.RotatedBy(Projectile.ai[0]);
                drawOffset = drawOffset.RotatedBy(2f * PI / max * (x + 1) - PI / 2);

                Color drawColor = color26;
                float dist = Main.LocalPlayer.Distance(Projectile.Center + drawOffset);
                float mult = 1f;
                if (dist < 1500)
                    mult = MathHelper.Lerp(0.15f, 1, dist / 1500f);
                drawColor *= mult;
                /*const int max = 4;
                for (int i = 0; i < max; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(max - i) / max;
                    Vector2 value4 = Projectile.Center + drawOffset.RotatedBy(-rotationPerTick * i);
                    float num165 = Projectile.rotation;
                    Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
                }*/

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f;
                    Color glowColor = Color.White * mult;

                    Main.EntitySpriteDraw(texture2D13, Projectile.Center + afterimageOffset + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
                }
                Main.spriteBatch.ResetToDefault();

                Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), drawColor, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity * .75f;
        }
    }
}