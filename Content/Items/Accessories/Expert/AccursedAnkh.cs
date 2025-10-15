using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Expert
{
    public class AccursedAnkh : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Expert", Name);
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 10));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
        public override int NumFrames => 10;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.maxStack = 1;
            Item.useAnimation = 34; // perfectly synced with sound effect
            Item.useTime = 34;
            Item.autoReuse = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;

            Item.UseSound = CursedCoffin.SpiritDroneSFX;
            Item.value = Item.sellPrice(0, 4);
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (!player.HandPosition.HasValue)
                return;
            player.direction = (int)player.HorizontalDirectionTo(Main.MouseWorld);
            player.itemLocation = player.HandPosition.Value - Vector2.UnitX * 15 * player.direction;
            player.itemLocation -= Vector2.UnitY * player.gfxOffY;
        }
        public override bool? UseItem(Player player)
        {
            player.Eternity().ShorterDebuffsTimer = 0;
            //player.FargoSouls().UsingAnkh = true;
            return base.UseItem(player);
        }
    }
}