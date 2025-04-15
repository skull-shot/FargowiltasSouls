using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Snow
{
    public class Wolf : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Wolf);

        public int AttackTimer;
        public Vector2 targetPos;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            AttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(3) && npc.FargoSouls().CanHordeSplit)
                EModeGlobalNPC.Horde(npc, Main.rand.Next(10) + 1);
        }

        public override bool SafePreAI(NPC npc)
        {
            AttackTimer++;
            if (AttackTimer >= 180)
            {
                if (AttackTimer == 180)
                {

                }
                if (AttackTimer == 200 && npc.HasValidTarget)
                {
                    targetPos = Main.player[npc.target].position;
                    Vector2 dist = targetPos - npc.position;
                    npc.velocity = 10 * Vector2.UnitX.RotatedBy(dist.ToRotation());
                }
                if (AttackTimer >= 200)
                {
                    npc.velocity.Y += 0.1f;
                    if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        AttackTimer = 0;
                    }
                }
                return false;
            }

            return base.SafePreAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Bleeding, 300);
            target.AddBuff(BuffID.Rabies, 900);
        }
    }
}
