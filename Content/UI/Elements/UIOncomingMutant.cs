﻿using Fargowiltas.Content.UI;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
    public class UIOncomingMutant : UIPanel
    {
        // Stores the offset from the top left of the UIPanel while dragging.
        private Vector2 offset;
        public bool dragging;
        public Texture2D Texture;
        public Texture2D AuraTexture;
        public Texture2D EmptyTexture;
        public string TextEMode;
        public string TextMaso;
        public string TextDisabled;
        public string TextRightClick;

        public string TextHoldShift;
        public string TextExpandedEternity;
        public string TextExpandedMaso;
        public string TextExpandedFeatures;
        public string TextMasoMultiplayer;

        public int ToggleCooldown;

        public bool Hovering = false;

        public string mode;

        public UIOncomingMutant(Texture2D tex, Texture2D auraTex, Texture2D emptyTex, string textEMode, string textMaso, string textDisabled, string textRightClick, 
            string textHoldShift, string textExpandedEternity, string textExpandedMaso, string textExpandedFeatures, string textMasoMultiplayer)
        {
            Texture = tex;
            AuraTexture = auraTex;
            EmptyTexture = emptyTex;
            TextEMode = textEMode;
            TextMaso = textMaso;
            TextDisabled = textDisabled;
            TextRightClick = textRightClick;
            TextHoldShift = textHoldShift;
            TextExpandedEternity = textExpandedEternity;
            TextExpandedMaso = textExpandedMaso;
            TextExpandedFeatures = textExpandedFeatures;
            TextMasoMultiplayer = textMasoMultiplayer;

            Width.Set(24, 0);
            Height.Set(26, 0);
        }

        private void DragStart(Vector2 pos)
        {
            offset = new Vector2(pos.X - Left.Pixels, pos.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(Vector2 pos)
        {
            Vector2 end = pos - offset;
            dragging = false;

            Left.Set(end.X, 0);
            Top.Set(end.Y, 0);
            Recalculate();

            StayInBounds();

            ClientConfig.Instance.EternityMutantX = end.X;
            ClientConfig.Instance.EternityMutantY = end.Y;
            ClientConfig.Instance.OnChanged();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            bool conditions = Main.playerInventory; //&&  WorldSavingSystem.EternityMode;

            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (ContainsPoint(Main.MouseScreen) && conditions)
            {
                if (!Hovering)
                    SoundEngine.PlaySound(SoundID.MenuTick);
                Hovering = true;
                Main.LocalPlayer.mouseInterface = true;
            }
            else
                Hovering = false;

            if (!dragging && conditions && ContainsPoint(Main.MouseScreen) && Main.mouseLeft && PlayerInput.MouseInfoOld.LeftButton == ButtonState.Released)
            {
                DragStart(Main.MouseScreen);
            }
            else if (dragging && (!Main.mouseLeft || !conditions))
            {
                DragEnd(Main.MouseScreen);
            }

            if (Main.LocalPlayer.FargoSouls().EmodeToggleCooldown <= 0 && !dragging && conditions && ContainsPoint(Main.MouseScreen) && Main.mouseRight && PlayerInput.MouseInfoOld.RightButton == ButtonState.Released)
            {
                Main.LocalPlayer.FargoSouls().EmodeToggleCooldown = 15;
                if (Masochist.CanToggleEternity())
                    FargoUIManager.Toggle<DifficultySelectionMenu>();
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0); // Main.MouseScreen.X and Main.mouseX are the same.
                Top.Set(Main.mouseY - offset.Y, 0);
                Recalculate();
            }
            else
            {
                Left.Set(ClientConfig.Instance.EternityMutantX, 0);
                Top.Set(ClientConfig.Instance.EternityMutantY, 0);
                Recalculate();
            }

            StayInBounds();
        }

        private void StayInBounds()
        {
            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                Recalculate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //if (!WorldSavingSystem.EternityMode)
            //    return;

            //base.DrawSelf(spriteBatch);

            CalculatedStyle style = GetDimensions();
            // Logic
            if (IsMouseHovering && !dragging)
            {
                Vector2 textPosition = Main.MouseScreen + new Vector2(21, 21);
                string text = WorldSavingSystem.MasochistModeReal ? TextMaso : WorldSavingSystem.EternityMode ? TextEMode : TextDisabled;
                if (WorldSavingSystem.MasochistModeReal)
                    text = $"[c/33ffbe:{text}]";
                else if (WorldSavingSystem.EternityMode)
                    text = $"[c/00FFFF:{text}]";

                if (Masochist.CanToggleEternity())
                    text += $"\n[c/787878:{TextRightClick}]";
                

                if (Main.keyState.IsKeyDown(Keys.LeftShift))
                {
                    string difText = WorldSavingSystem.MasochistModeReal ? TextExpandedMaso : TextExpandedEternity;
                    text += $"\n{difText}";
                    text += $"\n{TextExpandedFeatures}";
                    if (WorldSavingSystem.MasochistModeReal && Main.netMode != NetmodeID.SinglePlayer)
                        text += $"\n{TextMasoMultiplayer}";
                }
                else
                    text += $"\n[c/787878:{TextHoldShift}]";

                Utils.DrawBorderString(
                    spriteBatch,
                    text,
                    textPosition,
                    Color.White);
            }

            // Drawing
            Vector2 position = style.Position();
            if (WorldSavingSystem.EternityMode)
            {
                spriteBatch.Draw(Texture, position + new Vector2(2), Texture.Bounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                if (WorldSavingSystem.MasochistModeReal)
                    spriteBatch.Draw(AuraTexture, position, AuraTexture.Bounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(EmptyTexture, position + new Vector2(2), Texture.Bounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }
    }
}
