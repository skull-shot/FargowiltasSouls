using Fargowiltas.Content.NPCs;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosTelegraph : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.Opacity = 0f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;

            Projectile.hide = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public ref float PlayerID => ref Projectile.ai[0];
        public ref float NPCID => ref Projectile.ai[1];
        public ref float LengthExtra => ref Projectile.ai[2];
        public override void AI()
        {
            int playerID = (int)PlayerID;
            int npcID = (int)NPCID;
            if (npcID.IsWithinBounds(Main.maxNPCs) && Main.npc[npcID] is NPC npc && npc.Alive())
            {
                Projectile.Center = npc.Center;
            }
            if (playerID.IsWithinBounds(Main.maxPlayers) && Main.player[playerID] is Player player && player.Alive())
            {
                Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation();
            }

            if (Projectile.timeLeft > 10)
            {
                Projectile.Opacity += 0.1f;
            }
            else
            {
                Projectile.Opacity -= 0.1f;
            }
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity, 0, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> line = TextureAssets.Extra[178];
            float opacity = Projectile.Opacity;
            Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, new Color(200, 0, 200) * opacity, Projectile.rotation, new Vector2(0, line.Height() * 0.5f), new Vector2(0.6f + LengthExtra, Projectile.scale * 24), SpriteEffects.None);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCs.Add(index);
        }
    }
}