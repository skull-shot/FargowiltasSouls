using FargowiltasSouls.Content.Projectiles.Eternity.Enemies.Vanilla.Dungeon;
using FargowiltasSouls.Core.NPCMatching;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class Diabolist : DungeonTeleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.DiabolistRed, NPCID.DiabolistWhite);

        public int AttackTimer;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Textures/AdditiveTextures/Bloom");
            if (AttackTimer >= 2)
            {
                spriteBatch.UseBlendState(BlendState.Additive);
                float opacity = 1;
                if (AttackTimer > 300)
                {
                    opacity = MathHelper.Lerp(1, 0, (AttackTimer - 300) / 60f);
                }
                spriteBatch.Draw(glow.Value, npc.Center - Main.screenPosition,null, Color.Orange * (MathHelper.Lerp(0, 0.3f, AttackTimer / 300f)) * opacity, 0, glow.Size()/2, 8.3f, SpriteEffects.None, 1);
                spriteBatch.UseBlendState(BlendState.AlphaBlend);
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void AI(NPC npc)
        {
            
            int radius = 450;
            if (npc.ai[0] == 2)
            {
                AttackTimer++;
            }
            int maxtime = 300;
            if (npc.ai[0] < 30 && AttackTimer >= 1)
            {
                //prevents diabolist tp'ing on hit
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                //prevent teleport by cooldown
                TeleportTimer = 0;
                npc.GetGlobalNPC<DungeonTeleporters>().TeleportTimer = 0;
                npc.ai[0] = 3;

                float x = 1 - MathF.Pow(1 - (AttackTimer / (float)maxtime), 3);
                //purely visual do not need to exist server side
                if (AttackTimer < maxtime && AttackTimer % (int)MathHelper.Lerp(20, 3, x) == 0 && !Main.dedServ)
                {
                    Vector2 pos = //LumUtils.FindGroundVertical((npc.Center + new Vector2(Main.rand.Next(-radius, radius), 0)).ToTileCoordinates()).ToWorldCoordinates();
                        npc.Center + new Vector2(0, Main.rand.Next(0, radius)).RotatedByRandom(MathHelper.TwoPi);
                    if (!Collision.SolidCollision(pos, 1, 1))
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<FireEffect>(), 0, 0);
                }
                //would netspam and they die quickly so i think its safe to have the server not care about these
                if (AttackTimer > maxtime && !Main.dedServ)
                {
                    Vector2 pos = npc.Center + new Vector2(0, Main.rand.Next(20, radius)).RotatedByRandom(MathHelper.TwoPi);
                    if (AttackTimer % 3 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item14 with { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 1f }, pos);
                    }
                    if (!Collision.SolidCollision(pos, 1, 1))
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<DiabolistExplosion>(), FargoSoulsUtil.ScaledProjectileDamage(75), 1);
                }
                AttackTimer++;
                if (AttackTimer <= maxtime * 0.8f)
                    npc.ai[1] = 0;
                else
                    npc.ai[1] = 1;
                if (AttackTimer >= maxtime + 60)
                {
                    AttackTimer = 0;
                }
            }
            else
            {
                npc.ai[1] = 0;
            }
            base.AI(npc);
        }
    }
}
