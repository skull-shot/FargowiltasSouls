using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
    public class Slimes : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.BlueSlime,
            NPCID.BlackSlime,
            NPCID.Pinky,
            NPCID.SlimeRibbonGreen,
            NPCID.SlimeRibbonRed,
            NPCID.SlimeRibbonWhite,
            NPCID.SlimeRibbonYellow,
            NPCID.SlimeMasked,
            NPCID.Slimeling,
            NPCID.Slimer,
            NPCID.Slimer2,
            NPCID.SlimeSpiked,
            NPCID.BabySlime,
            NPCID.BlackSlime,
            NPCID.CorruptSlime,
            NPCID.DungeonSlime,
            NPCID.DungeonSlime,
            NPCID.GoldenSlime,
            NPCID.GreenSlime,
            NPCID.IceSlime,
            NPCID.JungleSlime,
            NPCID.IlluminantSlime,
            NPCID.LavaSlime,
            NPCID.MotherSlime,
            NPCID.PurpleSlime,
            NPCID.QueenSlimeMinionPink,
            NPCID.QueenSlimeMinionPurple,
            NPCID.RainbowSlime,
            NPCID.RedSlime,
            NPCID.SandSlime,
            NPCID.SpikedIceSlime,
            NPCID.SpikedJungleSlime,
            NPCID.UmbrellaSlime,
            NPCID.YellowSlime,
            NPCID.Crimslime,
            NPCID.BigCrimslime,
            NPCID.LittleCrimslime,
            NPCID.ToxicSludge
        );

        public int Counter;

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            //slimes target nearest player on spawn
            npc.TargetClosest(true);

            if (NPC.AnyNPCs(NPCID.KingSlime)) //no KS minions in emode
            {
                npc.active = false;
                return;
            }

            if (npc.type == NPCID.JungleSlime && Main.rand.NextBool(5))
                npc.Transform(NPCID.SpikedJungleSlime);

            if (npc.type == NPCID.IceSlime && Main.rand.NextBool(5))
                npc.Transform(NPCID.SpikedIceSlime);

            if (npc.type == NPCID.BlueSlime)
            {
                if (Main.rand.NextBool(500))
                {
                    npc.Transform(NPCID.GoldenSlime);
                }
                else if (Main.slimeRain)
                {
                    if (Main.rand.NextBool(8))
                    {
                        npc.SetDefaults(Main.rand.Next(new int[] {
                            NPCID.RedSlime,
                            NPCID.PurpleSlime,
                            NPCID.YellowSlime,
                            NPCID.BlackSlime,
                            NPCID.SlimeSpiked
                        }));

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                    /*else if (Main.rand.NextBool(50))
                    {
                        npc.SetDefaults(NPCID.Pinky);

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }*/
                }
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.BlueSlime && npc.netID == NPCID.Pinky)
            {
                //always be bouncing
                npc.ai[0] = -2000;
            }

            if (npc.type == NPCID.UmbrellaSlime)
            {
                if (npc.wet)
                    Counter = 30;

                if (Counter > 0)
                    Counter--;

                if (Counter <= 0 && npc.velocity.Y > 0)
                    npc.velocity.Y /= 10;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 60);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (npc.type == NPCID.BlueSlime)
            {
                void SplitIntoSlimes(int type)
                {
                    if (Main.rand.NextBool(3) && FargoSoulsUtil.HostCheck)
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, type);
                }

                switch (npc.netID)
                {
                    case NPCID.YellowSlime:
                        SplitIntoSlimes(NPCID.PurpleSlime);
                        break;

                    case NPCID.PurpleSlime:
                        SplitIntoSlimes(NPCID.RedSlime);
                        break;

                    case NPCID.RedSlime:
                        SplitIntoSlimes(NPCID.GreenSlime);
                        break;

                    case NPCID.Pinky:
                        SplitIntoSlimes(NPCID.YellowSlime);
                        //SplitIntoSlimes(NPCID.MotherSlime);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
