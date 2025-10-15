﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    [AutoloadEquip(EquipType.Face)]
    public class MagicalBulb : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<BulbKeyEffect>()];

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
            player.AddEffect<IvyVenomEffect>(Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[ModContent.BuffType<IvyVenomBuff>()] = true;
            player.FargoSouls().MagicalBulb = true;
            player.AddEffect<BulbKeyEffect>(item);
            player.AddEffect<BulbRegenEffect>(item);
        }
    }
    public class IvyVenomEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<VerdantHeader>();
        public override int ToggleItemType => ModContent.ItemType<MagicalBulb>();
        public override void PostUpdateEquips(Player player)
        {
            //if (!player.HasBuff<SouloftheMasochistBuff>())
                //player.AddBuff(ModContent.BuffType<PlanterasChildBuff>(), 2);
        }
    }
    public class BulbRegenEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<MagicalBulb>();
        public override void PostUpdateEquips(Player player)
        {
            Point pos = player.Center.ToTileCoordinates();
            if (pos.X > 0 && pos.Y > 0 && pos.X < Main.maxTilesX && pos.Y < Main.maxTilesY && player.whoAmI == Main.myPlayer) //check for multiplayer hopefully
            {
                int power = player.FargoSouls().MasochistSoul ? 12 : 6;
                float lightStrength = Lighting.GetColor(pos).ToVector3().Length();
                float ratio = lightStrength / 1.732f; //this value is 1,1,1 lighting
                if (ratio < 1)
                    ratio /= 2;
                player.lifeRegen += (int)(6 * ratio);
            }
        }
    }
    public class BulbKeyEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override int ToggleItemType => ModContent.ItemType<MagicalBulb>();
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (stunned)
                return;
            player.FargoSouls().MagicalBulbKey();
        }
    }
}