﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class PalmWoodEnchant : BaseEnchant
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<PalmwoodEffect>()];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(183, 141, 86);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<PalmwoodEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PalmWoodHelmet)
                .AddIngredient(ItemID.PalmWoodBreastplate)
                .AddIngredient(ItemID.PalmWoodGreaves)
                .AddIngredient(ItemID.Coral)
                .AddIngredient(ItemID.Banana)
                .AddIngredient(ItemID.Coconut)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Summon;
            tooltipColor = null;
            scaling = null;
            return (int)(PalmwoodEffect.BaseDamage(Main.LocalPlayer) * Main.LocalPlayer.ActualClassDamage(DamageClass.Summon));
        }
    }
    public class PalmwoodEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<PalmWoodEnchant>();
        public override bool ActiveSkill => Main.LocalPlayer.HasEffectEnchant<PalmwoodEffect>();
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (player.HasEffect<TimberEffect>())
            {
                if (player.Distance(target.Center) > ShadewoodEffect.Range(player, true))
                    return;
                if (player.FargoSouls().PalmWoodForceCD <= 0 && Collision.CanHit(player.Center, 0, 0, target.Center, 0, 0))
                {
                    Vector2 velocity = Vector2.Normalize(target.Center - player.Center) * 18;

                    int damage = 1000;
                    //int damage = hitInfo.SourceDamage;
                    //damage = (int)MathHelper.Clamp(damage, 0, 8000);

                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, velocity, ModContent.ProjectileType<PalmwoodShot>(), damage, 2, player.whoAmI);

                    player.FargoSouls().PalmWoodForceCD = 90;
                }
            }
        }
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (!stunned)
                ActivatePalmwoodSentry(player);
        }
        public static int BaseDamage(Player player)
        {
            int dmg = player.FargoSouls().ForceEffect<PalmWoodEnchant>() ? 140 : 18;
            return dmg;
        }
        public static void ActivatePalmwoodSentry(Player player)
        {
            if (player.HasEffect<PalmwoodEffect>() && player.HasEffectEnchant<PalmwoodEffect>())
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    FargoSoulsPlayer modPlayer = player.FargoSouls();
                    bool forceEffect = modPlayer.ForceEffect<PalmWoodEnchant>();

                    Vector2 mouse = Main.MouseWorld;

                    int maxSpawn = 1;

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<PalmTreeSentry>()] > maxSpawn - 1)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];

                            if (proj.active && proj.type == ModContent.ProjectileType<PalmTreeSentry>() && proj.owner == player.whoAmI)
                            {
                                proj.Kill();
                                break;
                            }
                        }
                    }

                    Vector2 offset = forceEffect ? (-40 * Vector2.UnitX) + (-120 * Vector2.UnitY) : (-41 * Vector2.UnitY);
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), mouse + offset, Vector2.Zero, ModContent.ProjectileType<PalmTreeSentry>(), BaseDamage(player), 0f, player.whoAmI, ai1: -30);
                }
            }
        }
    }
}
