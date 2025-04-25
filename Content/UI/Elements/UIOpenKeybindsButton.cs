using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
    public class UIOpenKeybindsButton : UIImageFramed
    {
        public UIOpenKeybindsButton(Asset<Texture2D> texture, Rectangle frame) : base(texture, frame)
        {
            
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle style = GetDimensions();
            if (IsMouseHovering)
            {
                Vector2 textPosition = style.Position() + new Vector2(0, style.Height + 8);
                Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.FargowiltasSouls.UI.OpenKeybinds"), textPosition, Color.White);
            }
        }
    }
}
