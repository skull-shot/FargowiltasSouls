using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class MinerEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(95, 117, 151);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float speed = modPlayer.ForceEffect<MinerEnchant>() ? .75f : .5f;
            AddEffects(player, speed, Item);
        }

        public static void AddEffects(Player player, float pickSpeed, Item item)
        {
            player.pickSpeed -= pickSpeed;
            player.nightVision = true;

            player.AddEffect<MiningSpelunk>(item);
            player.AddEffect<MiningHunt>(item);
            player.AddEffect<MiningDanger>(item);
            player.AddEffect<MiningShine>(item);

            player.FargoSouls().MiningImmunity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.UltrabrightHelmet)
                .AddIngredient(ItemID.MiningShirt)
                .AddIngredient(ItemID.MiningPants)
                .AddIngredient(ItemID.AncientChisel)
                .AddIngredient(ItemID.CopperPickaxe)
                .AddIngredient(ItemID.GravediggerShovel)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class MiningSpelunk : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
        public override int ToggleItemType => ModContent.ItemType<MinerEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            player.findTreasure = true;
        }
    }
    public class MiningHunt : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
        public override int ToggleItemType => ModContent.ItemType<MinerEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            player.detectCreature = true;
        }
    }
    public class MiningDanger : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
        public override int ToggleItemType => ModContent.ItemType<MinerEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            player.dangerSense = true;
        }
    }
    public class MiningShine : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
        public override int ToggleItemType => ModContent.ItemType<MinerEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            Lighting.AddLight(player.Center, 0.8f, 0.8f, 0);
        }
    }
}
