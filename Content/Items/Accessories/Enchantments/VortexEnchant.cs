﻿using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class VortexEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(0, 242, 170);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Purple;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<VortexEffect>(item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.VortexHelmet)
            .AddIngredient(ItemID.VortexBreastplate)
            .AddIngredient(ItemID.VortexLeggings)
            //vortex wings
            .AddIngredient(ItemID.VortexBeater)
            .AddIngredient(ItemID.Phantasm)
            //chain gun
            //electrosphere launcher
            .AddIngredient(ItemID.SDMG)

                .AddTile<EnchantedTreeSheet>()
            .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass,  out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Ranged;
            tooltipColor = null;
            scaling = null;
            return VortexEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class VortexProjGravity : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        
        public override void PostUpdateEquips(Player player)
        {
            foreach (Projectile toProj in Main.projectile.Where(p => p != null && p.active && p.friendly && p.owner == player.whoAmI))
            {
                foreach (Projectile fromProj in Main.projectile.Where(p => p != null && p.active && p.friendly && p.whoAmI != toProj.whoAmI && p.owner == player.whoAmI && !TungstenEffect.TungstenAlwaysAffectProj(p) && p.FargoSouls().CanSplit && FargoSoulsUtil.CanDeleteProjectile(p, 0)))
                {
                    // if (fromProj.aiStyle != 1)
                    //    continue;
                    Vector2 dif = toProj.Center - fromProj.Center;
                    int distSq = (int)dif.LengthSquared();
                    if (distSq < 1 || float.IsNaN(distSq))
                        continue;
                    int rSquared = distSq;
                    rSquared += 100;
                    Vector2 force = Utils.SafeNormalize(dif, Vector2.UnitY);
                    const float gravityConstant = 9000f; // tweak
                    force *= gravityConstant /* (toProj.Size.Length() * fromProj.Size.Length())*/ / rSquared;
                    fromProj.velocity += force;
                }
            }
        }
    }
    public class VortexEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<VortexEnchant>();
        public override bool ExtraAttackEffect => true;
        public static int BaseDamage(Player player) => (int)((player.ForceEffect<VortexEffect>() ? 5400 : 3000) * player.ActualClassDamage(DamageClass.Ranged));
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.VortexCD <= 0 && player.Distance(target.Hitbox.ClosestPointInRect(player.Center)) > 450)
            {
                bool force = modPlayer.ForceEffect<VortexEnchant>();
                Vector2 velocity = player.DirectionTo(target.Center);
                FargoSoulsUtil.NewProjectileDirectSafe(GetSource_EffectItem(player), player.Center, velocity, ModContent.ProjectileType<VortexLaser>(), BaseDamage(player), 0f, modPlayer.Player.whoAmI, 1f);
                float cd = 10;
                modPlayer.VortexCD = LumUtils.SecondsToFrames(cd);

                if (player.whoAmI == Main.myPlayer)
                    CooldownBarManager.Activate("VortexEnchantCooldown", FargoAssets.GetTexture2D("Content/Items/Accessories/Enchantments", "VortexEnchant").Value, new(0, 242, 170), 
                        () => 1f - Main.LocalPlayer.FargoSouls().VortexCD / (float)LumUtils.SecondsToFrames(cd), activeFunction: player.HasEffect<VortexEffect>);
            }
        }
    }
}
