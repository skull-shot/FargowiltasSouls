﻿using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Items.Weapons.Challengers;
using Terraria.ID;
using FargowiltasSouls.Items.Accessories.Expert;

namespace FargowiltasSouls.Items.BossBags
{
    public class TrojanSquirrelBag : BossBag
    {
        protected override bool IsPreHMBag => true;

        public override int BossBagNPC => ModContent.NPCType<NPCs.Challengers.TrojanSquirrel>();

        public override void OpenBossBag(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.ItemType<BoxofGizmos>());

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), Main.rand.Next(new int[] {
                ModContent.ItemType<TreeSword>(),
                ModContent.ItemType<MountedAcornGun>(),
                ModContent.ItemType<SnowballStaff>(),
                ModContent.ItemType<KamikazeSquirrelStaff>()
            }));

            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.WoodenCrate, 5);
            player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ItemID.HerbBag, 5);

            if (Main.rand.NextBool(5))
                player.QuickSpawnItem(player.GetSource_OpenItem(Item.type), ModContent.Find<ModItem>("Fargowiltas", "LumberJaxe").Type);
        }
    }
}