﻿using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Eternity;
using Microsoft.Xna.Framework;
using System.Security.Principal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.LunaticCultist
{
    public class CultistRitual : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public CultistRitual() : base(MathHelper.Pi / -140f, 1600f, NPCID.CultistBoss) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Cultist Ritual");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = ImmunityCooldownID.Bosses;
        }
        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] == 5)
            {
                int ritual = (int)npc.ai[2];
                ritual = FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, (int)npc.ai[2], ProjectileID.CultistRitual);
                if (ritual > -1 && ritual < Main.maxProjectiles && Main.projectile[ritual].active && Main.projectile[ritual].type == ProjectileID.CultistRitual)
                {
                    Projectile.Center = Main.projectile[ritual].Center;
                }
            }
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

        public override void PostAI()
        {
            base.PostAI();
            Projectile.hide = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 300);
        }
    }
}