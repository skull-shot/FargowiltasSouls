﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Luminance.Core.Graphics;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class HallowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(150, 133, 100);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 180000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<HallowEffect>(Item);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyHallowHead")
                .AddIngredient(ItemID.HallowedPlateMail)
                .AddIngredient(ItemID.HallowedGreaves)
                .AddIngredient(ItemID.HallowJoustingLance)
                .AddIngredient(ItemID.Gungnir)
                .AddIngredient(ItemID.HolyWater, 50)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }
    public class HallowEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
        public override int ToggleItemType => ModContent.ItemType<HallowEnchant>();

        public const int RepelRadius = 350;
        public static void HealRepel(Player player)
        {
            Item effectItem = player.EffectItem<HallowEffect>();
            if (!player.HasEffectEnchant<HallowEffect>())
                return;
            int duration = player.ForceEffect<HallowEffect>() ? 120 : 60;
            player.FargoSouls().HallowRepelTime = duration;

            SoundEngine.PlaySound(SoundID.Item72);
            Particle p = new HallowEnchantBarrier(player.Center, Vector2.Zero, RepelRadius / 160f, duration + 20, player: player, baseOpacity: 0.5f);
            p.Spawn();

        }
        public override void PostUpdateEquips(Player player)
        {
            if (player.FargoSouls().HallowRepelTime > 0)
            {
                player.FargoSouls().HallowRepelTime--;
                foreach (Projectile projectile in Main.projectile.Where(p => p.hostile && FargoSoulsUtil.CanDeleteProjectile(p) && p.Distance(player.Center) <= RepelRadius * 0.6f))
                {
                    projectile.velocity = Vector2.Normalize(projectile.Center - player.Center) * projectile.velocity.Length();
                    projectile.hostile = false;
                }
            }
        }
    }
}
