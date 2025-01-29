using Fargowiltas.NPCs;
using FargowiltasSouls.Assets.Particles;
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core;
using Luminance.Core.Graphics;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Core.Systems;

namespace FargowiltasSouls.Content.NPCs
{
    [AutoloadHead]
    public class ReleasedMutant : ModNPC
    {
        bool landsound = false;
        int slide;
        float number;
        int questionmarkinterpolant;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            this.ExcludeFromBestiary();
        }

        public override void ChatBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects)
        {

            position.Y += 8;
            if (NPC.spriteDirection == -1)
            {
                position.X += -10;
            }
            else
                position.X += 10;
            base.ChatBubblePosition(ref position, ref spriteEffects);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 250;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.noTileCollide = false;
            NPC.height = 46;
            NPC.width = 18;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("Mods.FargowiltasSouls.NPCs.ReleasedMutant.TalkToHim");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    var packet = Mod.GetPacket();
                    packet.Write((byte)FargowiltasSouls.PacketID.WakeUpMutant);
                    packet.Write((byte)NPC.whoAmI);
                    packet.Send();

                    WorldSavingSystem.HaveForcedMutantFromKS = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
                else
                {
                    NPC.Transform(ModContent.NPCType<Mutant>());
                    WorldSavingSystem.HaveForcedMutantFromKS = true;
                }
                    
                int mutant = NPC.FindFirstNPC(ModContent.NPCType<Mutant>());
                if (mutant != -1)
                {
                    Main.npcChatText = Language.GetTextValue("Mods.FargowiltasSouls.NPCs.ReleasedMutant.Introduction", Main.npc[mutant].GivenName);
                }
                
            }
        }

        public override string GetChat()
        {
            return Language.GetTextValue("Mods.FargowiltasSouls.NPCs.ReleasedMutant.Chat");
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (NPC.Center.X < Main.LocalPlayer.Center.X)
            {
                NPC.spriteDirection = 1;
                NPC.velocity += new Vector2(4f, -10);
                number = 3;
            }
            else
            {
                NPC.spriteDirection = -1;
                NPC.velocity += new Vector2(-4f, -10);
                number = -3;
            }


            base.OnSpawn(source);
        }


        public override void AI()
        {
            DrawOffsetY = -2;
            

            if (NPC.ai[0] == 1)
            {
                NPC.velocity.X = MathHelper.Lerp(number, 0, ++slide * 0.02f);
                NPC.netUpdate = true;
                if (NPC.velocity.X >= 0 && number == -3)
                {
                    NPC.velocity.X = 0;
                    NPC.ai[0] = 2;

                }

                if (NPC.velocity.X <= 0 && number == 3)
                {
                    NPC.velocity.X = 0;
                    NPC.ai[0] = 2;
                }
            }
            if (NPC.ai[0] == 2)
            {   
                if (NPC.ai[1] >= 30)
                {
                    if (NPC.Center.X < Main.LocalPlayer.Center.X)
                    {
                        NPC.spriteDirection = 1;
                    }
                    else
                    {
                        NPC.spriteDirection = -1;
                    }
                }

                int p = Player.FindClosest(NPC.Center, 3000, 3000);
                if (p.IsWithinBounds(Main.maxPlayers) && Main.player[p] is Player player && player.Alive() && NPC.Distance(player.Center) >= 1500)
                {
                    NPC.Transform(ModContent.NPCType<Mutant>());
                    WorldSavingSystem.HaveForcedMutantFromKS = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
                
            }

            if (NPC.velocity.Y == 0 && landsound == false)
            {
                FargoSoulsUtil.ScreenshakeRumble(5);
                SoundEngine.PlaySound(FargosSoundRegistry.MutantLand, NPC.Center);
                NPC.ai[0] = 1;
                landsound = true;
                Gore gore = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(5, 0), Main.rand.Next(11, 14), Scale: 0.8f);
                Gore gore1 = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(-5, 0), Main.rand.Next(11, 14), Scale: 0.8f);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            if (NPC.velocity.Y == 0)
            {
                NPC.frame.Y = 1 * frameHeight;
                NPC.rotation = 0;
            }

            if (NPC.ai[0] == 2 && ++NPC.ai[1] >= 15)
            {
                NPC.frame.Y = 2 * frameHeight;               
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 bluh = new Vector2(0, (float)Math.Sin(Main.GameUpdateCount / 90f * MathHelper.TwoPi) * 2f);
            Texture2D QuestionMark = ModContent.Request<Texture2D>("FargowiltasSouls/Content/NPCs/ReleasedMutant_Head", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rectangle = new(0, 0, QuestionMark.Width, QuestionMark.Height);
            if (NPC.ai[0] == 2 && NPC.ai[1] >= 15)
            {
                float opacity = MathHelper.Lerp(0, 1f, ++questionmarkinterpolant * 0.08f);
                if (opacity >= 1f)
                {
                    opacity = 1f;
                }

                float scale = MathHelper.Lerp(0, 1f, questionmarkinterpolant * 0.04f);
                if (scale >= 1f)
                {
                    scale = 1f;
                }

                Vector2 origin2 = rectangle.Size() / 2f;
                    spriteBatch.Draw(QuestionMark, NPC.Center + bluh - Main.screenPosition + new Vector2(5 * NPC.spriteDirection, -45), new Rectangle?(rectangle), Color.White * opacity, 0, origin2, scale, SpriteEffects.None, 0);
            }
            
            base.PostDraw(spriteBatch, screenPos, drawColor);
        }



    }
}
