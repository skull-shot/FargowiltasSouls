using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.OOA;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2Wyvern : EModeNPCBehaviour
    {

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2WyvernT1,
            NPCID.DD2WyvernT2,
            NPCID.DD2WyvernT3
        );

        public int State = 0;
        public int Timer = 0;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.Write7BitEncodedInt(State);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            Timer = binaryReader.Read7BitEncodedInt();
            State = binaryReader.Read7BitEncodedInt();
        }

        public override bool SafePreAI(NPC npc)
        {
            Timer++;
            if (Timer < 60)
                return base.SafePreAI(npc);

            //FargoSoulsUtil.PrintAI(npc);
            if (Timer >= 240 && npc.ai[0] == 2 && npc.ai[1] == 0 && npc.HasPlayerTarget && npc.HasValidTarget)
            {
                Player p = Main.player[npc.target];
                if (npc.Center.Distance(p.Center) < 100)
                    return base.SafePreAI(npc);
                Vector2 pos = npc.Center + 115 * Vector2.UnitX.RotatedBy(Main.rand.NextFloatDirection());
                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos2 = pos.RotatedBy(MathHelper.Pi * i, npc.Center);
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), pos2, Vector2.Zero, ModContent.ProjectileType<WyvernClone>(), npc.damage / 9, 2, ai0: npc.type, ai1: npc.target);
                    SoundEngine.PlaySound(SoundID.Item43, pos2);
                }
                Timer = 60;
            }

            return base.SafePreAI(npc);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);
            target.AddBuff(BuffID.Rabies, 3600);
        }
    }
}
