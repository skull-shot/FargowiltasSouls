﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class SqueakyToy : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 3);

            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }
        public static void ActiveEffects(Player player, Item item)
        {
            player.buffImmune[ModContent.BuffType<SqueakyToyBuff>()] = true;
            player.AddEffect<SqueakEffect>(item);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
        }
        public override bool? UseItem(Player player)
        {
            if (player.ItemTimeIsZero && player.ItemAnimationJustStarted)
                FargoSoulsPlayer.Squeak(player.Center, 1f);
            return base.UseItem(player);
        }
        /*private bool lastLMouse = false;
        public override void HoldItem(Player player) //doing this instead of making an item use animation lo
        {
            if (!lastLMouse && Main.mouseLeft)
            {
                FargoSoulsPlayer.Squeak(player.Center, 0.25f);
            }
            lastLMouse = Main.mouseLeft;
        }*/
    }
    public class SqueakEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<BionomicHeader>();
        public override int ToggleItemType => ModContent.ItemType<SqueakyToy>();
        public override bool MutantsPresenceAffects => true;
    }
}