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
    public class ShadowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(66, 53, 111);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ShadowBalls>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadowHelmet)
                .AddIngredient(ItemID.ShadowScalemail)
                .AddIngredient(ItemID.ShadowGreaves)
                .AddIngredient(ItemID.Musket)
                .AddIngredient(ItemID.TheBreaker)
                .AddIngredient(ItemID.ShadowOrb)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Magic;
            tooltipColor = null;
            scaling = null;
            return ShadowBalls.BaseDamage(Main.LocalPlayer);
        }
    }
    public class ShadowBalls : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShadowEnchant>();
        public override bool ExtraAttackEffect => true;
        public static int BaseDamage(Player player)
        {
            int dmg = player.FargoSouls().AncientShadowEnchantActive ? 40 : 25;
            return (int)(dmg * player.ActualClassDamage(DamageClass.Magic));
        }
        public override void PostUpdateEquips(Player player)
        {
            if (!HasEffectEnchant(player))
                return;
            if (player.whoAmI == Main.myPlayer)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                int currentOrbs = player.ownedProjectileCounts[ModContent.ProjectileType<ShadowEnchantOrb>()];

                int max = 2;
                bool ancientShadow = modPlayer.AncientShadowEnchantActive;
                bool forceEffect = modPlayer.ForceEffect<ShadowEnchant>();

                if (modPlayer.TerrariaSoul)
                {
                    max = 5;
                }
                else if (forceEffect && ancientShadow) //ancient shadow force
                {
                    max = 4;
                }
                else if (ancientShadow || (forceEffect)) //ancient shadow or normal shadow force
                {
                    max = 3;
                }

                //spawn for first time
                if (currentOrbs == 0 && modPlayer.ShadowOrbRespawnTimer <= 0)
                {
                    float rotation = 2f * (float)Math.PI / max;

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        int p = Projectile.NewProjectile(player.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowEnchantOrb>(), 0, 10f, player.whoAmI, 0, rotation * i);
                        Main.projectile[p].FargoSouls().CanSplit = false;
                    }
                }
                //equipped somwthing that allows for more or less, respawn, only once every 10 seconds to prevent exploit
                else if ((currentOrbs < max && modPlayer.ShadowOrbRespawnTimer <= 0) || currentOrbs > max)
                {
                    modPlayer.ShadowOrbRespawnTimer = 60 * 10;

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<ShadowEnchantOrb>() && proj.owner == player.whoAmI)
                        {
                            proj.Kill();
                        }
                    }

                    float rotation = 2f * (float)Math.PI / max;

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        int p = Projectile.NewProjectile(GetSource_EffectItem(player), spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowEnchantOrb>(), 0, 10f, player.whoAmI, 0, rotation * i);
                        Main.projectile[p].FargoSouls().CanSplit = false;
                    }
                }
                modPlayer.ShadowOrbRespawnTimer--;
            }
        }
    }
}
