using System.Linq;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Night
{
    public class Wraith : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Wraith);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            Main.projectile.Where(x => x.friendly && x.Alive() && x.FargoSouls().DeletionImmuneRank == 0 && !x.FargoSouls().IsOnHitSource).ToList().ForEach(x =>
            {
                if (Vector2.Distance(x.Center, npc.Center) <= 16 * 10)
                {
                    if (x.Eternity().beingWraithReflectBy < 0 || x.Eternity().beingWraithReflectBy == npc.whoAmI)
                    {
                        x.velocity += (Vector2.Normalize(x.Center - npc.Center) * x.velocity.Length()) / 10;
                        x.Eternity().beingWraithReflectBy = npc.whoAmI;

                        int d = Dust.NewDust(x.position, x.width, x.height, DustID.RedTorch, Alpha: 100, Scale: 2f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = Vector2.Zero;
                        Main.dust[d].velocity += Vector2.Normalize(x.Center - npc.Center) * 2;

                        //line of dust
                        Vector2 eyespos = npc.Center + new Vector2(10 * npc.spriteDirection, -10);
                        int length = (int)(eyespos - x.Center).Length() / 10;
                        Vector2 offset = Vector2.Normalize(eyespos - x.Center) * 10f;
                        for (int i = 0; i <= length; i++)
                        {
                            int w = Dust.NewDust(x.Center + offset * i, 0, 0, DustID.ViciousPowder, Scale: 0.5f);
                            Main.dust[w].noGravity = true;
                            Main.dust[w].velocity = Vector2.Zero;
                        }
                    }
                }
                else x.Eternity().beingWraithReflectBy = -1;
            });

            //EModeGlobalNPC.Aura(npc, 80, BuffID.Obstructed, false, DustID.Clentaminator_Red);
            //npc.aiStyle = NPCAIStyleID.Flying;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);
            target.AddBuff(ModContent.BuffType<UnluckyBuff>(), 60 * 30);
        }
    }
}
