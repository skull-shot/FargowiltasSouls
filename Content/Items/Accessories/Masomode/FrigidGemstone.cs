using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Fargowiltas.FargoSets;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class FrigidGemstone : SoulsItem
    {
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
            Item.value = Item.sellPrice(0, 4);
        }

        void Effects(Player player)
        {
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.AddEffect<FrigidGemstoneKeyEffect>(Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => Effects(player);

        public override void UpdateInventory(Player player) => Effects(player);

        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<FrigidGemstoneInactive>());
            return false;
        }

        public override void UpdateVanity(Player player) => Effects(player);
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<FrigidGemstoneInactive>());
        }
    }

    public class FrigidGemstoneInactive : SoulsItem
    {
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
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4); 
        }

        void Effects(Player player)
        {
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.AddEffect<FrigidGemstoneKeyEffect>(Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => Effects(player);

        //public override void UpdateInventory(Player player) => Effects(player);
        public override bool AltFunctionUse(Player player) 
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<FrigidGemstone>());
            return false;
        }

        public override void UpdateVanity(Player player) => Effects(player);
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<FrigidGemstone>());
        }
    }
    public class FrigidGemstoneKeyEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override int ToggleItemType => ModContent.ItemType<FrigidGemstone>();
        public override void ActiveSkillHeld(Player player, bool stunned)
        {
            if (stunned)
                return;
            player.FargoSouls().FrigidGemstoneKey();
        }
    }
}