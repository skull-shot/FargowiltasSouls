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
            .AddIngredient(ItemID.Uzi)
            //venus magnum
            .AddIngredient(ItemID.TacticalShotgun)
            //.AddIngredient(ItemID.StrangeGlowingMushroom);

            .AddTile(TileID.CrystalBall)
            .Register();
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

            if (projectile != null && (projectile.penetrate > 1 || projectile.penetrate < 0) && projectile.maxPenetrate == projectile.penetrate)
            {
                SpawnShrooms(player, target, baseDamage);
            }
        }

        public static void SpawnShrooms(Player player, NPC target, int baseDamage)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            int damage = baseDamage / 4;
            int num = 3;
            if (modPlayer.ForceEffect<ShroomiteEnchant>())
            {
                num = 5;
                damage = (int)(baseDamage / 2.5f);
                if (damage > 115)
                    damage = 115;
            }
            Projectile[] projs = FargoSoulsUtil.XWay(num, player.GetSource_EffectItem<ShroomiteShroomEffect>(), target.Center, ModContent.ProjectileType<ShroomiteShroom>(), 16, damage, 0f);

            foreach (Projectile p in projs)
            {
                p.velocity = p.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.15f);
                p.velocity *= Main.rand.NextFloat(0.8f, 1.2f);
            }

            modPlayer.ShroomiteCD = 20;
        }
    }
}
