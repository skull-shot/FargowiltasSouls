﻿using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Placables.Relics
{
    public abstract class BaseRelic : SoulsItem
    {
        public override string Texture => $"{Mod.Name}/Assets/Textures/Content/Items/Placables/Relics/{Name}";
        protected abstract int TileType { get; }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.DefaultToPlaceableTile(TileType);

            Item.width = 30;
            Item.height = 40;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.value = Item.buyPrice(0, 5);
        }
    }
}
