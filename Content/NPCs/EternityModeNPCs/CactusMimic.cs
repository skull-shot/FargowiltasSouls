using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs
{
    public class CactusMimic : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.frame.Width = 48;
            NPC.frame.Height = 74;
            Main.npcFrameCount[Type] = 4;
            NPC.damage = 20;
            NPC.width = 12;
            NPC.height = 64;
            NPC.value = 50;
            NPC.aiStyle = -1;
            NPC.hide = true;
            NPC.knockBackResist = 0.6f;
            NPC.lifeMax = 80;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.Dig;
            
            //base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "FargowiltasSouls/Content/NPCs/EternityModeNPCs/CactusMimic_Bestiary",
                PortraitScale = 0f,
                Scale = 0.2f,
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.FargowiltasSouls.Bestiary.CactusMimic")
            });
            base.SetBestiary(database, bestiaryEntry);
        }
        public override int SpawnNPC(int tileX, int tileY)
        {
            Tile tile = Main.tile[tileX, tileY];
            int ai = 0;
            if (tile.HasTile)
            {
                if (tile.TileType == TileID.Ebonsand) ai = 1;
                if (tile.TileType == TileID.Pearlsand) ai = 2;
                if (tile.TileType == TileID.Crimsand) ai = 3;
            }
            return NPC.NewNPC(NPC.GetSource_NaturalSpawn(), tileX * 16, tileY * 16, NPC.type, ai2: ai);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int[] sand = [TileID.Sand, TileID.Ebonsand, TileID.Pearlsand, TileID.Crimsand];
            if (sand.Contains(spawnInfo.SpawnTileType))
            {
                return 0.7f;
            }
            return 0;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
            base.DrawBehind(index);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> t = TextureAssets.Npc[Type];
            Rectangle source = new(48 * (int)NPC.ai[2], NPC.frame.Y, 48, 74);
            if (NPC.ai[0] == 1)
            {
                source.Y += 74;
            }
            Rectangle jaw1 = new(12, 148, 12, 22);
            Rectangle jaw2 = new(24, 148, 12, 22);
            //corrupt
            if (NPC.ai[2] == 1)
            {
                jaw1.X += 46;
                jaw2.X += 46;
            }
            //hallow
            else if (NPC.ai[2] == 2)
            {
                jaw1.X += 96;
                jaw2.X += 96;
            }
            //crimson
            else if (NPC.ai[2] == 3)
            {
                jaw1.X += 146;
                jaw2.X += 146;
            }
            Vector2 offset = new(0, 6);
            spriteBatch.Draw(t.Value, NPC.Center - Main.screenPosition + offset.RotatedBy(NPC.rotation), source, drawColor, NPC.rotation, new Vector2(48, 74) / 2, NPC.scale, SpriteEffects.None, 0);
            if (NPC.ai[0] == 1)
            {
                float rotation = 0;
                if (NPC.localAI[0] < 20)
                {
                    float x = 1 - MathF.Pow(1 - (NPC.localAI[0] / 20), 4);
                    rotation = Utils.AngleLerp(0, MathHelper.ToRadians(30), x);
                }
                else
                {
                    float x = 1 - MathF.Cos((((NPC.localAI[0] - 20) / 10) * MathF.PI) / 2);
                    rotation = Utils.AngleLerp(MathHelper.ToRadians(30), 0, x);
                }
                spriteBatch.Draw(t.Value, NPC.Center - Main.screenPosition + new Vector2( -5, -8).RotatedBy(NPC.rotation), jaw1, drawColor, NPC.rotation - rotation, new Vector2(3, 21), NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(t.Value, NPC.Center - Main.screenPosition + new Vector2(5, -8).RotatedBy(NPC.rotation), jaw2, drawColor, NPC.rotation + rotation, new Vector2(9, 21), NPC.scale, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void AI()
        {
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (NPC.velocity.X != 0) NPC.velocity.X *= 0.95f;
                return;

            }
            Player target = Main.player[NPC.target];
            if (NPC.GetLifePercent() < 0.9f || (NPC.Distance(target.Center) < 200 && !target.invis) || NPC.ai[0] == 1)
            {
                if (NPC.ai[0] == 0)
                {
                    NPC.ai[0] = 1;
                    int dir = (target.Center.X > NPC.Center.X ? 1 : -1);
                    NPC.velocity.X = dir * 6;
                    NPC.velocity.Y = -4;
                    NPC.rotation += MathHelper.ToRadians(dir);
                    NPC.ai[1] = dir;
                    Vector2 center = NPC.Center;
                    NPC.width = 64;
                    NPC.height = 10;
                    NPC.Center = center;
                }
                float rotoffset = NPC.velocity.Y > 0 ? 20 : (NPC.velocity.Y < 0 ? -20 : 0);
                if (NPC.ai[1] == 1)
                {
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, MathHelper.ToRadians(90 + rotoffset * NPC.ai[1]), 0.06f);
                }
                else
                {
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, MathHelper.ToRadians(-90 + rotoffset * NPC.ai[1]), 0.06f);
                }
                if(Collision.SolidCollision(NPC.BottomLeft, NPC.width, 6, true))
                {
                    NPC.velocity.X *= 0.8f;
                    if (NPC.localAI[0] == 20)
                    {
                        int dir = (target.Center.X > NPC.Center.X ? 1 : -1);
                        NPC.velocity.Y = -4;
                        NPC.velocity.X = dir * 3;
                        if (NPC.ai[1] != dir)
                        {
                            NPC.rotation += MathHelper.ToRadians(180);
                            NPC.ai[1] = dir;
                        }
                    }

                    
                }
                NPC.localAI[0]++;
                if (NPC.localAI[0] >= 30)
                {
                    NPC.localAI[0] = 0;
                }
            }

                base.AI();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && NPC.ai[0] == 1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            //corrupt
            if (NPC.ai[2] == 1)
            {
                target.AddBuff(BuffID.CursedInferno, 120);
            }
            //crimson
            else if (NPC.ai[2] == 3)
            {
                target.AddBuff(BuffID.Ichor, 600);
            }
            base.OnHitPlayer(target, hurtInfo);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Cactus, 1, 2, 7));
            npcLoot.Add(ItemDropRule.Common(ItemID.PinkPricklyPear, 5));

            base.ModifyNPCLoot(npcLoot);
        }
        
    }
}
