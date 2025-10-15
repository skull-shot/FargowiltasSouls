using FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.Desert;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class RaggedCaster : DungeonTeleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat);

        public int AttackTimer;

        public override void AI(NPC npc)
        {
            if (npc.ai[0] < 30 && AttackTimer < 180)
            {
                TeleportTimer = 0;
                npc.GetGlobalNPC<DungeonTeleporters>().TeleportTimer = 0;
                npc.ai[0] = 1;
                if (AttackTimer == 2 && FargoSoulsUtil.HostCheck)
                {
                    Vector2 pos = npc.Center - new Vector2(0, 60);
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)pos.X, (int)pos.Y, ModContent.NPCType<SoulVortex>(), ai0: npc.whoAmI);
                }
                AttackTimer++;
                if (AttackTimer <= 120)
                    npc.ai[1] = 1;
                else
                    npc.ai[1] = 0;
            }
            else
            {
                npc.ai[1] = 1;
                if (npc.HasValidTarget && !Main.dedServ)
                {
                    Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 5;
                    Dust d = Dust.NewDustPerfect(npc.Center + new Vector2(-14 * npc.direction, -14), DustID.SpectreStaff, vel, 100, Scale: 1.6f);
                    Dust d2 = Dust.NewDustPerfect(npc.Center + new Vector2(12 * npc.direction, -14), DustID.SpectreStaff, vel, 100, Scale: 1.6f);
                    d.noGravity = true;
                    d2.noGravity = true;
                }
            }
            base.AI(npc);
        }
    }
}
