﻿using FargowiltasSouls.Content.Buffs.Eternity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.DukeFishron
{
    public class FishronRitual2 : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_409";

        public FishronRitual2() : base(MathHelper.Pi / 140f, 1600f, NPCID.DukeFishron) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Oceanic Ritual");
            Main.projFrames[Projectile.type] = 3;
            CooldownSlot = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = 0;
        }
        protected override void Movement(NPC npc)
        {
            Projectile.velocity = npc.Center - Projectile.Center;
            Projectile.velocity /= 20f;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation += 0.2f;
            Projectile.frame++;
            if (Projectile.frame > 2)
                Projectile.frame = 0;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            target.FargoSouls().MaxLifeReduction += 30;
            target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 10 * 60);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 50 + (int)(100.0 * Main.DiscoG / 255.0), 255, 150) * (targetPlayer == Main.myPlayer ? 1f : 0.2f);
        }
    }
}