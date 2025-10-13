using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerraForce : BaseForce
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
    [AccessoryEffectLoader.GetEffect<ParryEffect>()];
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<CopperEnchant>(),
                ModContent.ItemType<TinEnchant>(),
                ModContent.ItemType<IronEnchant>(),
                ModContent.ItemType<LeadEnchant>(),
                ModContent.ItemType<SilverEnchant>(),
                ModContent.ItemType<TungstenEnchant>(),
                ModContent.ItemType<ObsidianEnchant>()
            ];

            Main.RegisterItemAnimation(Item.type, new DrawAnimationRectangularV(6, 5, 10));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void UpdateInventory(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
            IronEnchant.PassiveEffects(player, Item);
        }
        public override void UpdateVanity(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
            IronEnchant.PassiveEffects(player, Item);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);

            player.AddEffect<TerraEffect>(Item);
            player.AddEffect<TerraLightningEffect>(Item);
            // tin
            player.AddEffect<TinEffect>(Item);
            // copper
            player.AddEffect<CopperEffect>(Item);
            // iron
            IronEnchant.AddEffects(player, Item);
            // lead
            player.AddEffect<LeadEffect>(Item);
            // silver
            player.AddEffect<SilverEffect>(Item);
            player.AddEffect<ParryEffect>(Item);
            // tungsten
            player.AddEffect<TungstenEffect>(Item);
            // obsidian
            ObsidianEnchant.AddEffects(player, Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Generic;
            tooltipColor = null;
            scaling = null;
            return TerraLightningEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class TerraEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
    }
    public class TerraLightningEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public static int BaseDamage(Player player) => FargoSoulsUtil.HighestDamageTypeScaling(player, 900);
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TerraProcCD > 0)
                modPlayer.TerraProcCD--;
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            bool wetCheck = target.HasBuff(BuffID.Wet);
            if ((hitInfo.Crit || wetCheck))
            {
                LightningProc(player, target);
            }
        }

        public static void LightningProc(Player player, NPC target, float damageMultiplier = 1f)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TerraProcCD == 0 && player.HasEffect<CopperEffect>())
            {
                int cdLength = 300;

                // cooldown scaling from 2x to 1x depending on how recently you got hurt
                /*
                int maxHurtTime = 60 * 30;
                if (modPlayer.TimeSinceHurt < maxHurtTime)
                {
                    float multiplier = 2f - (modPlayer.TimeSinceHurt / maxHurtTime) * 1f;
                    cdLength = (int)(cdLength * multiplier);
                }
                */

                Vector2 ai = target.Center - player.Center;
                Vector2 velocity = Vector2.Normalize(ai) * 20;

                FargoSoulsUtil.NewProjectileDirectSafe(player.GetSource_EffectItem<TerraLightningEffect>(), player.Center, velocity, ModContent.ProjectileType<TerraLightning>(), (int)(BaseDamage(player) * damageMultiplier), 0f, modPlayer.Player.whoAmI, ai.ToRotation());
                float modifier = 1f;
                /*if (player.HasEffect<TinEffect>() && !modPlayer.Eternity)
                {
                    modPlayer.TinCrit += 5;
                    if (modPlayer.TinCrit > modPlayer.TinCritMax)
                        modPlayer.TinCrit = modPlayer.TinCritMax;
                    else
                        CombatText.NewText(modPlayer.Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.Items.TinEnchant.CritUp", 5));

                    if (modPlayer.TinCrit >= 25)
                        modifier -= 0.4f;
                }*/
                modPlayer.TerraProcCD = (int)(cdLength * modifier);
            }
        }
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.TerraProcCD = 300 * 2;
        }
    }
}
