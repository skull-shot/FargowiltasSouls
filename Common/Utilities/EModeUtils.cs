using Fargowiltas;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Utilities
{
    public static class EModeUtils
    {
        public static void DropSummon(NPC npc, int itemType, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (WorldSavingSystem.EternityMode && prerequisite && !downed && FargoSoulsUtil.HostCheck && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                
                Player player = Main.player[npc.target];
                if (player.HasItem(itemType))
                {
                    player.GetModPlayer<FargoPlayer>().ItemHasBeenOwned[itemType] = true;
                }
                if (!player.GetModPlayer<FargoPlayer>().ItemHasBeenOwned[itemType])
                {
                    Item.NewItem(npc.GetSource_Loot(), player.Hitbox, itemType);
                    player.GetModPlayer<FargoPlayer>().ItemHasBeenOwned[itemType] = true;
                }
                droppedSummonFlag = true;
            }
        }

        public static void DropSummon(NPC npc, string itemName, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (WorldSavingSystem.EternityMode && prerequisite && !downed && FargoSoulsUtil.HostCheck && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];
                int type = ModContent.TryFind("Fargowiltas", itemName, out ModItem modItem) ? modItem.Type : -1;
                if (type >= 0)
                {
                    if (player.HasItem(type))
                    {
                        player.GetModPlayer<FargoPlayer>().ItemHasBeenOwned[type] = true;
                    }
                    if (!player.GetModPlayer<FargoPlayer>().ItemHasBeenOwned[type])
                    {
                        Item.NewItem(npc.GetSource_Loot(), player.Hitbox, type);
                        player.GetModPlayer<FargoPlayer>().ItemHasBeenOwned[type] = true;
                    }
                    
                }
                droppedSummonFlag = true;
            }
        }
    }
}
