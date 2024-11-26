using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class CobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(61, 164, 196);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<CobaltEffect>(Item);
            player.AddEffect<AncientCobaltEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyCobaltHead")
            .AddIngredient(ItemID.CobaltBreastplate)
            .AddIngredient(ItemID.CobaltLeggings)
            .AddIngredient(null, "AncientCobaltEnchant")
            .AddIngredient(ItemID.ScarabBomb, 10)
            .AddIngredient(ItemID.DD2ExplosiveTrapT1Popper)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class CobaltEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<CobaltEnchant>();

        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            bool canProc = true;
            if (!HasEffectEnchant(player))
                canProc = player.FargoSouls().WeaponUseTimer <= 0;
            if (player.whoAmI == Main.myPlayer && canProc)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();

                int baseDamage = 75;
                int multiplier = 2;
                int cap = 150;

                if (player.ForceEffect<CobaltEffect>())
                {
                    baseDamage = 150;
                    multiplier = 4;
                    cap = 400;
                }

                if (player.HasEffect<EarthForceEffect>() || modPlayer.TerrariaSoul)
                {
                    baseDamage = 600;
                    multiplier = 5;
                    cap = 2500;
                }

                int explosionDamage = baseDamage + info.Damage * multiplier;
                if (explosionDamage > cap)
                    explosionDamage = cap;

                Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(player.GetSource_Accessory(player.EffectItem<CobaltEffect>()), player.Center, Vector2.Zero, ModContent.ProjectileType<CobaltExplosion>(), (int)(explosionDamage * player.ActualClassDamage(DamageClass.Melee)), 0f, Main.myPlayer);
                if (p != null)
                    p.FargoSouls().CanSplit = false;
            }
        }
    }
}
