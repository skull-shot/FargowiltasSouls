using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_462";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Bolt");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.PhantasmalBolt;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.scale = 2f;
            Projectile.hostile = true;
            Projectile.extraUpdates = 3;

            Projectile.timeLeft = 75 * 4;
            CooldownSlot = ImmunityCooldownID.Bosses;

            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override void AI()
        {
            int index = Dust.NewDust(Projectile.Center, 0, 0, DustID.Vortex, 0.0f, 0.0f, 100, new Color(), 1f);
            Main.dust[index].noLight = true;
            Main.dust[index].noGravity = true;
            Main.dust[index].velocity = Projectile.velocity;
            Main.dust[index].position -= Vector2.One * 4f;
            Main.dust[index].scale = 0.8f;
            /*if (++Projectile.frameCounter >= 12 * 12)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                    Projectile.frame = 0;
            }*/

            if (WorldSavingSystem.EternityMode && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<CosmosChampion>()))
            {
                float rotation = Projectile.velocity.ToRotation();
                Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.championBoss].target].Center - Projectile.Center;
                float targetAngle = vel.ToRotation();
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.001f));
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CosmosDeathray>(), Projectile.damage, 0f, Main.myPlayer, 1f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 360);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> line = TextureAssets.Extra[178];
            float opacity = Projectile.Opacity * 0.75f;
            float lerper = Projectile.timeLeft / (75f * 4);
            float lineLength = MathHelper.SmoothStep(1f, 0f, lerper);
            Color color = Color.Lerp(Color.White, Color.Cyan, MathF.Pow(lerper, 0.5f));
            Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, color * opacity, Projectile.rotation - MathHelper.PiOver2, new Vector2(0, line.Height() * 0.5f), new Vector2(0.7f * lineLength, Projectile.scale * 2), SpriteEffects.None);


            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}