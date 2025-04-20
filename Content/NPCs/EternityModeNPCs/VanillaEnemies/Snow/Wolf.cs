using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Projectiles.Masomode.Enemies.Vanilla.Snow;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Snow
{
    public class Wolf : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Wolf);

        public int AttackTimer;
        public Vector2 targetPos;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            AttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);
            if (Main.rand.NextBool(3) && npc.FargoSouls().CanHordeSplit)
                EModeGlobalNPC.Horde(npc, Main.rand.Next(10) + 1);
        }

        public override bool SafePreAI(NPC npc)
        {
            AttackTimer++;
            int timeToCharge = 220;
            if (AttackTimer >= 180)
            {
                if (AttackTimer == 180)
                {
                    FargoSoulsUtil.DustRing(npc.Center, 30, DustID.FrostStaff, 7f);
                    SoundEngine.PlaySound(SoundID.NPCHit6 with { Pitch = -0.9f }, npc.Center);
                }
                else if (AttackTimer < timeToCharge)
                {
                    npc.velocity *= 0.98f;
                }
                else if (AttackTimer == timeToCharge && npc.HasValidTarget && Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    Vector2 targetPoint = Main.player[npc.target].Center - Vector2.UnitY * 200;
                    float distanceScale = MathHelper.Clamp(npc.Distance(targetPoint) / 1000f, 0f, 1f);
                    float vel = 5f + 20f * distanceScale;
                    npc.velocity = npc.DirectionTo(targetPoint) * vel;
                    SoundEngine.PlaySound(FargosSoundRegistry.ThrowShort with { Pitch = 0.5f }, npc.Center);
                }
                else if (AttackTimer >= timeToCharge)
                {
                    if (npc.velocity.Y == 0)
                    {
                        AttackTimer = 0;
                    }
                    else
                    {
                        if (AttackTimer % 4 == 0 && FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<SnowMist>(), npc.damage / 8, 0f);
                        }
                        npc.velocity.Y += 0.01f;
                    }
                }
                return false;
            }

            return base.SafePreAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Bleeding, 300);
            target.AddBuff(BuffID.Rabies, 900);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}
