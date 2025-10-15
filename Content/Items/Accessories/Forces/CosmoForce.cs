﻿using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class CosmoForce : BaseForce
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<CosmosMoonEffect>()]; //doing it like this solely so the tooltip displays the decreased cooldown
        
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Enchants[Type] =
            [
                ModContent.ItemType<MeteorEnchant>(),
                ModContent.ItemType<WizardEnchant>(),
                ModContent.ItemType<SolarEnchant>(),
                ModContent.ItemType<VortexEnchant>(),
                ModContent.ItemType<NebulaEnchant>(),
                ModContent.ItemType<StardustEnchant>()
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            //meme speed, solar flare,
            SetActive(player);
            modPlayer.CosmosForce = true;
            modPlayer.WizardEnchantActive = true;
            player.AddEffect<MeteorMomentumEffect>(Item);
            player.AddEffect<StardustEffect>(Item);
            if (player.AddEffect<CosmoForceEffect>(Item))
                player.AddEffect<CosmosMoonEffect>(Item);

            if (!player.HasEffect<CosmoForceEffect>())
            {
                //meteor shower
                MeteorEnchant.AddEffects(player, Item);
                //solar shields
                //player.AddEffect<SolarEffect>(Item);
                player.AddEffect<SolarFlareEffect>(Item);
                //stealth, voids, pet
                VortexEnchant.AddEffects(player, Item);
                //boosters
                player.AddEffect<NebulaEffect>(Item);
                //guardian and time freeze
                player.AddEffect<StardustMinionEffect>(Item);
                player.AddEffect<StardustEffect>(Item);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);

            recipe.AddIngredient(ModContent.ItemType<Eridanium>(), 5);

            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Generic;
            tooltipColor = null;
            scaling = null;
            return CosmosMoonEffect.BaseDamage(Main.LocalPlayer);
        }
    }
    public class CosmosMoonEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<CosmoForce>();
        public override bool ExtraAttackEffect => true;
        public static int BaseDamage(Player player) => FargoSoulsUtil.HighestDamageTypeScaling(player, 1200);
        public override void PostUpdate(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.TerrariaSoul)
            {
                if (!player.ItemTimeIsZero)
                {
                    modPlayer.CosmosMoonTimer += 2f * modPlayer.AttackSpeed;
                    if (modPlayer.CosmosMoonTimer >= LumUtils.SecondsToFrames(3) && player.whoAmI == Main.myPlayer)
                    {

                        NPC result = null;
                        float range = 1200;
                        for (int i = 0; i < 200; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.CanBeChasedBy() && Collision.CanHitLine(player.Center, 1, 1, npc.Center, 1, 1))
                            {
                                float foundRange = player.Distance(npc.Center);
                                if (!(range <= foundRange))
                                {
                                    range = foundRange;
                                    result = npc;
                                }
                            }
                        }
                        int npcid = result != null ? result.whoAmI : -1;

                        Projectile.NewProjectileDirect(player.GetSource_EffectItem<CosmosMoonEffect>(), player.Center, player.DirectionTo(Main.MouseWorld) * 7, ModContent.ProjectileType<TerrariaSoulMoon>(), BaseDamage(player), 1, player.whoAmI, MathHelper.Pi, ai1: npcid, ai2: modPlayer.CosmosMoonCycle);
                        modPlayer.CosmosMoonTimer = 0;
                        modPlayer.CosmosMoonCycle++;
                        modPlayer.CosmosMoonCycle %= 4;
                    }
                }
                return;
            }

            if (player.HeldItem != null && player.HeldItem.damage > 0 && (player.controlUseItem || !player.ItemTimeIsZero))
            {
                modPlayer.CosmosMoonTimer += 2f * modPlayer.AttackSpeed;
                int moonCount = player.ownedProjectileCounts[ModContent.ProjectileType<CosmosForceMoon>()];
                if (modPlayer.CosmosMoonTimer >= LumUtils.SecondsToFrames(3) && player.whoAmI == Main.myPlayer && moonCount < 4)
                {
                    int moonDamage = FargoSoulsUtil.HighestDamageTypeScaling(player, 850);

                    Projectile.NewProjectileDirect(player.GetSource_EffectItem<CosmosMoonEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<CosmosForceMoon>(), moonDamage, 1, player.whoAmI, MathHelper.Pi, ai2: modPlayer.CosmosMoonCycle);
                    modPlayer.CosmosMoonTimer = 0;
                    modPlayer.CosmosMoonCycle++;
                    modPlayer.CosmosMoonCycle %= 4;
                }
            }
            
        }
    }
    public class CosmoForceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        //public override int ToggleItemType => ModContent.ItemType<CosmoForce>();
    }
}
