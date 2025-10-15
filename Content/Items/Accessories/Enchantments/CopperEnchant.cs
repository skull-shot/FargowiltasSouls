using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class CopperEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(213, 102, 23);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<CopperEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CopperHelmet)
                .AddIngredient(ItemID.CopperChainmail)
                .AddIngredient(ItemID.CopperGreaves)
                .AddIngredient(ItemID.CopperShortsword)
                .AddIngredient(ItemID.ThunderStaff)
                .AddIngredient(ItemID.WandofSparking)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Ranged;
            tooltipColor = null;
            scaling = null;
            return CopperEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class CopperEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<CopperEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.CopperProcCD > 0)
                modPlayer.CopperProcCD--;
            CooldownBarManager.Activate("CopperEnchantCooldown", FargoAssets.GetTexture2D("Content/Items/Accessories/Enchantments", "CopperEnchant").Value, new(213, 102, 23),
                () => Main.LocalPlayer.FargoSouls().CopperProcCD / (60f * 4), true, activeFunction: player.HasEffect<CopperEffect>);
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            bool wetCheck = target.HasBuff(BuffID.Wet) && Main.rand.NextBool(4);
            if ((hitInfo.Crit || wetCheck))
            {
                CopperProc(player, target);
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                if (modPlayer.CopperProcCD > 0)
                    modPlayer.CopperProcCD -= 2;
            }
        }
        public static int BaseDamage(Player player) => (int)((player.ForceEffect<CopperEffect>() ? 150 : 40) * player.ActualClassDamage(DamageClass.Ranged));
        public static void CopperProc(Player player, NPC target)
        {
            if (!player.HasEffectEnchant<CopperEffect>())
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.CopperProcCD <= 0)
            {
                bool forceEffect = modPlayer.ForceEffect<CopperEnchant>();
                target.AddBuff(BuffID.Electrified, 180);

                int arcs = 5;
                int cdLength = 60 * 5;

                if (forceEffect)
                {
                    arcs = 8;
                }

                int damage = BaseDamage(player);

                Projectile.NewProjectile(player.GetSource_EffectItem<CopperEffect>(), player.Center, player.DirectionTo(target.Center) * 20, ModContent.ProjectileType<CopperLightning>(), 
                    damage, 0f, modPlayer.Player.whoAmI, player.DirectionTo(target.Center).ToRotation(), damage, ai2: arcs);

                modPlayer.CopperProcCD = cdLength;
            }
        }
    }
}
