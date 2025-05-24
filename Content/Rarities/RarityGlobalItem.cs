using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Rarities
{
    public class RarityGlobalItem : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {   
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                if (item.rare == ModContent.RarityType<EternitySoulRarity>())
                {
                    EternitySoulRarity.RenderTooltip(line);
                    return false;
                }
            }
            
                
            return true;
        }
    }
}
