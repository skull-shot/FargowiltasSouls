using System;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Martians
{
    public class MartianProbe : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MartianProbe);
        public override bool SafePreAI(NPC npc)
        {
            if (!NPC.downedGolemBoss)
            {
                SaucerBehavior(npc);
                return false; //prevent spawning event
            }
            else return base.SafePreAI(npc);
        }
        private void SaucerBehavior(NPC npc)
        {
            // vanilla aistyle80 copy paste, but without event trigger
            if (npc.ai[0] == 0f)
            {
                if (npc.direction == 0)
                {
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
                if (npc.collideX)
                {
                    npc.direction = -npc.direction;
                    npc.netUpdate = true;
                }
                npc.velocity.X = 3f * (float)npc.direction;
                Vector2 center24 = npc.Center;
                Point point10 = center24.ToTileCoordinates();
                int num1277 = 30;
                if (WorldGen.InWorld(point10.X, point10.Y, 30))
                {
                    for (int num1278 = 0; num1278 < 30; num1278++)
                    {
                        if (WorldGen.SolidTile(point10.X, point10.Y + num1278))
                        {
                            num1277 = num1278;
                            break;
                        }
                    }
                }
                if (num1277 < 15)
                {
                    npc.velocity.Y = Math.Max(npc.velocity.Y - 0.05f, -3.5f);
                }
                else if (num1277 < 20)
                {
                    npc.velocity.Y *= 0.95f;
                }
                else
                {
                    npc.velocity.Y = Math.Min(npc.velocity.Y + 0.05f, 1.5f);
                }
                float distanceToPlayer;
                int num1279 = npc.FindClosestPlayer(out distanceToPlayer);
                if (num1279 == -1 || Main.player[num1279].dead)
                {
                    return;
                }
                if (distanceToPlayer < 352f && Main.player[num1279].Center.Y > npc.Center.Y)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.ai[1]++;
                npc.velocity *= 0.95f;
                if (npc.ai[1] >= 60f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 2f;
                    int num1280 = npc.FindClosestPlayer();
                    if (num1280 != -1)
                    {
                        npc.ai[3] = ((Main.player[num1280].Center.X > npc.Center.X) ? (-1f) : 1f);
                    }
                    else
                    {
                        npc.ai[3] = 1f;
                    }
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.noTileCollide = true;
                npc.ai[1]++;
                npc.velocity.Y = Math.Max(npc.velocity.Y - 0.1f, -10f);
                npc.velocity.X = Math.Min(npc.velocity.X + npc.ai[3] * 0.05f, 4f);
                if ((npc.position.Y < (float)(-npc.height) || npc.ai[1] >= 180f) && Main.netMode != 1)
                {
                    Vector2 spawnpos = new Vector2(npc.Center.X, npc.Center.Y + 16*60);
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnpos, NPCID.MartianSaucerCore);
                    npc.active = false;
                    npc.netUpdate = true;
                }
            }
            Vector3 rgb = Color.SkyBlue.ToVector3();
            if (npc.ai[0] == 2f)
            {
                rgb = Color.Red.ToVector3();
            }
            rgb *= 0.65f;
            Lighting.AddLight(npc.Center, rgb);
        }
    }
}
