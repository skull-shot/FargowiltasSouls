﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.MechanicalBosses
{
    public class PrimeTrail : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles/Eternity/Bosses/MechanicalBosses", Name);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.8f;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            bool fade = false;

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice);
            if (npc != null)
            {
                Projectile.Center = npc.Center;
                if (Projectile.ai[1] == 0) //swipe limb
                {
                    if (!npc.GetGlobalNPC<PrimeLimb>().IsSwipeLimb || npc.ai[2] < 140)
                        fade = true;
                }
                else if (Projectile.ai[1] == 1) //attack limb while spinning
                {
                    if (npc.GetGlobalNPC<PrimeLimb>().IsSwipeLimb || Main.npc[(int)npc.ai[1]].ai[1] != 1 && Main.npc[(int)npc.ai[1]].ai[1] != 2)
                        fade = true;
                }
                else if (Projectile.ai[1] == 2) //attack limb doing attacks
                {
                    if (npc.GetGlobalNPC<PrimeLimb>().IsSwipeLimb || Main.npc[(int)npc.ai[1]].ai[1] == 1 || Main.npc[(int)npc.ai[1]].ai[1] == 2)
                        fade = true;
                }
                else if (Projectile.ai[1] == 3) // dash attack
                {
                    if (++Projectile.localAI[0] >= Projectile.ai[2])
                        fade = true;
                }
            }
            else
            {
                fade = true;
            }

            if (fade)
            {
                Projectile.alpha += 8;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.alpha -= Projectile.ai[1] == 0 ? 16 : 8;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float increment;
            if (Projectile.ai[1] == 1f || Projectile.ai[1] == 3f)
                increment = 0.1f;
            else if (Projectile.ai[1] == 2f)
                increment = 0.5f;
            else
                increment = 0.25f;

            Texture2D glow = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += increment)
            {
                int max0 = (int)i - 1;
                if (max0 < 0)
                    continue;

                Color color27;
                if (Projectile.ai[1] == 1 || Projectile.ai[1] == 3)
                    color27 = Color.Red * 0.7f; // red //new Color(191, 51, 255, 210); //purple
                else if (Projectile.ai[1] == 2)
                    color27 = new Color(51, 255, 191, 210) * 0.75f; //teal
                else
                    color27 = new Color(255, 255, 75, 210); //yellow

                color27 *= 0.3f * Projectile.Opacity;
                color27 *= (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale;
                scale *= (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);

                center += Projectile.Size / 2;

                Main.EntitySpriteDraw(
                    glow,
                    center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    null,
                    color27,
                    Projectile.rotation,
                    glow.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0);
            }

            return false;
        }
    }
}