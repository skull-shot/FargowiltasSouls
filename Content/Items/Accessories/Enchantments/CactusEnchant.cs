using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class CactusEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(121, 158, 29);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().CactusImmune = true;
            player.AddEffect<CactusEffect>(Item);
        }
        public override void UpdateInventory(Player player)
        {
            player.FargoSouls().CactusImmune = true;
        }
        public override void UpdateVanity(Player player)
        {
            player.FargoSouls().CactusImmune = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CactusHelmet)
                .AddIngredient(ItemID.CactusBreastplate)
                .AddIngredient(ItemID.CactusLeggings)
                .AddIngredient(ItemID.CactusSword)
                .AddIngredient(ItemID.PinkPricklyPear)
                .AddIngredient(ItemID.Waterleaf)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class CactusEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<CactusEnchant>();

        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.CactusProcCD > 0)
            {
                modPlayer.CactusProcCD--;
            }
        }

        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            if (player.whoAmI != Main.myPlayer || !HasEffectEnchant(player))
                return;

            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.CactusProcCD == 0)
            {
                CactusSpray(player, player.Center);
                modPlayer.CactusProcCD = 20;
            }
        }
        public static void CactusProc(NPC npc, Player player)
        {
            CactusSpray(player, npc.Center);
        }

        private static void CactusSpray(Player player, Vector2 position)
        {
            int dmg = 16;
            int numNeedles = 8;
            int rangemult = 1;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.ForceEffect<CactusEnchant>())
            {
                dmg = 40;
                numNeedles = 16;
                rangemult = 2;
            }

            for (int i = 0; i < numNeedles; i++)
            {
                int spread = (int)MathHelper.Lerp(0.9f, 4.5f, i);
                int p = Projectile.NewProjectile(player.GetSource_EffectItem<CactusEffect>(), position, Vector2.UnitX.RotatedBy(spread + Main.rand.NextFloat(-0.2f, 0.2f)) * (4 + Main.rand.NextFloat(-0.5f, 0.5f)) * rangemult, ModContent.ProjectileType<CactusNeedle>(), FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 5f);
                if (p != Main.maxProjectiles)
                {
                    Projectile proj = Main.projectile[p];
                    if (proj != null && proj.active)
                    {
                        proj.FargoSouls().CanSplit = false;

                        proj.ai[0] = 1; //these needles can inflict enemies with needled
                    }

                }
            }
        }
    }

}
