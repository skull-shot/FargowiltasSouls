﻿using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantDestroyerHead : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantDestroyerHead_April" :
            "FargowiltasSouls/Assets/Textures/EModeResprites/NPC_134";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Destroyer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Projectile.ai[2] == 0
                ? Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value
                : ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Textures/EModeResprites/NPC_13", AssetRequestMode.ImmediateLoad).Value;
            int num214 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        public override void AI()
        {
            //keep the head looking right
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
            Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            const int homingDelay = 60;
            float desiredFlySpeedInPixelsPerFrame = 10 * Projectile.ai[1];
            float amountOfFramesToLerpBy = 25 / Projectile.ai[1]; // minimum of 1, please keep in full numbers even though it's a float!

            if (++Projectile.localAI[1] > homingDelay)
            {
                int foundTarget = (int)Projectile.ai[0];
                Player p = Main.player[foundTarget];
                if (Projectile.Distance(p.Center) > 700)
                {
                    desiredFlySpeedInPixelsPerFrame *= 2;
                    amountOfFramesToLerpBy /= 2;
                }
                Vector2 desiredVelocity = Projectile.SafeDirectionTo(p.Center) * desiredFlySpeedInPixelsPerFrame;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
            }

            const float IdleAccel = 0.05f;
            foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == Projectile.type && p.whoAmI != Projectile.whoAmI && p.Distance(Projectile.Center) < Projectile.width))
            {
                Projectile.velocity.X += IdleAccel * (Projectile.position.X < p.position.X ? -1 : 1);
                Projectile.velocity.Y += IdleAccel * (Projectile.position.Y < p.position.Y ? -1 : 1);
                p.velocity.X += IdleAccel * (p.position.X < Projectile.position.X ? -1 : 1);
                p.velocity.Y += IdleAccel * (p.position.Y < Projectile.position.Y ? -1 : 1);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 240);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.RedTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
            //int g = Gore.NewGore(Projectile.Center, Projectile.velocity / 2, mod.GetGoreSlot("Assets/Gores/DestroyerGun/DestroyerHead"), Projectile.scale);
            // Main.gore[g].timeLeft = 20;
            SoundEngine.PlaySound(Projectile.ai[2] == 0 ? SoundID.NPCDeath14 : SoundID.NPCDeath1, Projectile.Center);
        }
    }
}