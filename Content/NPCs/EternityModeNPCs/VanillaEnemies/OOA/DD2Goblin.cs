using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Threading.Channels;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2Goblin : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2GoblinT1,
            NPCID.DD2GoblinT2,
            NPCID.DD2GoblinT3
        );

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        int SpawnTimer = 60;
        int Timer = -60;
        int State;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.Write7BitEncodedInt(State);
            binaryWriter.Write7BitEncodedInt(SpawnTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            Timer = binaryReader.Read7BitEncodedInt();
            State = binaryReader.Read7BitEncodedInt();
            SpawnTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            int chance = Math.Max(15 - (int)(Timer / 60), 8);
            if (State == 0 && Main.rand.NextBool(chance) && SpawnTimer < 0)
            {
                npc.dontTakeDamage = true;
                npc.HideStrikeDamage = true;
                State = 1;
                Timer = 0;
                modifiers.Null();
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -1 }, npc.Center);
                SoundEngine.PlaySound(SoundID.DD2_GoblinScream, npc.Center);
                for (int i = 0; i < 20; i++)
                {
                    Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Smoke, Scale: 2f);
                    d.noGravity = true;
                    d.velocity *= 0.7f;
                }
                return;
            }
            else
                npc.HideStrikeDamage = false;
        }

        public override void OnHitByAnything(NPC npc, Player player, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByAnything(npc, player, hit, damageDone);
        }

        public override bool SafePreAI(NPC npc)
        {
            Timer++;
            if (SpawnTimer-- >= 0)
                return base.SafePreAI(npc);

            if (State == 1)
            {
                npc.rotation += npc.spriteDirection * 2 * MathHelper.TwoPi / 30;
                npc.position += 6 * npc.spriteDirection * Vector2.UnitX;
                Dust d = Dust.NewDustDirect(npc.Bottom, 1, 1, DustID.Smoke, Scale: 2f);
                d.noGravity = true;
                d.velocity *= 0.7f;
                if (Timer >= 30)
                {
                    npc.dontTakeDamage = false;
                    State = 0;
                    Timer = 0;
                }
                return false;
            }

            return base.SafePreAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            //target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}
