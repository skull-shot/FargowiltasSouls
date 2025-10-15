﻿using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Life
{
    public class LesserFairy : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_75";

        public int counter;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lesser Fairy");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "小精灵");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.Pixie];
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(
                ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<LifeChampion>()],
                quickUnlock: true
            );
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            ]);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.damage = 180;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;

            AnimationType = NPCID.Pixie;
            NPC.aiStyle = -1;

            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemTopaz);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }

            if (Main.rand.NextBool(40))
            {
                SoundEngine.PlaySound(SoundID.Pixie, NPC.Center);
            }

            NPC.direction = NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
            NPC.rotation = NPC.velocity.X * 0.1f;

            if (++counter > 60 && counter < 240)
            {
                if (!NPC.HasValidTarget)
                    NPC.TargetClosest();

                if (NPC.Distance(Main.player[NPC.target].Center) < 300)
                {
                    NPC.velocity = NPC.velocity.RotateTowards(NPC.SafeDirectionTo(Main.player[NPC.target].Center).ToRotation(), 0.04f);
                }
            }
            else if (counter > 300)
            {
                NPC.SimpleStrikeNPC(int.MaxValue, 0, false, 0, null, false, 0, true);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {

        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 4)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }

            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
            {
                NPC.frame.Y = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemTopaz, 0f, 0f, 0, default, 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[Type].Value;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f;

                DrawData data = new(texture, NPC.Center + afterimageOffset - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), NPC.frame, color26, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.DeepPink).UseSecondaryColor(Color.Pink);
                GameShaders.Misc["LCWingShader"].Apply(data);
                data.Draw(Main.spriteBatch);
            }
            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), NPC.frame, color26, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}
