using Fargowiltas.Content.UI;
using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;

namespace FargowiltasSouls.Content.UI.Elements
{
    public class UIActiveSkillMenuDrag : UIPanel
    {
        // Stores the offset while dragging.
        private Vector2 offset;
        public bool dragging;

        public bool Hovering = false;

        public UIActiveSkillMenuDrag()
        {

        }
        private Vector2 TranslatePosition(Vector2 pos)
        {
            pos -= offset;
            pos.X = MathHelper.Clamp(pos.X, 0, Main.screenWidth - Parent.Width.Pixels);
            pos.Y = MathHelper.Clamp(pos.Y, 0, Main.screenHeight - Parent.Height.Pixels);
            pos += offset;
            return pos;
        }
        private void DragStart(Vector2 pos)
        {
            pos = TranslatePosition(pos);
            offset = new Vector2(pos.X - Parent.Left.Pixels, pos.Y - Parent.Top.Pixels);
            dragging = true;
        }

        public void DragEnd(Vector2 pos)
        {
            pos = TranslatePosition(pos);
            Vector2 end = pos - offset;
            dragging = false;
            Parent.Left.Set(end.X, 0f);
            Parent.Top.Set(end.Y, 0);
            Parent.Recalculate();

            StayInBounds();

            ClientConfig.Instance.ActiveSkillMenuX = end.X;
            ClientConfig.Instance.ActiveSkillMenuY = end.Y;
            ClientConfig.Instance.OnChanged();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            bool conditions = true; //&&  WorldSavingSystem.EternityMode;

            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (ContainsPoint(Main.MouseScreen) && conditions)
            {
                //if (!Hovering)
                //    SoundEngine.PlaySound(SoundID.MenuTick);
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

            if (dragging)
            {
                Vector2 pos = TranslatePosition(Main.MouseScreen);
                Parent.Left.Set(pos.X - offset.X, 0);
                Parent.Top.Set(pos.Y - offset.Y, 0);
                Parent.Recalculate();
            }
            else
            {
                Parent.Left.Set(ClientConfig.Instance.ActiveSkillMenuX, 0f);
                Parent.Top.Set(ClientConfig.Instance.ActiveSkillMenuY, 0);
                Parent.Recalculate();

            }

            StayInBounds();
        }

        private void StayInBounds()
        {
            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Parent.Left.Pixels = Utils.Clamp(Parent.Left.Pixels, 0, Main.screenWidth);
                Parent.Top.Pixels = Utils.Clamp(Parent.Top.Pixels, 0, Main.screenHeight);
                // Recalculate forces the UI system to do the positioning math again.
                Parent.Recalculate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            return;
        }
    }
}
