﻿using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Buffs.Eternity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.MoonLord
{
    public class LunarRitual : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        private const float maxSize = 1600f;

        public LunarRitual() : base(MathHelper.Pi / 140f, maxSize, NPCID.MoonLordCore) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Lunar Ritual");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = ImmunityCooldownID.Bosses;
        }
        protected override void Movement(NPC npc)
        {
            Vector2 target = npc.Center;
            if (npc.HasValidTarget) //tracks halfway between player and boss
                target += (Main.player[npc.target].Center - npc.Center) / 2;

            if (Projectile.Distance(target) <= 1)
                Projectile.Center = target;
            else if (Projectile.Distance(target) > threshold)
                Projectile.velocity = (target - Projectile.Center) / 30;
            else if (npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4 && npc.GetGlobalNPC<MoonLordCore>().VulnerabilityTimer < 60 && !npc.dontTakeDamage)
                Projectile.velocity = (Main.player[npc.target].Center - Projectile.Center) * 0.05f;
            else
                Projectile.velocity = Projectile.SafeDirectionTo(target);

            threshold += 6;
            if (threshold > maxSize)
                threshold = maxSize;
        }

        public override void AI()
        {
            base.AI();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 300);
        }
    }
}