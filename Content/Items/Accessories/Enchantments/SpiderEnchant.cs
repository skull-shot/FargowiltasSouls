using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SpiderEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override Color nameColor => new(109, 78, 69);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SpiderEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SpiderMask)
            .AddIngredient(ItemID.SpiderBreastplate)
            .AddIngredient(ItemID.SpiderGreaves)
            .AddIngredient(ItemID.SpiderStaff)
            .AddIngredient(ItemID.QueenSpiderStaff)
            .AddIngredient(ItemID.WebSlinger)
            //web rope coil
            //rainbow string
            //fried egg
            //.AddIngredient(ItemID.SpiderEgg);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class SpiderEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<SpiderEnchant>();
        //public override bool MinionEffect => true;
        
        public override void PostUpdateEquips(Player player)
        {
            //minion crits
            if (HasEffectEnchant(player))
                player.FargoSouls().MinionCrits = true;
            player.GetCritChance(DamageClass.Generic) += 10;
            if (player.FargoSouls().ForceEffect(ModContent.ItemType<SpiderEnchant>()))
                player.GetCritChance(DamageClass.Generic) += 15;
        }
    }
}
