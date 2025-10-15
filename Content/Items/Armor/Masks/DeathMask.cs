﻿using FargowiltasSouls.Assets.Textures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor.Masks
{
    [AutoloadEquip(EquipType.Head)]
    public class DeathMask : ModItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Armor/Masks", Name);
        public override void SetStaticDefaults()
        {         
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
    }
}