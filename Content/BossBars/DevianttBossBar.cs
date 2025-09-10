using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.BossBars
{
    public class DevianttBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;
        public override Asset<Texture2D> GetIconTexture(ref Microsoft.Xna.Framework.Rectangle? iconFrame)
        {   
           if (bossHeadIndex != -1)
           {
              return TextureAssets.NpcHeadBoss[bossHeadIndex];
           }
           return null;
        }

        public Asset<Texture2D> Frame;

        public Asset<Texture2D> Bars;

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            (Texture2D barTexture, Vector2 barCenter, _, _, Color iconColor, float life, float lifeMax, float shield, float shieldMax, float iconScale, bool showText, Vector2 textOffset) = drawParams;

            Frame = ModContent.Request<Texture2D>("FargowiltasSouls/Content/BossBars/DevianttFrame", AssetRequestMode.ImmediateLoad);
            Bars = ModContent.Request<Texture2D>("FargowiltasSouls/Content/BossBars/DevianttBars", AssetRequestMode.ImmediateLoad);

            int headTextureIndex = NPCID.Sets.BossHeadTextures[npc.type];
            if (headTextureIndex == -1)
            {
                NPCLoader.BossHeadSlot(npc, ref headTextureIndex);
                if (headTextureIndex == -1)
                    return false;
            }

            float lifeRatio = LumUtils.Saturate(life / lifeMax);

            Texture2D iconTexture = TextureAssets.NpcHeadBoss[headTextureIndex].Value;
            Rectangle iconFrame = iconTexture.Frame();

            Point barSize = new(456, 22);
            Point topLeftOffset = new(32, 24);

            Rectangle bgFrame = Bars.Frame(verticalFrames: 3, frameY: 0);
            bgFrame.Width = Bars.Width();

            Rectangle barFrame = Bars.Frame(verticalFrames: 3, frameY: 0);
            barFrame.X += topLeftOffset.X;
            barFrame.Y += topLeftOffset.Y;
            barFrame.Width = (int)(Bars.Width() * lifeRatio);
            barFrame.Height = barSize.Y;

            Rectangle barShieldFrame = Bars.Frame(verticalFrames: 3, frameY: 1);
            barShieldFrame.X += topLeftOffset.X;
            barShieldFrame.Y += topLeftOffset.Y;
            barShieldFrame.Width = 2;
            barShieldFrame.Height = barSize.Y;

            Rectangle tipShieldFrame = Bars.Frame(verticalFrames: 3, frameY: 1);
            tipShieldFrame.X += topLeftOffset.X;
            tipShieldFrame.Y += topLeftOffset.Y;
            tipShieldFrame.Width = (int)(Bars.Width() * lifeRatio);
            tipShieldFrame.Height = barSize.Y;

            Rectangle barPosition = Utils.CenteredRectangle(barCenter, barSize.ToVector2());
            Vector2 barTopLeft = barPosition.TopLeft();
            Vector2 topLeft = barTopLeft - topLeftOffset.ToVector2();

            int shieldScale = (int)(barSize.X * shield / shieldMax);
            shieldScale -= shieldScale % 2;

            int lifeScale = (int)(barSize.X * life / lifeMax);
            lifeScale -= lifeScale % 2;

            // Background.
            spriteBatch.Draw(Bars.Value, barTopLeft + new Vector2(2, 0), bgFrame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);

            //Vector2 stretchScale = new(scale / barFrame.Width, 1f);          
            Main.spriteBatch.Draw(Bars.Value, barTopLeft + new Vector2(4, 0), tipShieldFrame, Color.White, 0f, Vector2.Zero, 1, 0, 0f);
            Main.spriteBatch.Draw(Bars.Value, barTopLeft + new Vector2(2, 0), barFrame, Color.White, 0f, Vector2.Zero, 1, 0, 0f);

            // Bar itself (shield).
            if (shield > 0f)
            {
                spriteBatch.Draw(Bars.Value, barTopLeft, barShieldFrame, Color.White, 0f, Vector2.Zero, 1, 0, 0f);
                spriteBatch.Draw(Bars.Value, barTopLeft + new Vector2(shieldScale - 2, 0f), tipShieldFrame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
            }

            // Frame.
            Rectangle frameFrame = Frame.Frame(verticalFrames: 1, frameY: 0);
            spriteBatch.Draw(Frame.Value, topLeft + new Vector2(0, 4), frameFrame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);

            // Icon.
            Vector2 iconOffset = new(10f, 16f);
            Vector2 iconSize = new(26f, 28f);
            Vector2 iconPosition = iconOffset + iconSize * 0.5f;
            spriteBatch.Draw(iconTexture, topLeft + iconPosition, iconFrame, iconColor, 0f, iconFrame.Size() / 2f, iconScale, 0, 0f);

            // Health text.
            if (BigProgressBarSystem.ShowText && showText)
            {
                if (shield > 0f)
                    BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, textOffset, shield, shieldMax);
                else
                    BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, textOffset, life, lifeMax);
            }
            return false;
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (npc.townNPC || !npc.active)
                return false;

            life = npc.life;
            lifeMax = npc.lifeMax;
                    

            bossHeadIndex = npc.GetBossHeadTextureIndex();
            return true;
        }
    }
}
