using Fargowiltas.NPCs;
using FargowiltasSouls.Assets.Particles;
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Common.Graphics.Particles;
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

namespace FargowiltasSouls.Content.NPCs
{
    public class UnconsciousDeviantt : ModNPC
    {
        bool landsound = false;
        int slide;
        float number;
        int textpopup;
        int camerapaninterpolant;
        LoopedSoundInstance FallingLoop;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 100;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.noTileCollide = false;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Wake her up?";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.npcChatText = Language.GetTextValue("Mods.Fargowiltas.NPCs.Deviantt.Chat.Introduction");
                NPC.Transform(ModContent.NPCType<Deviantt>());
            }
        }

        public override string GetChat()
        {
            return Language.GetTextValue("Mods.Fargowiltas.NPCs.Deviantt.Chat.Unconscious");
        }

        public override void ChatBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects)
        {
            if (NPC.spriteDirection == -1)
            {
                position.X -= 20;
            }
            else
                position.X += 20;
            
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {   Particle p = new SmallSparkle(NPC.Center, Main.rand.NextVector2Circular(8, 8), Color.Pink, 1, 50, 0, 0, false);
                Particle p2 = new HeartParticle(NPC.Center, Main.rand.NextVector2Circular(12, 12), Color.Red, 1.5f, 35, false);
                p.Spawn();
                p2.Spawn();
            }
                
            SoundEngine.PlaySound(FargosSoundRegistry.DeviTeleport with { Volume = 2f}, NPC.Center);
            if (NPC.Center.X < Main.LocalPlayer.Center.X)
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

            DrawOffsetY = 3;
            if (NPC.ai[0] == 0)
            {
                MusicFade();
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
                if (++textpopup >= 5)
                {
                    textpopup = 0;
                    CombatText.NewText(NPC.Hitbox, Color.Pink, "A", true);
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

           
            
        }

        public void MusicFade()
        {
            if (NPC.ai[0] == 0)
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 0, 0.05f);
            }
            else
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 1, 0.02f);
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
                if (landsound == false)
                {
                    FargoSoulsUtil.ScreenshakeRumble(5);
                    SoundEngine.PlaySound(FargosSoundRegistry.DeviFloorImpact, NPC.Center);
                    NPC.ai[0] = 1;
                    landsound = true;
                    Gore gore = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(5, 0), Main.rand.Next(11, 14), Scale: 0.8f);
                    Gore gore1 = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(-5,0), Main.rand.Next(11, 14), Scale: 0.8f);
                }            
                NPC.frame.Y = 1 * frameHeight;
                NPC.rotation = 0;
            }
        }



    }
}
