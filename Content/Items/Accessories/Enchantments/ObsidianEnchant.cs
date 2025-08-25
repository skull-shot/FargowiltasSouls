using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
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
            //player.AddEffect<AshWoodFireballs>(item);
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

            if (modPlayer.ForceEffect<ObsidianEnchant>() || AshWoodEffect.TriggerFromDebuffs(player) || player.lavaWet || modPlayer.LavaWet)
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
                .AddIngredient(null, "AshWoodEnchant")
                .AddIngredient(ItemID.MoltenSkullRose) 
                .AddIngredient(ItemID.Obsidifish)      //.AddIngredient(ItemID.Cascade)

                .AddTile<EnchantedTreeSheet>()

                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            Player player = Main.LocalPlayer;
            scaling = (int)((player.HeldItem.damage + player.FindAmmo(player.HeldItem.useAmmo).damage) * player.ActualClassDamage(player.HeldItem.DamageType));
            if (scaling < 0)
                scaling = 0;

            int softcapMult = (int)(player.ForceEffect<ObsidianProcEffect>() ? 2.5 : 1);
            if (scaling > 50 * softcapMult)
                scaling = ((100 * softcapMult) + scaling) / 3;

            damageClass = DamageClass.Default;
            tooltipColor = null;
            return 100;
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
            if (!HasEffectEnchant(player))
                return;
            if (player.FargoSouls().ObsidianCD == 0)
            {
                float explosionDamage = baseDamage;
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                bool force = player.ForceEffect<ObsidianProcEffect>();
                float softcapMult = force ? 2.5f : 1f;

                if (force && (AshWoodEffect.TriggerFromDebuffs(player) || player.lavaWet || modPlayer.LavaWet)) // this section is just imitating the previous version but cleaner
                    explosionDamage *= 1.5f;

                if (explosionDamage > 50f * softcapMult)
                    explosionDamage = ((100f * softcapMult) + explosionDamage) / 3f;

                Projectile.NewProjectile(GetSource_EffectItem(player), target.Center, Vector2.Zero, ModContent.ProjectileType<ObsidianExplosion>(), (int)explosionDamage, 0, player.whoAmI);

                modPlayer.ObsidianCD = 50;
            }
        }
    }
}
