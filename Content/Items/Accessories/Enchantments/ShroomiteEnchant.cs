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
    public class ShroomiteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override Color nameColor => new(0, 140, 244);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ShroomiteHealEffect>(Item);
            if (player.HasEffect<ShroomiteHealEffect>())
                player.AddEffect<ShroomiteMushroomPriority>(Item);
            player.AddEffect<ShroomiteShroomEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyShroomHead")
            .AddIngredient(ItemID.ShroomiteBreastplate)
            .AddIngredient(ItemID.ShroomiteLeggings)
            //shroomite digging
            //hammush
            .AddIngredient(ItemID.MushroomSpear)
            .AddIngredient(ItemID.PulseBow)
            //venus magnum
            .AddIngredient(ItemID.TacticalShotgun)
            //.AddIngredient(ItemID.StrangeGlowingMushroom);

                .AddTile<EnchantedTreeSheet>()
            .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            Player player = Main.LocalPlayer;
            scaling = (int)((player.HeldItem.damage + player.FindAmmo([player.HeldItem.useAmmo]).damage) * player.ActualClassDamage(DamageClass.Ranged) / (player.ForceEffect<ShroomiteShroomEffect>() ? 2.5 : 4));
            if (scaling < 0)
                scaling = 0;

            if (scaling > 75)
                scaling = (150 + scaling) / 3;

            damageClass = DamageClass.Ranged;
            tooltipColor = null;
            return player.ForceEffect<ShroomiteShroomEffect>() ? 40 : 25;
        }
    }
    public class ShroomiteHealEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShroomiteEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            //if (!player.FargoSouls().TerrariaSoul)
            //    player.shroomiteStealth = true;
        }
    }
    public class ShroomiteMushroomPriority : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShroomiteEnchant>();
    }
    public class ShroomiteShroomEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShroomiteEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.ShroomiteCD > 0)
                modPlayer.ShroomiteCD--;
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (!HasEffectEnchant(player))
                return;

            if (modPlayer.ShroomiteCD > 0)
                return;

            if (projectile != null && ((projectile.penetrate > 1 || projectile.penetrate < 0) && (projectile.maxPenetrate == projectile.penetrate) || projectile.type == ProjectileID.JackOLantern) && projectile.type != ModContent.ProjectileType<ShroomiteShroom>())
            {
                SpawnShrooms(player, target, hitInfo, baseDamage);
            }
        }

        public static void SpawnShrooms(Player player, NPC target, NPC.HitInfo hitInfo, int baseDamage)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float shroomDamage = baseDamage / 4; // 0.25x or 25%
            int num = 3;
            if (modPlayer.ForceEffect<ShroomiteEnchant>())
            {
                num = 5;
                shroomDamage = baseDamage / 2.5f; // 0.4x or 40%
            }
            if (!player.HasEffect<NatureEffect>())
            {
                shroomDamage *= player.ActualClassDamage(DamageClass.Ranged) / player.ActualClassDamage(hitInfo.DamageType);
                shroomDamage = (float)Math.Round(shroomDamage);
            }
            if (shroomDamage > 75f)
                shroomDamage = (150f + shroomDamage) / 3f; // refer to Boreal Wood Enchantment for new softcap
            Projectile[] projs = FargoSoulsUtil.XWay(num, player.GetSource_EffectItem<ShroomiteShroomEffect>(), target.Center, ModContent.ProjectileType<ShroomiteShroom>(), 16, (int)shroomDamage, 0f);

            foreach (Projectile p in projs)
            {
                p.velocity = p.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.15f);
                p.velocity *= Main.rand.NextFloat(0.8f, 1.2f);
            }

            modPlayer.ShroomiteCD = 20;
        }
    }
}
