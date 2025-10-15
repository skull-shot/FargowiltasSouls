﻿using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.Golem
{
    public class EyeBeam2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_259";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye Beam");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeBeam);
            AIType = ProjectileID.EyeBeam;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 300);
            NPC npc = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);
            if (npc != null)
            {
                target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 90);
                if (Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] == null || //outside temple
                    Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].WallType != WallID.LihzahrdBrickUnsafe)
                {
                    target.AddBuff(ModContent.BuffType<DaybrokenBuff>(), 120);
                }
            }

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<EarthChampion>()))
            {
                target.AddBuff(ModContent.BuffType<DaybrokenBuff>(), 300);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}