using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
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
            .AddIngredient(ItemID.SilverBroadsword)
            .AddIngredient(ItemID.Harpoon)
            .AddIngredient(ItemID.SilverWatch)

                .AddTile<EnchantedTreeSheet>()
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
            int cooldown = FargoSoulsPlayer.ShieldCooldown(player);
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.HasEffect<PumpkingsCapeEffect>())
                    CooldownBarManager.Activate("ParryCooldown", FargoAssets.GetTexture2D("Content/Items/Accessories/Eternity", "PumpkingsCape").Value, Color.Lerp(Color.LightGoldenrodYellow, Color.OrangeRed, 0.5f), () => (float)Main.LocalPlayer.FargoSouls().shieldCD / cooldown, activeFunction: () => player.HasEffect<PumpkingsCapeEffect>());
                else if (player.HasEffect<DreadShellEffect>())
                    CooldownBarManager.Activate("ParryCooldown", FargoAssets.GetTexture2D("Content/Items/Accessories/Eternity", "DreadShell").Value, Color.DarkRed, () => (float)Main.LocalPlayer.FargoSouls().shieldCD / cooldown, activeFunction: () => player.HasEffect<DreadShellEffect>());
                else CooldownBarManager.Activate("ParryCooldown", FargoAssets.GetTexture2D("Content/Items/Accessories/Enchantments", "SilverEnchant").Value, Color.Gray, () => (float)Main.LocalPlayer.FargoSouls().shieldCD / cooldown, activeFunction: () => player.HasEffect<SilverEffect>());
            }
        }
    }
}
