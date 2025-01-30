using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ChlorophyteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(36, 137, 0);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<ChloroMinion>(item);
            player.FargoSouls().ChlorophyteEnchantActive = true;
            player.AddEffect<JungleJump>(item);
            player.AddEffect<JungleDashEffect>(item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyChloroHead")
            .AddIngredient(ItemID.ChlorophytePlateMail)
            .AddIngredient(ItemID.ChlorophyteGreaves)
            .AddIngredient(null, "JungleEnchant")
            .AddIngredient(ItemID.ChlorophyteWarhammer)
            .AddIngredient(ItemID.ChlorophyteClaymore)
            //grape juice
            //.AddIngredient(ItemID.Seedling);
            //plantero pet

            .AddTile(TileID.CrystalBall)
           .Register();
        }
    }
    public class ChloroMinion : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<ChlorophyteEnchant>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Chlorofuck>()] == 0)
            {
                int dmg = 29;
                const int max = 5;
                float rotation = 2f * (float)Math.PI / max;
                for (int i = 0; i < max; i++)
                {
                    Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                    FargoSoulsUtil.NewSummonProjectile(GetSource_EffectItem(player), spawnPos, Vector2.Zero, ModContent.ProjectileType<Chlorofuck>(), dmg, 10f, player.whoAmI, Chlorofuck.Cooldown, rotation * i);
                }
            }
        }
    }
}
