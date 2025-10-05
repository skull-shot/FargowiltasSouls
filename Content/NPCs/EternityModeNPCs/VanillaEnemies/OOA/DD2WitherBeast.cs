using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.OOA;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2WitherBeast : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2WitherBeastT2,
            NPCID.DD2WitherBeastT3
        );

        public int Timer = 0;
        public int Shield = -1;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.Write7BitEncodedInt(Shield);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            Timer = binaryReader.Read7BitEncodedInt();
            Shield = binaryReader.Read7BitEncodedInt();
        }

        public override bool SafePreAI(NPC npc)
        {
            if (!DD2Event.Ongoing)
                return base.SafePreAI(npc);


            Timer++;
            if (Shield == -1)
            {
                foreach (NPC n in Main.npc.Where(x => x.active && x.type == npc.type))
                {
                    if (n.ai[0] == 1 && Collision.CheckAABBvAABBCollision(n.position, n.Size, npc.position, npc.Size))
                    {
                        Timer -= 2;
                    }
                }
            }
            if (Timer > 120 && npc.ai[0] == 0)
                npc.ai[0] = 109;

            if (npc.ai[0] == 1 && Shield == -1)
            {
                SoundEngine.PlaySound(SoundID.Item28 with { Pitch = -0.5f, Volume = 2f }, npc.Center);
                for (int i = 0; i < 10; i++)
                {
                    float rot = MathHelper.TwoPi * i / 10f;
                    new SmallSparkle(npc.Center + Vector2.UnitX, 1.5f * Vector2.UnitX.RotatedBy(rot), Color.Purple, 1f, 15).Spawn();
                }
                Shield = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center - 10 * Vector2.UnitY, Vector2.Zero, ModContent.ProjectileType<OOAForcefield>(), 0, 0f, ai1: npc.HorizontalDirectionTo(EModeDD2Event.GetEterniaCrystal().Center));
            }
            return base.SafePreAI(npc);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //EModeGlobalNPC.Aura(npc, 300, BuffID.WitheredArmor, false, 119);
            //EModeGlobalNPC.Aura(npc, 300, BuffID.WitheredWeapon, false, 14);
        }
    }
}
