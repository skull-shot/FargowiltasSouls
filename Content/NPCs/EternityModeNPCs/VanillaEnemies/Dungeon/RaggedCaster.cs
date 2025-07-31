using FargowiltasSouls.Content.Projectiles.Masomode.Enemies.Vanilla.Dungeon;
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
                if (AttackTimer == 2)
                {
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center - new Microsoft.Xna.Framework.Vector2(0, 60), Vector2.Zero, ModContent.ProjectileType<SoulVortex>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1, ai0: npc.whoAmI);
                }
                AttackTimer++;
                if (AttackTimer <= 120)
                    npc.ai[1] = 1;
                else
                    npc.ai[1] = 0;
            }
            else
            {
                npc.ai[1] = 0;
            }
                base.AI(npc);

        }


    }
}
