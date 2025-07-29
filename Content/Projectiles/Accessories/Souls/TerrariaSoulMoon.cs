using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Content.Projectiles;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Accessories.Souls
{
    public class TerrariaSoulMoon : CosmosForceMoon
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Cosmos/CosmosMoon";
        public ref float NPCID => ref Projectile.ai[1];
        public override void AI()
        {
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.1f);
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.2f, 0.1f);
            Projectile.width = 150;
            Projectile.height = 150;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                SoundEngine.PlaySound(SoundID.Item92 with { Volume = 0.5f }, Projectile.Center);

                Projectile.rotation = Main.rand.NextFloat(MathF.Tau);
            }
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.Alive())
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.Distance(player.Center) > 1200)
            {
                Projectile.Kill();
                return;
            }

            Projectile.extraUpdates = 1;
            Projectile.Opacity = 1;
            Projectile.scale = 0.2f;
            Projectile.tileCollide = true;

            int npcID = (int)NPCID;
            if (!npcID.IsWithinBounds(Main.maxNPCs))
                return;
            NPC target = Main.npc[npcID];
            if (!target.Alive())
                return;

            Vector2 idlePosition = target.Center;
            Vector2 toIdlePosition = idlePosition - Projectile.Center;
            float distance = toIdlePosition.Length();
            float speed = 25f;
            float inertia = 15f;
            toIdlePosition.Normalize();
            toIdlePosition *= speed;
            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + toIdlePosition) / inertia;
            if (distance == 0)
                Projectile.velocity = Vector2.Zero;
            if (distance < Projectile.velocity.Length())
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * distance;
            if (Projectile.velocity == Vector2.Zero && distance > 10)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
            /*
            Projectile.velocity *= 1.06f;
            Projectile.velocity = Projectile.velocity.ClampLength(0, 20f);
            */

            Projectile.rotation += 0.04f;
        }
    }
}