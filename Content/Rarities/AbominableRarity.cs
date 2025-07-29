using FargowiltasSouls.Content.Buffs.Eternity;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Rarities
{
    public class AbominableRarity : ModRarity
    {
        public static Color AbomColor = Color.DarkOrange;
        public static int Lerp;
        public static int Style = 1;
        public static bool RenderTooltip(DrawableTooltipLine line)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                //++Lerp;
                if (Style == 1)
                {
                    AbomColor.G -= 3;
                    if (AbomColor.G <= 110)
                    {
                        Style = 2;
                        //Lerp = 0;
                    }
                        
                }
                if (Style == 2)
                {
                    AbomColor.G += 3;
                    if (AbomColor.G >= 195)
                    {
                        Style = 3;
                        //Lerp = 0;
                    }

                }
                if (Style == 3)
                {
                    AbomColor.B += 3;
                    if (AbomColor.B >= 55)
                    {
                        Style = 4;
                        //Lerp = 0;
                    }

                }
                if (Style == 4)
                {
                    AbomColor.B -= 3;
                    if (AbomColor.B <= 5)
                    {
                        Style = 1;
                        //Lerp = 0;
                    }

                }
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), AbomColor, 1); //draw the tooltip manually
                return false;
            }
            return true;
        }
    }
}
