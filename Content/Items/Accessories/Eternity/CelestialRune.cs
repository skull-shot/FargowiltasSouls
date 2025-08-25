using System;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Accessories.VerdantDoomsayerMask;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class CelestialRune : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<MarkedforDeathBuff>()] = true;
            player.AddEffect<CelestialRuneAttacks>(Item);
          //player.AddEffect<CelestialRuneOnhit>(Item);
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            Player player = Main.LocalPlayer;
            damageClass = GetRuneClassDamage(player);

            int damage = (int)(CelestialRuneAttacks.BaseDamage(Main.LocalPlayer) * player.ActualClassDamage(damageClass));
            if (damageClass == DamageClass.Summon)
                damage /= (int)player.ActualClassDamage(DamageClass.Summon);
            if (damageClass == DamageClass.Ranged || damageClass == DamageClass.Magic)
                damage *= 2;

            tooltipColor = null;
            scaling = null;
            return damage;
        }
        public static DamageClass GetRuneClassDamage(Player player)
        {
            DamageClass damageClass = DamageClass.Generic;
            if (player.HeldItem.IsWeaponWithDamageClass() && (player.HeldItem.DamageType == DamageClass.Melee || player.HeldItem.DamageType == DamageClass.Ranged || player.HeldItem.DamageType == DamageClass.Magic || player.HeldItem.DamageType == DamageClass.Summon))
                damageClass = player.HeldItem.DamageType;
            if (player.HeldItem.DamageType == DamageClass.MeleeNoSpeed)
                damageClass = DamageClass.Melee;
            if (player.HeldItem.DamageType == DamageClass.SummonMeleeSpeed)
                damageClass = DamageClass.Summon;
            return damageClass;
        }
    }

    public class CelestialRuneAttacks : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<CelestialRune>();
        public override bool ExtraAttackEffect => true;
        public static int BaseDamage(Player player) => 50;
        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.whoAmI == Main.myPlayer && modPlayer.AdditionalAttacksTimer <= 0)
            {
                modPlayer.AdditionalAttacksTimer = 100;

                Vector2 position = player.Center;
                Vector2 velocity = Vector2.Normalize(Main.MouseWorld - position);

                if (damageType.CountsAsClass(DamageClass.Melee)) //fireball
                {
                    SoundEngine.PlaySound(SoundID.Item34, position);
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(GetSource_EffectItem(player), position, velocity.RotatedByRandom(Math.PI / 6) * Main.rand.NextFloat(6f, 10f),
                            ModContent.ProjectileType<CelestialRuneFireball>(), (int)(BaseDamage(player) * player.ActualClassDamage(DamageClass.Melee)), 9f, player.whoAmI);
                    }
                }
                if (damageType.CountsAsClass(DamageClass.Ranged)) //lightning
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        float ai1 = Main.rand.Next(100);
                        Vector2 vel = Vector2.Normalize(velocity.RotatedByRandom(Math.PI / 4)).RotatedBy(MathHelper.ToRadians(5) * i) * 7f;
                        Projectile.NewProjectile(GetSource_EffectItem(player), position, vel, ModContent.ProjectileType<CelestialRuneLightningArc>(),
                            (int)(BaseDamage(player) * player.ActualClassDamage(DamageClass.Ranged) * 2), 1f, player.whoAmI, velocity.ToRotation(), ai1);
                    }
                }
                if (damageType.CountsAsClass(DamageClass.Magic)) //ice mist
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), position, velocity * 4.25f, ModContent.ProjectileType<CelestialRuneIceMist>(), (int)(BaseDamage(player) * player.ActualClassDamage(DamageClass.Magic) * 2), 4f, player.whoAmI);
                }
                if (damageType.CountsAsClass(DamageClass.Summon)) //ancient vision
                {
                    FargoSoulsUtil.NewSummonProjectile(GetSource_EffectItem(player), position, velocity * 16f, ModContent.ProjectileType<CelestialRuneAncientVision>(), (int)BaseDamage(player), 3f, player.whoAmI);
                }
            }
        }
    }
  /*public class CelestialRuneOnhit : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<CelestialRune>();
        public override bool MinionEffect => true;
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            int damage = info.Damage;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.HurtTimer <= 0)
            {
                modPlayer.HurtTimer = 20;

                if (modPlayer.MoonChalice)
                {
                    int dam = 50;
                    if (modPlayer.MasochistSoul)
                        dam *= 2;
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Main.rand.NextVector2Circular(20, 20),
                                ModContent.ProjectileType<AncientVision>(), (int)(dam * player.ActualClassDamage(DamageClass.Summon)), 6f, player.whoAmI);
                    }
                }
                else
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, new Vector2(0, -10), ModContent.ProjectileType<AncientVision>(),
                        (int)(40 * player.ActualClassDamage(DamageClass.Summon)), 3f, player.whoAmI);
                }
            }
        }
    }*/
}
