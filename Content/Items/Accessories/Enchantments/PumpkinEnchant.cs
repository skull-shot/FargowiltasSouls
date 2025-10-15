using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class PumpkinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(227, 101, 28);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<PumpkinEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PumpkinHelmet)
                .AddIngredient(ItemID.PumpkinBreastplate)
                .AddIngredient(ItemID.PumpkinLeggings)
                .AddIngredient(ItemID.PumpkinPie)
                .AddIngredient(ItemID.MolotovCocktail, 50)
                .AddIngredient(ItemID.Sickle)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Generic;
            tooltipColor = null;
            scaling = null;
            return PumpkinEffect.BaseDamage(Main.LocalPlayer);
        }
    }

    public class PumpkinEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<PumpkinEnchant>();
        public static int BaseDamage(Player player) => FargoSoulsUtil.HighestDamageTypeScaling(player, player.ForceEffect<PumpkinEffect>() ? 45 : 15);
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if ((player.controlLeft || player.controlRight) && !modPlayer.IsStandingStill && player.whoAmI == Main.myPlayer)
            {
                if (modPlayer.PumpkinSpawnCD <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<GrowingPumpkin>()] < 10)
                {
                    int x = (int)player.Center.X / 16;
                    int y = (int)(player.position.Y + player.height - 1f) / 16;

                    //Main.tile[x, y] ??= new Tile();

                    if (!Main.tile[x, y].HasTile && Main.tile[x, y].LiquidType == 0 && Main.tile[x, y + 1] != null && (WorldGen.SolidTile(x, y + 1) || Main.tile[x, y + 1].TileType == TileID.Platforms)
                        || modPlayer.ForceEffect<PumpkinEnchant>())
                    {
                        Projectile.NewProjectile(player.GetSource_Accessory(player.EffectItem<PumpkinEffect>()), player.Center, Vector2.Zero, ModContent.ProjectileType<GrowingPumpkin>(), 0, 0, player.whoAmI);
                        modPlayer.PumpkinSpawnCD = LumUtils.SecondsToFrames(7.5f);
                    }
                }
            }

            if (modPlayer.PumpkinSpawnCD > 0)
            {
                modPlayer.PumpkinSpawnCD--;
            }
        }
    }

}
