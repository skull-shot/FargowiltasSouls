using Fargowiltas.Content.Items.Tiles;
using Fargowiltas.Content.Projectiles;
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Snow
{
    public class IcyMerman : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.IcyMerman);

        public int State = 0;
        public Vector2 targetPos;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(State);
            binaryWriter.WriteVector2(targetPos);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            State = binaryReader.Read7BitEncodedInt();
            targetPos = binaryReader.ReadVector2();
        }

        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }

        public override void OnFirstTick(NPC npc)
        {
            // Immune to water physics
            npc.waterMovementSpeed = 1f;
            npc.buffImmune[ModContent.BuffType<HypothermiaBuff>()] = true;
 
            if (!npc.wet)
            {
                State = 3;
                return;
            }

            // round npc.Top down to the nearest multiple of 16
            targetPos = new (npc.Top.X - (npc.Top.X%16), npc.Top.Y - (npc.Top.Y % 16));
            Tile t = Framing.GetTileSafely(targetPos);
            // Find the yPos where the water ends
            while (t.LiquidAmount > 0)
            {
                t = Framing.GetTileSafely(targetPos);
                if (t.TileType == TileID.BreakableIce)
                    break;
                targetPos.Y -= 16;
            }


        }

        public override bool SafePreAI(NPC npc)
        {
            npc.TargetClosest();
            switch (State)
            {
                // Hiding
                case 0:
                    if (!npc.wet)
                    {
                        State = 2;
                        break;
                    }
                    if (!npc.HasValidTarget)
                        break;

                    Player target = Main.player[npc.target];
                    if (npc.life < npc.lifeMax)
                    {
                        targetPos = target.Center;
                        State = 1;
                        break;
                    }

                    float attackDist = 130;
                    bool inRange = Math.Abs(target.Center.X - npc.Center.X) < attackDist;
                    if (inRange)
                    {
                        if (canStrike(target))
                        {
                            destroyIce(target.Bottom.Y - (target.Bottom.Y % 16));
                            targetPos = target.Center;
                            State = 1;
                        }
                        else if (Collision.CanHitLine(npc.Center, 1, 1, target.Center, 1, 1))
                        {
                            targetPos = target.Center;
                            State = 1;
                        }
                    }
                    break;
                // Lunging
                case 1:
                    FargoSoulsUtil.DustRing(npc.Center, 20, DustID.Frost, 5f);
                    Vector2 targetPoint = targetPos - Vector2.UnitY * 100;
                    float distanceScale = MathHelper.Clamp(npc.Distance(targetPoint) / 1000f, 0f, 1f);
                    float vel = 7f + 20f * distanceScale;
                    npc.velocity = npc.DirectionTo(targetPoint) * vel;
                    SoundStyle sound = Main.rand.NextFromList(SoundID.Zombie21, SoundID.Zombie22, SoundID.Zombie23);
                    SoundEngine.PlaySound(sound, npc.Center);
                    State = 2;
                    break;
                // Completing Lunge
                case 2:
                    if (npc.velocity.Y > 0)
                        State = 3;
                    break;
                // Normal AI
                case 3:
                    return base.SafePreAI(npc);
            }
            return false;
        }

        public bool canStrike(Player player)
        {
            if (!player.active || player.dead)
                return false;

            // Check that the player is standing on breakable ice and the layer found in first tick matches the layer the player is standing on
            Tile bottom = Framing.GetTileSafely(player.Bottom);
            float y1 = player.Bottom.Y - (player.Bottom.Y % 16);

            if (bottom.TileType != TileID.BreakableIce)
                return false;

            float y2 = targetPos.Y;
            return y1 == y2;
        }

        public void destroyIce(float yPos)
        {
            if (!FargoSoulsUtil.HostCheck)
                return;

            // Iterate to the right
            int yPosition = (int) (yPos / 16f);
            int x = 0;
            while (true)
            {
                int xPosition = (int)(x + (targetPos.X / 16f));

                if (xPosition < 0 || xPosition >= Main.maxTilesX || yPosition < 0 || yPosition >= Main.maxTilesY)
                    break;

                Tile t = Main.tile[xPosition, yPosition];
                if (t == null)
                    break;

                if (!FargoGlobalProjectile.OkayToDestroyTileAt(xPosition, yPosition) || FargoGlobalProjectile.TileIsLiterallyAir(t))
                    break;

                if (t.TileType != TileID.BreakableIce)
                    break;
                else
                {
                    WorldGen.KillTile(xPosition, yPosition, noItem: true);
                    Vector2 blockPos = new(16 * xPosition, 16 * yPosition);
                    SoundEngine.PlaySound(SoundID.Item27, blockPos);
                    for (int i = 0; i < 5; i++)
                        Dust.NewDust(blockPos, 16, 16, DustID.Ice);
                }

                x++;
            }
            // Iterate to the left (x = 0 was covered by right iteration)
            x = -1;
            while (true)
            {
                int xPosition = (int)(x + (targetPos.X / 16f));

                if (xPosition < 0 || xPosition >= Main.maxTilesX || yPosition < 0 || yPosition >= Main.maxTilesY)
                    break;

                Tile t = Main.tile[xPosition, yPosition];
                if (t == null)
                    break;

                if (!FargoGlobalProjectile.OkayToDestroyTileAt(xPosition, yPosition) || FargoGlobalProjectile.TileIsLiterallyAir(t))
                    break;

                if (t.TileType != TileID.BreakableIce)
                    break;
                else
                {
                    WorldGen.KillTile(xPosition, yPosition, noItem: true);
                    Vector2 blockPos = new(16 * xPosition, 16 * yPosition);
                    SoundEngine.PlaySound(SoundID.Item27, blockPos);
                    for (int i = 0; i < 5; i++)
                        Dust.NewDust(blockPos, 16, 16, DustID.Ice);
                }

                x--;
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // reduced opacity when hiding
            if (State == 0)
            {
                Texture2D texture = TextureAssets.Npc[npc.type].Value;
                Rectangle frame = npc.frame;
                SpriteEffects flip = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 origin = frame.Size() / 2 + (5 * Vector2.UnitY);
                Main.EntitySpriteDraw(texture, npc.Center - screenPos, frame, drawColor * 0.25f, npc.rotation, origin, npc.scale, flip);
                return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            // merman should only spawn in water
            base.EditSpawnPool(pool, spawnInfo);
            if (!spawnInfo.Water)
                pool[NPCID.IcyMerman] = 0f;
        }
    }
}
