﻿using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Jungle
{
    public class Bees : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Bee,
            NPCID.BeeSmall
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Venom] = true;
        }

        //public override void OnSpawn(NPC npc)
        //{
        //    base.OnSpawn(npc);

        //    if (Main.rand.NextBool(5))
        //        switch ((Main.hardMode && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.beeBoss, NPCID.QueenBee)) ? Main.rand.Next(16, 21) : Main.rand.Next(16))
        //        {
        //            case 0: npc.Transform(NPCID.Hornet); break;
        //            case 1: npc.Transform(NPCID.HornetFatty); break;
        //            case 2: npc.Transform(NPCID.HornetHoney); break;
        //            case 3: npc.Transform(NPCID.HornetLeafy); break;
        //            case 4: npc.Transform(NPCID.HornetSpikey); break;
        //            case 5: npc.Transform(NPCID.HornetStingy); break;
        //            case 6: npc.Transform(NPCID.LittleHornetFatty); break;
        //            case 7: npc.Transform(NPCID.LittleHornetHoney); break;
        //            case 8: npc.Transform(NPCID.LittleHornetLeafy); break;
        //            case 9: npc.Transform(NPCID.LittleHornetSpikey); break;
        //            case 10: npc.Transform(NPCID.LittleHornetStingy); break;
        //            case 11: npc.Transform(NPCID.BigHornetFatty); break;
        //            case 12: npc.Transform(NPCID.BigHornetHoney); break;
        //            case 13: npc.Transform(NPCID.BigHornetLeafy); break;
        //            case 14: npc.Transform(NPCID.BigHornetSpikey); break;
        //            case 15: npc.Transform(NPCID.BigHornetStingy); break;
        //            case 16: npc.Transform(NPCID.MossHornet); break;
        //            case 17: npc.Transform(NPCID.BigMossHornet); break;
        //            case 18: npc.Transform(NPCID.GiantMossHornet); break;
        //            case 19: npc.Transform(NPCID.LittleMossHornet); break;
        //            case 20: npc.Transform(NPCID.TinyMossHornet); break;
        //        }
        //}

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.beeBoss, NPCID.QueenBee) && Main.npc[EModeGlobalNPC.beeBoss] is NPC n && n.TryGetGlobalNPC(out QueenBee qb) && qb.RunEmodeAI)
            {
                npc.life = 0;
                npc.HitEffect();
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
            target.AddBuff(ModContent.BuffType<SwarmingBuff>(), 600);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.beeBoss, NPCID.QueenBee) && Main.npc[EModeGlobalNPC.beeBoss] is NPC n && n.TryGetGlobalNPC(out QueenBee qb) && qb.RunEmodeAI)
            {
                SpriteEffects effects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f;
                    Color glowColor = Color.Goldenrod;

                    Main.EntitySpriteDraw(TextureAssets.Npc[npc.type].Value, npc.Center + afterimageOffset - Main.screenPosition + new Vector2(0f, npc.gfxOffY), npc.frame, glowColor,
                        npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0);
                }
                Main.spriteBatch.ResetToDefault();
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}
