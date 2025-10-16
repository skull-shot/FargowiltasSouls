using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Cavern
{
    public class TimInferno : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Projectiles", "Empty");

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public ref float owner => ref Projectile.ai[0];
        public ref float timer => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC parentNPC && parentNPC.type == NPCID.Tim)
            {
                owner = parentNPC.whoAmI;
            }
            base.OnSpawn(source);
        }

        public override void AI()
        {
            if (owner < 0 || owner > Main.maxNPCs)
                Projectile.Kill();
            NPC tim = Main.npc[(int)owner];
            if (!tim.active || tim.ai[0] >= 648)
                Projectile.Kill();

            timer++;
            float num = timer > 30 ? 1 : MathHelper.Lerp(0f, 1f, timer / 30);
            Projectile.width = Projectile.height = (int)(280 * num);
            Projectile.Center = tim.Center;
            Projectile.velocity *= 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            target.AddBuff(BuffID.OnFire, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/FlameRing", AssetRequestMode.ImmediateLoad).Value;

            // vanilla draw code
            float num = 1f;
            float num2 = 0.1f;
            float num3 = 0.9f;
            if (!Main.gamePaused)
            {
                Projectile.scale += 0.004f;
            }
            if (Projectile.scale < 1f)
            {
                num = Projectile.scale;
            }
            else
            {
                Projectile.scale = 0.8f;
                num = Projectile.scale;
            }
            if (!Main.gamePaused)
            {
                Projectile.rotation += 0.05f;
            }
            if (Projectile.rotation > (float)Math.PI * 2f)
            {
                Projectile.rotation -= (float)Math.PI * 2f;
            }
            if (Projectile.rotation < (float)Math.PI * -2f)
            {
                Projectile.rotation += (float)Math.PI * 2f;
            }
            for (int j = 0; j < 3; j++)
            {
                Rectangle frame = new(0, 400 * j, 400, 400);
                float num4 = num + num2 * (float)j;
                if (num4 > 1f)
                {
                    num4 -= num2 * 2f;
                }
                float num5 = MathHelper.Lerp(0.8f, 0f, Math.Abs(num4 - num3) * 10f);
                float scale = timer > 30 ? 1 : MathHelper.Lerp(0f, 1f, timer / 30);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, new Color(num5, num5, num5, num5 / 2f), Projectile.rotation + MathHelper.Pi / 3f * j, frame.Size() / 2, num4 * scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
