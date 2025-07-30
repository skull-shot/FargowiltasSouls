using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    [AutoloadEquip(EquipType.Shoes)]
    public class AeolusBoots : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //terraspark
            player.accRunSpeed = 6.75f;
            player.rocketBoots = player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
            player.moveSpeed += 0.08f;
            player.iceSkate = true;

            //lava wader
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 420;
            player.lavaRose = true;

            //amph boot
            player.AddEffect<MasoAeolusFrog>(Item);

            player.AddEffect<MasoAeolusFlower>(Item);
            player.AddEffect<ZephyrJump>(Item);

            //dunerider boot
            player.desertBoots = true;

            player.jumpBoost = true;
            player.noFallDmg = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ZephyrBoots>())
                .AddIngredient(ItemID.AmphibianBoots)
                .AddIngredient(ItemID.FairyBoots)
                .AddIngredient(ItemID.SandBoots)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class MasoAeolusFrog : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<AeolusBoots>();
        public override void PostUpdateEquips(Player player)
        {
            player.frogLegJumpBoost = true;
            player.autoJump = true;
        }
    }
    public class MasoAeolusFlower : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<AeolusBoots>();
        
        public override void PostUpdateEquips(Player player)
        {
            if (!player.flowerBoots)
            {
                player.flowerBoots = true;
                if (player.whoAmI == Main.myPlayer)
                    player.DoBootsEffect(new Utils.TileActionAttempt(player.DoBootsEffect_PlaceFlowersOnTile));
            }
        }
    }
}
