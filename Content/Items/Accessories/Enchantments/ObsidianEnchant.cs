using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ObsidianEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override Color nameColor => new(69, 62, 115);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 50000;
        }

        public override void UpdateInventory(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            player.AddEffect<AshWoodEffect>(item);
            player.AddEffect<AshWoodFireballs>(item);
            player.AddEffect<ObsidianEffect>(item);

            player.lavaImmune = true;
            player.fireWalk = true;
            //player.buffImmune[BuffID.OnFire] = true;

            //in lava effects
            if (player.lavaWet)
            {
                player.gravity = Player.defaultGravity;
                player.ignoreWater = true;
                player.accFlipper = true;

                player.AddBuff(ModContent.BuffType<ObsidianLavaWetBuff>(), 600);
            }

            if (modPlayer.ObsidianCD > 0)
                modPlayer.ObsidianCD--;

            bool triggerFromDebuffs = false;
            if (modPlayer.ForceEffect<ObsidianEnchant>())
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int type = player.buffType[i];
                    if (type > 0 && type is not BuffID.PotionSickness or BuffID.ManaSickness or BuffID.WaterCandle && Main.debuff[type] && FargowiltasSouls.DebuffIDs.Contains(type))
                        triggerFromDebuffs = true;
                }
            }
            if (triggerFromDebuffs || player.lavaWet || modPlayer.LavaWet)
            {
                player.AddEffect<ObsidianProcEffect>(item);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ObsidianHelm)
            .AddIngredient(ItemID.ObsidianShirt)
            .AddIngredient(ItemID.ObsidianPants)
            .AddIngredient(ItemID.MoltenSkullRose) //molten skull rose
                                                   //.AddIngredient(ItemID.Cascade)
            .AddIngredient(null, "AshWoodEnchant")

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class ObsidianEffect : AccessoryEffect
    {

        public override Header ToggleHeader => null;
    }
    public class ObsidianProcEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<ObsidianEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (player.HasEffect<TerraLightningEffect>())
                return;
            if (player.FargoSouls().ObsidianCD == 0)
            {
                int damage = baseDamage;
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                int cap = player.ForceEffect<ObsidianProcEffect>() ? 200 : 50;
                damage = Math.Min(damage, FargoSoulsUtil.HighestDamageTypeScaling(player, cap));

                if (player.lavaWet || modPlayer.LavaWet)
                    damage = (int)(damage * 1.3f);
                if (!player.ForceEffect<ObsidianProcEffect>())
                    damage = (int)(damage * 0.875f);

                if (damage > 250)
                    damage = 250;

                Projectile.NewProjectile(GetSource_EffectItem(player), target.Center, Vector2.Zero, ModContent.ProjectileType<ObsidianExplosion>(), damage, 0, player.whoAmI);

                modPlayer.ObsidianCD = 50;
            }
        }
    }
}
