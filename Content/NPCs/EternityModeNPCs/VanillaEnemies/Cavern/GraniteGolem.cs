using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Cavern;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class GraniteGolem : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GraniteGolem);

        public int projCount = 0;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(projCount);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            projCount = binaryReader.Read7BitEncodedInt();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.ai[2] < 0f) // while shielding, absorb
            {
                float distance = 2f * 16;

                Main.projectile.Where(x => x.active && x.friendly && x.FargoSouls().DeletionImmuneRank == 0 && !FargoSoulsUtil.IsSummonDamage(x, false)).ToList().ForEach(x =>
                {
                    if (Vector2.Distance(x.Center, npc.Center) <= distance)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.BlueTorch, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, default, 1.5f);
                            Main.dust[dustId].noGravity = true;
                        }

                        SoundEngine.PlaySound(SoundID.MaxMana, npc.Center);
                        if (projCount < 10) {

                            if (FargoSoulsUtil.HostCheck)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GranitePebble>(), npc.damage / 6, 1f, ai0: npc.whoAmI, ai2: projCount);
                            }
                            projCount++;
                            List<Projectile> pebbles = Main.projectile.Where(y => y.active && y.type == ModContent.ProjectileType<GranitePebble>() && y.ai[0] == npc.whoAmI).ToList();
                            float spacing = (MathHelper.TwoPi) / pebbles.Count;
                            float startRot = pebbles[0].ai[1];
                            for (int i = 0; i < pebbles.Count; i++)
                            {
                                pebbles[i].ai[1] = startRot + (spacing * i);
                            }
                        }
                        x.Kill();
                    }
                });
            }
            else
            {
                projCount = 0;
            }
            // EModeGlobalNPC.CustomReflect(npc, DustID.Granite, 2);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            //target.FargoSouls().AddBuffNoStack(BuffID.Stoned, 60);
        }
    }
}
