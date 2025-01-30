using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Snow
{
    public class Penguins : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Penguin,
            NPCID.PenguinBlack,
            NPCID.CorruptPenguin,
            NPCID.CrimsonPenguin
        );

        public override bool SafePreAI(NPC npc)
        {
            if (npc.velocity.X < 0f)
            {
                npc.direction = -1;
            }
            if (npc.velocity.X > 0f)
            {
                npc.direction = 1;
            }

            if (npc.wet)
            {
                npc.noGravity = true;

                float num6 = 5f;
                if (npc.velocity.Y > 0f)
                {
                    num6 = 3f;
                }
                if (npc.velocity.Y < 0f)
                {
                    num6 = 8f;
                }

                Vector2 vector2 = new Vector2(npc.direction, -1f);
                vector2.Normalize();
                vector2 *= num6;
                if (num6 < 5f)
                {
                    npc.velocity = (npc.velocity * 24f + vector2) / 25f;
                }
                else
                {
                    npc.velocity = (npc.velocity * 9f + vector2) / 10f;
                }
            }
            else
            {
                npc.noGravity = false;
            }

            return base.SafePreAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (npc.type == NPCID.CorruptPenguin || npc.type == NPCID.CrimsonPenguin)
            {
                target.AddBuff(ModContent.BuffType<SqueakyToyBuff>(), 120);
            }
        }
    }
}
