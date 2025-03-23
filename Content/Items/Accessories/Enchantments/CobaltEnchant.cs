using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class CobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(61, 164, 196);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<CobaltEffect>(Item);
            player.AddEffect<AncientCobaltFallEffect>(Item);
            player.AddEffect<AncientCobaltEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyCobaltHead")
            .AddIngredient(ItemID.CobaltBreastplate)
            .AddIngredient(ItemID.CobaltLeggings)
            .AddIngredient(null, "AncientCobaltEnchant")
            .AddIngredient(ItemID.ScarabBomb, 10)
            .AddIngredient(ItemID.DD2ExplosiveTrapT1Popper)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class CobaltEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<CobaltEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            var modPlayer = player.FargoSouls();

            if (modPlayer.CobaltJumpCooldown > 0)
                modPlayer.CobaltJumpCooldown--;

            if (modPlayer.CanCobaltJump || modPlayer.JustCobaltJumped && !player.ExtraJumps.ToArray().Any(j => j.Active) && !modPlayer.JungleJumping)
            {
                
            }
            else
                player.jumpSpeedBoost += 3f;
        }
        public override void PostUpdate(Player player)
        {
            //player.jump *= 2;
        }
    }
}
