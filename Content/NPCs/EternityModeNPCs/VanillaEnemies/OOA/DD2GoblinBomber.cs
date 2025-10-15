﻿using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.OOA;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2GoblinBomber : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2GoblinBomberT1,
            NPCID.DD2GoblinBomberT2,
            NPCID.DD2GoblinBomberT3
        );

        public int Timer = 0;
        public int rand = 0;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.Write7BitEncodedInt(rand);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            Timer = binaryReader.Read7BitEncodedInt();
            rand = binaryReader.Read7BitEncodedInt();
        }

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasPlayerTarget)
                npc.TargetClosest();

            Timer++;
            if (Timer == 180)
                rand = Main.rand.Next(3);
            if (Timer > 180 && npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 700)
            {
                if (rand == 0) // chance to not throw
                {
                    Vector2 targetPos = Main.player[npc.target].Center - npc.Center;
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextFloat(0.8f, 1.2f) * 8 * Vector2.UnitX.RotatedBy(targetPos.ToRotation()) - 2 * (npc.Center.Y - targetPos.Y) / 2000f * Vector2.UnitY, ModContent.ProjectileType<StinkBomb>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1);
                    SoundEngine.PlaySound(FargosSoundRegistry.ThrowShort with { Pitch = 1f }, npc.Center);
                }
                Timer = 0;
            }

            return base.SafePreAI(npc);
        }
    }
}
