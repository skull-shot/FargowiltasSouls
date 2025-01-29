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
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Globals;

namespace FargowiltasSouls.Content.NPCs
{
    [AutoloadHead]
    public class UnconsciousDeviantt : ModNPC
    {
        bool landsound = false;
        int slide;
        float number;
        int textpopup;
        int speedlerp;
        int questionmarkinterpolant;
        LoopedSoundInstance FallingLoop;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            this.ExcludeFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 250;
            NPC.height = 10;
            NPC.width = 30;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.noTileCollide = false;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("Mods.FargowiltasSouls.NPCs.UnconsciousDeviantt.UnconsciousChatButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    var packet = Mod.GetPacket();
                    packet.Write((byte)FargowiltasSouls.PacketID.WakeUpDeviantt);
                    packet.Write((byte)NPC.whoAmI);
                    packet.Send();
                }
                else
                    WakeUp(NPC);

                int Abom = NPC.FindFirstNPC(ModContent.NPCType<Abominationn>());
                int Mutant = NPC.FindFirstNPC(ModContent.NPCType<Mutant>());             

                Main.npcChatText = Language.GetTextValue("Mods.FargowiltasSouls.NPCs.UnconsciousDeviantt.Introduction", NPC.GivenName);

                if (Mutant != -1 || Abom != -1)
                {
                    Main.npcChatText = Language.GetTextValue("Mods.FargowiltasSouls.NPCs.UnconsciousDeviantt.IntroductionHasMetBros", NPC.GivenName);
                    }
                }
            }

        public static void WakeUp(NPC npc)
        {
            npc.Transform(ModContent.NPCType<Deviantt>());
            npc.velocity.Y += -3;
        }

        public override string GetChat()
        {
            return Language.GetTextValue("Mods.FargowiltasSouls.NPCs.UnconsciousDeviantt.Unconscious");
        }

        public override void ChatBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects)
        {
            position.Y -= 10;
            if (NPC.spriteDirection == -1)
            {
                position.X -= 5;
            }
            else
                position.X += 5;

            
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {   Particle p1 = new SmallSparkle(NPC.Center, Main.rand.NextVector2Circular(8, 8), Color.Pink, 1, 50, 0, 0, false);
                Particle p2 = new HeartParticle(NPC.Center, Main.rand.NextVector2Circular(12, 12), Color.Red, 1.5f, 35, false);
                p1.Spawn();
                p2.Spawn();
            }
                
            SoundEngine.PlaySound(FargosSoundRegistry.DeviTeleport with { Volume = 2f}, NPC.Center);
            int p = Player.FindClosest(NPC.Center, 3000, 3000);
            if (p.IsWithinBounds(Main.maxPlayers) && Main.player[p] is Player player && player.Alive() && NPC.Center.X < player.Center.X)
            {
                NPC.spriteDirection = 1;
                number = 5;
            }
            else
            {
                NPC.spriteDirection = -1;
                number = -5;
            }
                
                
            base.OnSpawn(source);
        }


        public override void AI()
        {
            //DrawOffsetY = 2;
            if (NPC.ai[0] == 0)
            {
                float velocity = MathHelper.Lerp(0, 0.2f, ++speedlerp * 0.003f);
                if (velocity >= 0.2f)
                {
                    velocity = 0.2f;
                }
                NPC.velocity.X += velocity * NPC.spriteDirection;
                NPC.netUpdate = true;
                FallingLoop ??= LoopedSoundManager.CreateNew(FargosSoundRegistry.DeviFallLoop, () =>
                {
                    return !NPC.active || NPC.ai[0] != 0;
                });

                FallingLoop?.Update(NPC.Center, sound =>
                {
                    sound.Volume = 2f;
                });

                //CameraPanSystem.PanTowards(NPC.Center, ++camerapaninterpolant * 0.004f);
                NPC.velocity.X += 0.05f * NPC.spriteDirection;
                if (++textpopup >= 10)
                {
                    textpopup = 0;
                    int i = CombatText.NewText(NPC.Hitbox, Color.Pink, "A", true);
                   
                }
            }
            if (NPC.ai[0] == 1)
            {
                NPC.velocity.X = MathHelper.Lerp(number, 0, ++slide * 0.07f);
                if (NPC.velocity.X >= 0 && number == -5)
                {
                    NPC.velocity.X = 0;
                }

                if (NPC.velocity.X <= 0 && number == 5)
                {
                    NPC.velocity.X = 0;
                }
            }

            //wake up on her own if it becomes nighttime
            if (Main.dayTime == false)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    var packet = Mod.GetPacket();
                    packet.Write((byte)FargowiltasSouls.PacketID.WakeUpDeviantt);
                    packet.Write((byte)NPC.whoAmI);
                    packet.Send();
                }
                else
                    WakeUp(NPC);
            }

            if (NPC.velocity.Y == 0)
            {
                if (landsound == false)
                {
                    FargoSoulsUtil.ScreenshakeRumble(5);
                    NPC.life -= 1;
                    CombatText.NewText(NPC.Hitbox, Color.Red, "1", false, true);
                    SoundEngine.PlaySound(new SoundStyle($"FargowiltasSouls/Assets/Sounds/SqueakyToy/squeak{Main.rand.Next(1, 7)}") with { Volume = 0.1f }, NPC.Center);
                    SoundEngine.PlaySound(FargosSoundRegistry.DeviFloorImpact, NPC.Center);
                    NPC.ai[0] = 1;
                    landsound = true;
                    Gore gore = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(5, 0), Main.rand.Next(11, 14), Scale: 0.8f);
                    Gore gore1 = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(-5, 0), Main.rand.Next(11, 14), Scale: 0.8f);
                }
            }
        }      

        public override void FindFrame(int frameHeight)
        {   
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 0 * frameHeight;
                NPC.rotation += 0.1f * NPC.spriteDirection;
            }
            if (NPC.velocity.Y == 0)
            {
                NPC.frame.Y = 1 * frameHeight;
                NPC.rotation = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 bluh = new Vector2(0, (float)Math.Sin(Main.GameUpdateCount / 90f * MathHelper.TwoPi) * 2f);
            Texture2D QuestionMark = ModContent.Request<Texture2D>("FargowiltasSouls/Content/NPCs/UnconsciousDeviantt_Head", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rectangle = new(0, 0, QuestionMark.Width, QuestionMark.Height);
            if (NPC.ai[0] == 1)
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
                spriteBatch.Draw(QuestionMark, NPC.Center + bluh - Main.screenPosition + new Vector2(0, -50), new Rectangle?(rectangle), Color.White * opacity, 0, origin2, scale, SpriteEffects.None, 0);
            }
           
            base.PostDraw(spriteBatch, screenPos, drawColor);
        }



    }
}
