﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class DarkArtistEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(155, 92, 176);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.ApprenticeEnchantActive = true;
            modPlayer.DarkArtistEnchantActive = true;
            player.AddEffect<ApprenticeSupport>(Item);
            player.AddEffect<DarkArtistMinion>(Item);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ApprenticeAltHead)
                .AddIngredient(ItemID.ApprenticeAltShirt)
                .AddIngredient(ItemID.ApprenticeAltPants)
                .AddIngredient(null, "ApprenticeEnchant")
                .AddIngredient(ItemID.DD2FlameburstTowerT3Popper)
                //.AddIngredient(ItemID.ShadowbeamStaff);
                .AddIngredient(ItemID.InfernoFork)
                //Razorpine
                //staff of earth

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }

        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Magic;
            tooltipColor = null;
            scaling = null;
            return DarkArtistMinion.BaseDamage(Main.LocalPlayer);
        }
    }
    public class DarkArtistMinion : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<DarkArtistEnchant>();
        public override bool MinionEffect => true;
        public static int BaseDamage(Player player) => (int)(72 * player.ActualClassDamage(DamageClass.Magic));
        public override void PostUpdateEquips(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlameburstMinion>()] == 0)
            {
                //spawn tower boi
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(GetSource_EffectItem(player), player.Center, Vector2.Zero, ModContent.ProjectileType<FlameburstMinion>(), 0, 0f, player.whoAmI);
                    proj.netUpdate = true; // TODO make this proj sync meme
                }
            }
        }
    }
}
