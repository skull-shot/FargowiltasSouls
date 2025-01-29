using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    [AutoloadEquip(EquipType.Shield)]
    public class SilverEnchant : BaseEnchant
    {
        public override List<AccessoryEffect> ActiveSkillTooltips => 
            [AccessoryEffectLoader.GetEffect<ParryEffect>()];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            
        }

        public override Color nameColor => new(180, 180, 204);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SilverEffect>(Item);
            player.AddEffect<ParryEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.SilverHelmet)
            .AddIngredient(ItemID.SilverChainmail)
            .AddIngredient(ItemID.SilverGreaves)
            .AddIngredient(ItemID.EmptyBucket)
            .AddIngredient(ItemID.SilverBroadsword)
            .AddIngredient(ItemID.BlandWhip)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class SilverEffect : AccessoryEffect
    {

        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<SilverEnchant>();
    }
    public class ParryEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<SilverEnchant>();
        public override bool ActiveSkill => true;

        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("ParryCooldown", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Enchantments/SilverEnchant").Value, Color.Gray, () => (float)shieldCD / cooldown)
        }
    }
}
