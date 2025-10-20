using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Cavern;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class GraniteElemental : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GraniteFlyer);

        public int AttackTimer = 0;
        public int DamageStored = 0;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write7BitEncodedInt(DamageStored);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            AttackTimer = binaryReader.Read7BitEncodedInt();
            DamageStored = binaryReader.Read7BitEncodedInt();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            // Absorb Projectiles
            if (npc.dontTakeDamage)
            {
                float distance = 2f * 16;

                Main.projectile.Where(x => EModeGlobalProjectile.CanBeAbsorbed(x)).ToList().ForEach(x =>
                {
                    if (Vector2.Distance(x.Center, npc.Center) <= distance)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.BlueTorch, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, default, 1.5f);
                            Main.dust[dustId].noGravity = true;
                        }

                        SoundEngine.PlaySound(SoundID.MaxMana, npc.Center);
                        DamageStored += x.damage;
                        x.active = false;
                    }
                });
            }
            // EModeGlobalNPC.CustomReflect(npc, DustID.Granite, 2);
        }

        public override void SafePostAI(NPC npc)
        {
            base.SafePostAI(npc);
            if (npc.dontTakeDamage)
            {
                const int timeToAttack = 50;

                npc.velocity *= AttackTimer >= timeToAttack ? 0.99f : 0f;
                npc.noGravity = true;
                AttackTimer++;
                if (AttackTimer == timeToAttack - 25)
                {
                    SoundEngine.PlaySound(SoundID.Item91, npc.Center);
                    FargoSoulsUtil.DustRing(npc.Center, 20, 180, 5f, scale: 2f);
                }
                if (AttackTimer == timeToAttack && npc.HasValidTarget)
                {
                    SoundEngine.PlaySound(SoundID.Item72, npc.Center);
                    Vector2 vel = (Main.player[npc.target].Center - npc.Center);
                    vel.Normalize();
                    if (FargoSoulsUtil.HostCheck)
                    {
                        float scale = 1 + (DamageStored / 80f);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, 3f * vel, ModContent.ProjectileType<GraniteBolt>(), (int)(scale * npc.damage / 6), 0f);
                    }
                    npc.velocity = -vel;
                }
            }
            else
            {
                DamageStored = 0;
                AttackTimer = 0;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            //target.FargoSouls().AddBuffNoStack(BuffID.Stoned, 60);
        }
    }
}
