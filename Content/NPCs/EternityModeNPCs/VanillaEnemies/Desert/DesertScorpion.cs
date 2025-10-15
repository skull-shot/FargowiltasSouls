﻿using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Desert;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Core.Systems.DashManager;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Desert
{
    public class DesertScorpion : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DesertScorpionWalk,
            NPCID.DesertScorpionWall
        );

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.DesertScorpionWall)
            {
                Counter++;
                if (Counter > 195 && Counter < 210)
                {
                    npc.velocity *= 1f - (Counter - 195) / 15f;
                }
                if (Counter == 210)
                {
                    FargoSoulsUtil.DustRing(npc.Center, 32, DustID.Web, 5f, default, 2f);
                }
                if (Counter >= 210)
                    npc.velocity *= 0f;
                if (Counter > 240)
                {
                    Counter = 0;

                    if (FargoSoulsUtil.HostCheck && npc.HasPlayerTarget)
                    {
                        Vector2 vel = npc.SafeDirectionTo(Main.player[npc.target].Center) * 14;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<VenomSpit>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0, Main.myPlayer);
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 300);
        }
    }
}
