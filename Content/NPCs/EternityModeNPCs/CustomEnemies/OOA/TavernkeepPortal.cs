using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.CustomEnemies.OOA
{
    public class TavernkeepPortal : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_549";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            NPCID.Sets.BelongsToInvasionOldOnesArmy[Type] = true;
            this.ExcludeFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 120;
            NPC.lifeMax = 1000;
            NPC.ShowNameOnHover = false;
            NPC.dontTakeDamage = true;
        }

        public ref float state => ref NPC.ai[0];
        public ref float timer => ref NPC.ai[1];

        public override void AI()
        {
            timer++;
            Lighting.AddLight(NPC.Center, TorchID.Purple);
            if (state == 0)
            {
                if (timer > 60)
                {
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
                    timer = 0;
                    state = 1;
                }
                else
                {
                    Dust.NewDustPerfect(NPC.Center + 10 * Vector2.UnitY, DustID.Shadowflame, 1.5f * Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi), Scale: 0.4f);
                }
            }
            else if (state == 1)
            {
                NPC.TargetClosest();
                float scale = timer / 60f;
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, Scale: scale);

                if (timer > 60)
                {
                    state = 2;
                    timer = 0;
                }
            }
            else if (state == 2)
            {
                if (timer == 60)
                    SoundEngine.PlaySound(SoundID.DD2_OgreSpit, NPC.Center);

                if (timer == 140)
                    SoundEngine.PlaySound(SoundID.DD2_OgreAttack, NPC.Center);

                if (timer == 152 && FargoSoulsUtil.HostCheck)
                {
                    int tavernkeep = FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center + 60 * Vector2.UnitX,
                        NPCID.BartenderUnconscious, velocity: 30 * Vector2.UnitX);
                    if (tavernkeep < 200)
                        Main.npc[tavernkeep].AddBuff(BuffID.OgreSpit, 600);
                }

                if (timer >= 260)
                {
                    state = 3;
                    timer = 0;
                }
            }
            else if (state == 3)
            {
                float scale = (60 - timer) / 60f;
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, Scale: scale);

                if (timer > 60)
                {
                    NPC.active = false;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ > 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= 8 * frameHeight)
                    NPC.frame.Y = 0;

                NPC.ai[2]++;
                if (NPC.ai[2] > 6)
                    NPC.ai[2] = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D text = TextureAssets.Npc[Type].Value;
            Vector2 origin2 = NPC.frame.Size() / 2;

            float scale = NPC.scale;
            switch(state)
            {
                case 0:
                    scale *= 0f;
                    break;
                case 1:
                    scale *= MathHelper.Clamp(timer / 60, 0f, 1f);
                    break;
                case 3:
                    scale *= MathHelper.Clamp((60 - timer) / 60, 0f, 1f);
                    break;
                default:
                    break;
            }

            float opac = 1;
            if (state == 3)
                opac *= MathHelper.Clamp((60 - timer) / 60, 0f, 1f);

            Main.EntitySpriteDraw(text, NPC.Center - screenPos - (scale * NPC.height / 4) * Vector2.UnitY, NPC.frame, Color.Pink * opac, 0, origin2, scale, SpriteEffects.None);

            Texture2D eyeText = TextureAssets.Extra[ExtrasID.DD2ElderEye].Value;
            int frameHeight = eyeText.Height / 8;
            Rectangle eyeFrame = new Rectangle(0, (int)NPC.ai[2] * frameHeight, eyeText.Width, frameHeight);
            Vector2 eyeOrigin = eyeFrame.Size() / 2;
            Vector2 eyeOffset = (NPC.height / 4) * Vector2.UnitY;
            eyeOffset.Y *= 4 * scale;

            Main.EntitySpriteDraw(eyeText, NPC.Center - screenPos - eyeOffset, eyeFrame, Color.Pink * opac, 0, eyeOrigin, 0.25f + (opac * scale / 2), SpriteEffects.None);

            if (state == 2)
                AnimateOgre(screenPos, drawColor);

            return false;
        }

        public void AnimateOgre(Vector2 screenPos, Color drawColor)
        {
            float maxTime = 200f;
            int type = NPCID.DD2OgreT2;
            int frameY = (int)Math.Floor(timer / 8) % 11;

            if (timer >= 60 && timer <= 108)
            {
                // spit
                frameY = 22 + (int)Math.Floor((decimal)(timer - 60) / 8);
            }
            else if (timer > 108 && timer < 140)
            {
                frameY = 30;
            }
            else if (timer >= 140 && timer <= 172)
            {
                // throw (8 frames)
                frameY = 31 + (int)Math.Floor((decimal)(timer - 140) / 4);
            }
            else if (timer > 172 && timer < maxTime)
            {
                frameY = 0;
            }

            int spriteDirection = NPC.direction;

            Vector2 offset = Vector2.UnitX;
            if (timer < 60)
            {
                offset *= MathHelper.Clamp(timer / 2, 0, 30);
            }
            else if (timer < maxTime)
            {
                offset *= 30;
            }
            else
            {
                spriteDirection *= -1;
                offset *= MathHelper.Clamp((maxTime + 60 - timer) / 2, 0, 30);
            }

            Vector2 halfSize = new Vector2(TextureAssets.Npc[type].Width() / 2, TextureAssets.Npc[type].Height() / Main.npcFrameCount[type] / 2);
            float num36 = Main.NPCAddHeight(NPC);
            SpriteEffects flip = spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D value17 = TextureAssets.Npc[type].Value;
            Vector2 vector18 = NPC.Bottom - screenPos;
            Rectangle rectangle7 = value17.Frame(5, 10, frameY / 10, frameY % 10);
            Vector2 origin8 = rectangle7.Size() * new Vector2(0.5f, 1f);
            origin8.Y -= 4f;
            int num60 = 94;
            if (spriteDirection == 1)
            {
                origin8.X = num60;
            }
            else
            {
                origin8.X = rectangle7.Width - num60;
            }
            Color value18 = Color.White;
            float amount4 = 0f;
            int num61 = 0;
            float num62 = 0f;
            Color color18 = drawColor;
            if (timer < 60f)
            {
                float num63 = timer / 60f;
                num61 = 3;
                num62 = 1f - num63 * num63;
                value18 = new Color(127, 0, 255, 0);
                amount4 = 1f;
                color18 = Color.Lerp(Color.Transparent, color18, num63 * num63);
            }
            for (int num64 = 0; num64 < num61; num64++)
            {
                Color value19 = drawColor;
                value19 = Color.Lerp(value19, value18, 0);
                value19 = drawColor;
                value19 = Color.Lerp(value19, value18, amount4);
                value19 *= 1f - num62;
                Vector2 position10 = vector18;
                position10 -= new Vector2(value17.Width, value17.Height / Main.npcFrameCount[type]) * 1 / 2f;
                position10 += halfSize * 1 + new Vector2(0f, num36);
                Main.EntitySpriteDraw(value17, position10 + offset, rectangle7, value19, 0, origin8, 1, flip, 0f);
            }

            if (timer > maxTime)
            {
                float num63 = (60 + maxTime - timer) / 60f;
                num61 = 3;
                num62 = 1f - num63 * num63;
                value18 = new Color(127, 0, 255, 0);
                amount4 = 1f;
                color18 = Color.Lerp(Color.Transparent, color18, num63 * num63);
            }
            for (int num64 = 0; num64 < num61; num64++)
            {
                Color value19 = drawColor;
                value19 = Color.Lerp(value19, value18, 0);
                value19 = drawColor;
                value19 = Color.Lerp(value19, value18, amount4);
                value19 *= 1f - num62;
                Vector2 position10 = vector18;
                position10 -= new Vector2(value17.Width, value17.Height / Main.npcFrameCount[type]) * 1 / 2f;
                position10 += halfSize * 1 + new Vector2(0f, num36);
                Main.EntitySpriteDraw(value17, position10 + offset, rectangle7, value19, 0, origin8, 1, flip, 0f);
            }

            Main.EntitySpriteDraw(value17, vector18 + offset, rectangle7, color18, 0, origin8, 1, flip, 0f);
        }
    }
}
