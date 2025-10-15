﻿using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Placables.Trophies
{
    public abstract class BaseTrophy : SoulsItem
    {
        public override string Texture => $"{Mod.Name}/Assets/Textures/Content/Items/Placables/Trophies/{Name}";
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

            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 1);
        }
    }
}
