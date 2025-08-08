using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AncientShadowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(94, 85, 220);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().AncientShadowEnchantActive = true;
            player.AddEffect<AncientShadowDarkness>(Item);
            player.AddEffect<ShadowBalls>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.AncientShadowHelmet)
            .AddIngredient(ItemID.AncientShadowScalemail)
            .AddIngredient(ItemID.AncientShadowGreaves)
            //.AddIngredient(ItemID.AncientNecroHelmet);
            //.AddIngredient(ItemID.AncientGoldHelmet);
            .AddIngredient<ShadowEnchant>()
            .AddIngredient(ItemID.ShadowFlameKnife)
            .AddIngredient(ItemID.ShadowFlameHexDoll)
            //dart rifle
            //toxicarp

            .AddTile(TileID.CrystalBall)
            .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Magic;
            tooltipColor = null;
            scaling = null;
            return (int)(40 * Main.LocalPlayer.ActualClassDamage(DamageClass.Magic));
        }
    }
    public class AncientShadowDarkness : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<AncientShadowEnchant>();
        public override void PostUpdateMiscEffects(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.AncientShadowFlameCooldown > 0)
                modPlayer.AncientShadowFlameCooldown--;
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if ((projectile == null || projectile.type != ProjectileID.ShadowFlame) && Main.rand.NextBool(5))
                target.AddBuff(BuffID.Darkness, 600, true);
        }
    }
}
