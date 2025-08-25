using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class PalladiumEnchant : BaseEnchant
    {

        public override Color nameColor => new(245, 172, 40);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<PalladiumEffect>(Item);
            player.AddEffect<PalladiumHealing>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyPallaHead")
                .AddIngredient(ItemID.PalladiumBreastplate)
                .AddIngredient(ItemID.PalladiumLeggings)
                .AddIngredient(ItemID.BatBat)
                .AddIngredient(ItemID.SoulDrain)
                .AddIngredient(ItemID.UndergroundReward)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Generic;
            tooltipColor = null;
            scaling = null;
            return PalladiumEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class PalladiumHealing : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (!HasEffectEnchant(player))
                return;

            if (!player.onHitRegen)
            {
                player.AddBuff(BuffID.RapidHealing, Math.Min(LumUtils.SecondsToFrames(5), hitInfo.Damage / 3)); //heal time based on damage dealt, capped at 5sec
            }
        }
    }
    public class PalladiumEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<PalladiumEnchant>();
        public static int BaseDamage(Player player) => FargoSoulsUtil.HighestDamageTypeScaling(player, player.ForceEffect<PalladiumEffect>() ? 100 : 50);
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            int increment = player.statLife - modPlayer.StatLifePrevious;
            if (increment > 0)
            {
                modPlayer.PalladCounter += increment;
                if (modPlayer.PalladCounter > 80)
                {
                    modPlayer.PalladCounter = 0;
                    if (player.whoAmI == Main.myPlayer && player.statLife < player.statLifeMax2)
                    {
                        Projectile.NewProjectile(player.GetSource_Accessory(player.EffectItem<PalladiumEffect>()), player.Center, -Vector2.UnitY, ModContent.ProjectileType<PalladOrb>(),
                            BaseDamage(player), 10f, player.whoAmI, -1);
                    }
                }
            }

            if (player.ForceEffect<PalladiumEffect>() || modPlayer.TerrariaSoul)
                player.onHitRegen = true;
        }
    }
}
