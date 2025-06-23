using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Items.Armor;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class MutantMaskMapLayer : ModMapLayer
    {
        public int maxFrames = 4;
        public int frame = 1;
        public int framecounter;

        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        { 
                return;

            Texture2D wingTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Armor/MutantMaskFlame", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            if (++framecounter >= 8)
            {
                framecounter = 0;
                frame++;
                if (frame >= maxFrames)
                    frame = 1;
            }
            
            Rectangle flameframe = new(0, (wingTexture.Height / maxFrames) * frame, wingTexture.Width, wingTexture.Height / maxFrames);
            context.Draw(wingTexture, Main.LocalPlayer.Center / 16, Color.White, new SpriteFrame(1, 4, 0, 1), 1.2f, 1.2f, Terraria.UI.Alignment.Center); 
        }
    }
}
