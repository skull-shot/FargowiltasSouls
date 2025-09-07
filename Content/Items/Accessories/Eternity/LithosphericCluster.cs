using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    [AutoloadEquip(EquipType.Shield)]
    public class LithosphericCluster : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<ParryEffect>()];
        public override bool Eternity => true;

        public override int NumFrames => 5;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 5));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true; //intentionally not set for inactive version
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

        public static void PassiveEffect(Player player, Item item)
        {
            SecurityWallet.PassiveEffects(player);
            WyvernFeather.PassiveEffects(player, item);
            MysticSkull.PassiveEffects(player);
            //WretchedPouch.PassiveEffects(player, item);
            //SkullCharm.PassiveEffects(player, item);
            //DreadShell.PassiveEffects(player, item);
        }
        public static void ActiveEffect(Player player, Item item)
        {
            PassiveEffect(player, item);
            player.AddEffect<LithosphericEffect>(item);
            //SecurityWallet.ActiveEffects(player, item);
            WyvernFeather.ActiveEffects(player, item);
            //MysticSkull.ActiveEffects(player, item);
            WretchedPouch.ActiveEffects(player, item);
            CrystalSkull.ActiveEffects(player, item, false);
            DreadShell.ActiveEffects(player, item);
        }

        public override void UpdateInventory(Player player) => PassiveEffect(player, Item);
        public override void UpdateVanity(Player player) => PassiveEffect(player, Item);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffect(player, Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient<SecurityWallet>()
            .AddIngredient<WyvernFeather>()
            .AddIngredient<MysticSkull>()
            .AddIngredient<WretchedPouch>()
            .AddIngredient<CrystalSkull>()
            .AddIngredient<DreadShell>()
            .AddIngredient(ItemID.SpectreBar, 5)
            .AddIngredient<DeviatingEnergy>(10)

            .AddTile(TileID.MythrilAnvil)
            .DisableDecraft()
            .Register();
        }
        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<LithosphericClusterInactive>());
            return false;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<LithosphericClusterInactive>());
        }
    }
    [AutoloadEquip(EquipType.Shield)]
    public class LithosphericClusterInactive : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<ParryEffect>()];
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
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateInventory(Player player) { return; }//PassiveEffect(player, Item);
        public override void UpdateVanity(Player player) { return; }//PassiveEffect(player, Item);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            LithosphericCluster.ActiveEffect(player, Item);
        }
        public override bool? UseItem(Player player) => true;

        public override bool AltFunctionUse(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab);
            player.ReplaceItem(Item, ModContent.ItemType<LithosphericCluster>());
            return false;
        }

        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<LithosphericCluster>());
        }
    }
    public class LithosphericEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<LithosphericCluster>();
    }
}
