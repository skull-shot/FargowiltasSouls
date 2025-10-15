﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler.Content;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SolarEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public static readonly Color NameColor = new(254, 158, 35);
        public override Color nameColor => NameColor;


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Purple;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.AddEffect<SolarEffect>(Item);
            player.AddEffect<SolarFlareEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SolarFlareHelmet)
            .AddIngredient(ItemID.SolarFlareBreastplate)
            .AddIngredient(ItemID.SolarFlareLeggings)
            //solar wings
            //.AddIngredient(ItemID.HelFire)
            //golem fist
            //xmas tree sword
            .AddIngredient(ItemID.DayBreak)
            .AddIngredient(ItemID.SolarEruption)
            .AddIngredient(ItemID.StarWrath) //terrarian

                .AddTile<EnchantedTreeSheet>()
            .Register();

        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Melee;
            tooltipColor = null;
            scaling = null;
            return SolarFlareEffect.BaseDamage(Main.LocalPlayer);
        }
    }

    public class SolarFlareEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<SolarEnchant>();
        public override bool ExtraAttackEffect => true;
        public static int BaseDamage(Player player) => (int)((player.ForceEffect<SolarFlareEffect>() ? 5800 : 2900) * player.ActualClassDamage(DamageClass.Melee));
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("SolarEnchantCharge", FargoAssets.GetTexture2D("Content/Items/Accessories/Enchantments", "SolarEnchant").Value, SolarEnchant.NameColor, 
                    () => Main.LocalPlayer.FargoSouls().SolarEnchCharge / 240, true, activeFunction: () => player.HasEffect<SolarFlareEffect>());

            player.endurance += 0.2f * modPlayer.SolarEnchCharge / 240f;
            if (player.HeldItem != null && player.HeldItem.damage > 0 && player.controlUseItem)
            {
                if (modPlayer.SolarEnchCharge < 240)
                {
                    modPlayer.SolarEnchCharge += 1;
                    if (modPlayer.SolarEnchCharge == 240)
                    {
                        SoundEngine.PlaySound(FargosSoundRegistry.ChargeSound, player.Center);
                    }
                }
                else
                {
                    //Charged particles
                    float rotation = Main.rand.NextFloat(0, 2 * MathHelper.Pi);
                    Vector2 pos = player.Center + 24 * Vector2.UnitX.RotatedBy(rotation);
                    Particle spark = new SparkParticle(pos, -(0.8f * Vector2.UnitX).RotatedBy(rotation), Color.OrangeRed, 0.4f, 10);
                    spark.Spawn();
                }
            }
            else if (modPlayer.SolarEnchCharge >= 240)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with {Pitch = -0.6f, Volume = 0.8f}, player.Center);

                bool wizBoost = modPlayer.ForceEffect<SolarEnchant>();
                int multiplier = wizBoost ? 2 : 1;
                int damage = 3200 * multiplier;
                int speed = wizBoost ? 17 : 13;

                Projectile.NewProjectile(player.GetSource_EffectItem<SolarFlareEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<SolarEnchFlare>(), (int)(damage * player.ActualClassDamage(DamageClass.Melee)), 1f, player.whoAmI, ai2: speed);

                modPlayer.SolarEnchCharge = 0;
            }
            else if (modPlayer.SolarEnchCharge > 0)
            {
                modPlayer.SolarEnchCharge--;
            }
        }
    }
}
