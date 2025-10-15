using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class BlazingWheel : SpikeBall
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BlazingWheel);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.scale *= 1.5f;

            npc.dontTakeDamage = false;
            npc.lifeMax = 75;
            npc.defense = 25;

            npc.value = 450;
            npc.HitSound = SoundID.LiquidsWaterLava;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.lavaImmune = true;
        }

        private int directionCounter = 600;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(directionCounter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            directionCounter = binaryReader.ReadInt32();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //spawn smokescreen
            /*if ((npc.collideX || npc.collideY) && ++Counter >= 10)
            {
                Counter = 0;

                int num151 = Main.rand.Next(15, 20);
                for (int num152 = 0; num152 < num151; num152++)
                {
                    int num153 = Dust.NewDust(npc.position, npc.width, npc.height, 31, 0f, 0f, 100);
                    Dust dust2 = Main.dust[num153];
                    dust2.alpha += Main.rand.Next(100);
                    dust2 = Main.dust[num153];
                    dust2.velocity *= 0.3f;
                    Main.dust[num153].velocity.X += (float)Main.rand.Next(-10, 11) * 0.025f;
                    Main.dust[num153].velocity.Y -= 0.4f + (float)Main.rand.Next(-3, 14) * 0.15f;
                    Main.dust[num153].fadeIn = 1.25f + (float)Main.rand.Next(20) * 0.15f;
                }
            }*/

            if (--directionCounter <= 0)
            {
                npc.direction = -npc.direction;
                directionCounter = Main.rand.Next(300, 900);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void OnKill(NPC npc)
        {
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.InfernoFork, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                Main.dust[d].noGravity = true;
            }
        }
    }
}
