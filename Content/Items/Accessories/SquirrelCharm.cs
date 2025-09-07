using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories
{
    public class SquirrelCharm : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories", Name);

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
            Item.value = Item.sellPrice(0, 1);
        }

        int counter;

        void PassiveEffect(Player player)
        {
            player.FargoSouls().SquirrelCharm = Item;
            if (player.whoAmI == Main.myPlayer && player.FargoSouls().IsStandingStill && player.itemAnimation == 0 && player.HeldItem != null)
            {
                if (++counter > 60)
                {
                    player.detectCreature = true;

                    if (counter % 10 == 0)
                        Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(player.Center);
                }
            }
            else
            {
                counter = 0;
            }
        }

        public override void UpdateInventory(Player player) => PassiveEffect(player);
        public override void UpdateVanity(Player player) => PassiveEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual) => PassiveEffect(player);
    }
}