﻿using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls.Content.Items.Armor.Masks;
using FargowiltasSouls.Content.Items.Placables;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class LifelightBag : BossBag
    {
        protected override bool IsPreHMBag => false;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifelightMask>(), 7));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifeRevitalizer>()));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Lifelight>()));
            itemLoot.Add(new OneFromOptionsDropRule(1, 1,
            [
                ModContent.ItemType<EnchantedLifeblade>(),
                ModContent.ItemType<Lightslinger>(),
                ModContent.ItemType<CrystallineCongregation>(),
                ModContent.ItemType<KamikazePixieStaff>()
            ]));
            itemLoot.Add(ItemDropRule.Common(ItemID.SoulofLight, 1, 3, 3));
            itemLoot.Add(ItemDropRule.Common(ItemID.PixieDust, 1, 25, 25));
        }
    }
}