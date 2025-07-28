using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class BionomicCluster : SoulsItem
    {
        public override bool Eternity => true;
        public override List<AccessoryEffect> ActiveSkillTooltips => 
            [AccessoryEffectLoader.GetEffect<FrigidGemstoneKeyEffect>()];

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 3;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.UseSound = SoundID.DD2_BetsyFlameBreath with { Pitch = -1f, Volume = 2f };
        }

        public static void PassiveEffect(Player player, Item item)
        {
            FrigidGemstone.PassiveEffects(player, item);
            SandsofTime.PassiveEffects(player);
            //SqueakyToy.PassiveEffects(player, item);
            NymphsPerfume.PassiveEffects(player, item);
            //TimsConcoction.PassiveEffects(player, item);
        }
        public static void ActiveEffect(Player player, Item item)
        {
            PassiveEffect(player, item);
            FrigidGemstone.ActiveEffects(player, item);
            //SandsofTime.ActiveEffects(player, Item);
            SqueakyToy.ActiveEffects(player, item);
            NymphsPerfume.ActiveEffects(player, item);
            TimsConcoction.ActiveEffects(player, item);
        }

        public override void UpdateInventory(Player player) => PassiveEffect(player, Item);
        public override void UpdateVanity(Player player) => PassiveEffect(player, Item);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffect(player, Item);
            player.AddEffect<FrigidGemstoneKeyEffect>(Item);
        }

        public override void UseItemFrame(Player player) => SandsofTime.Use(player);
        public override bool? UseItem(Player player) => true;

        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<BionomicClusterInactive>());
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient<FrigidGemstone>()
            .AddIngredient<SandsofTime>()
            .AddIngredient<SqueakyToy>()
            .AddIngredient<NymphsPerfume>()
            .AddIngredient<TimsConcoction>()
            .AddIngredient(ItemID.HellstoneBar, 5)
            .AddIngredient<DeviatingEnergy>(10)

            .AddTile(TileID.Anvils)
            .DisableDecraft()
            .Register();
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<BionomicClusterInactive>());
        }
    }
    public class BionomicClusterInactive : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 0;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 6;
            Item.useTime = 180;
            Item.useAnimation = 180;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item6;
        }

        public override void UpdateInventory(Player player) { return; }//PassiveEffect(player, Item);
        public override void UpdateVanity(Player player) { return; }//PassiveEffect(player, Item);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BionomicCluster.ActiveEffect(player, Item);
        }

        public override void UseItemFrame(Player player) => SandsofTime.Use(player);
        public override bool? UseItem(Player player) => true;

        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<BionomicCluster>());
            return false;
        }

        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<BionomicCluster>());
        }
    }
}
