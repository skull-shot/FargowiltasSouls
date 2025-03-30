using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Snow
{
    public class SnowFlinx : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SnowFlinx);

        private static Texture2D snowballTexture;
        public override void Load()
        {
            snowballTexture = TextureAssets.Projectile[ProjectileID.SnowBallHostile].Value;
        }

        public int AttackTimer;
        public int State;
        
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write7BitEncodedInt(State);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            AttackTimer = binaryReader.Read7BitEncodedInt();
            State = binaryReader.Read7BitEncodedInt();

        }

        public override bool SafePreAI(NPC npc)
        {
            AttackTimer++;
            //Main.NewText(AttackTimer);
            if (AttackTimer > 240)
            {
                State = 1;
                if (Math.Abs(npc.velocity.X) < 15)
                    npc.velocity.X = npc.direction * (float) Math.Pow(1.1, (AttackTimer - 300));
                float rot = (AttackTimer - 240) / 50f;
                if (rot > 0.6f)
                    rot = 0.6f;

                Main.NewText(rot);
                npc.rotation += npc.direction * rot;
                if (AttackTimer > 360)
                {
                    AttackTimer = 0;
                }
                return false;
            }
            State = 0;

            return base.SafePreAI(npc);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (State == 1)
                modifiers.Knockback *= 0;
            base.ModifyIncomingHit(npc, ref modifiers);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (State == 1 && AttackTimer > 300)
            {
                Rectangle frame = new(0, 0, snowballTexture.Width, snowballTexture.Height);

                Main.EntitySpriteDraw(snowballTexture, npc.Center, frame, drawColor, npc.rotation, new Vector2(frame.Width / 2, frame.Height / 2), npc.scale, SpriteEffects.None);
                return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 600);
        }
    }
}
