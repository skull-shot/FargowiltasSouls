﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ShadewoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(88, 104, 118);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ShadewoodEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadewoodHelmet)
                .AddIngredient(ItemID.ShadewoodBreastplate)
                .AddIngredient(ItemID.ShadewoodGreaves)
                .AddIngredient(ItemID.ViciousMushroom)
                .AddRecipeGroup("FargowiltasSouls:RambutanOrBloodOrange")
                .AddIngredient(ItemID.Deathweed)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Melee;
            tooltipColor = null;
            scaling = null;
            return ShadewoodEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class ShadewoodEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShadewoodEnchant>();
        public static int Range(Player player, bool forceEffect) => (int)((forceEffect ? 400f : 200f) * (1f + player.FargoSouls().AuraSizeBonus));
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.whoAmI != Main.myPlayer)
                return;
            bool forceEffect = modPlayer.ForceEffect<ShadewoodEnchant>();
            int dist = Range(player, forceEffect);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
                {
                    Vector2 npcComparePoint = FargoSoulsUtil.ClosestPointInHitbox(npc, player.Center);
                    if (player.Distance(npcComparePoint) < dist && (forceEffect || Collision.CanHitLine(player.Center, 0, 0, npcComparePoint, 0, 0)))
                    {
                        //if (!player.HasEffect<TimberEffect>())
                        npc.AddBuff(ModContent.BuffType<SuperBleedBuff>(), 60);
                    }
                        
                }
            }
            if (!MoltenAuraProj.CombinedAura(player))
            {
                int visualProj = ModContent.ProjectileType<ShadewoodAuraProj>();
                if (player.ownedProjectileCounts[visualProj] <= 0)
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, visualProj, 0, 0, Main.myPlayer);
                }
            }

            if (modPlayer.ShadewoodCD > 0)
            {
                modPlayer.ShadewoodCD--;
            }
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            ShadewoodProc(player, target, projectile);
        }
        public static int BaseDamage(Player player)
        {
            int dmg = 12;
            if (player.FargoSouls().ForceEffect<ShadewoodEnchant>())
                dmg *= 3;
            if (player.HasEffect<TimberEffect>())
                dmg = (int)(dmg * 2.4f);
            return (int)(dmg * player.ActualClassDamage(DamageClass.Melee));
        }
        public static void ShadewoodProc(Player player, NPC target, Projectile projectile)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool forceEffect = modPlayer.ForceEffect<ShadewoodEnchant>();

            if (target.HasBuff(ModContent.BuffType<SuperBleedBuff>()) && modPlayer.ShadewoodCD == 0 && (projectile == null || projectile.type != ModContent.ProjectileType<SuperBlood>()) && player.whoAmI == Main.myPlayer)
            {
                modPlayer.ShadewoodCD = 40;
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(player.GetSource_Misc(""), target.Center.X, target.Center.Y - 20, 0f + Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), ModContent.ProjectileType<SuperBlood>(), BaseDamage(player), 0f, Main.myPlayer);
                }

                if (forceEffect)
                {
                    target.AddBuff(BuffID.Ichor, 120);
                }
            }
        }
    }
}
