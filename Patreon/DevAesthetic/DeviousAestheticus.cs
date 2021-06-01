﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles;
using System.Collections.Generic;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    public class DeviousAestheticus : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devious Aestheticus");
            Tooltip.SetDefault(
@"Shot spread scales with up to 8 empty minion slots
'If you're seeing this, You've been in a coma for 20 years, I don't know where this message will be, but please wake up'");
        }

        public override void SetDefaults()
        {
            item.damage = 222;
            item.summon = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 1;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DevRocket>();
            item.shootSpeed = 10f;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> Patreon Item <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockback)
        {
            damage /= 4;

            int p = Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), type, damage, knockback, player.whoAmI);

            float minionSlotsUsed = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && !Main.projectile[i].hostile && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].minion)
                    minionSlotsUsed += Main.projectile[i].minionSlots;
            }

            float modifier = (player.maxMinions - minionSlotsUsed) / 2;
            if (modifier < 0)
                modifier = 0;
            if (modifier > 4)
                modifier = 4;

            float spread = MathHelper.ToRadians(75);
            FargoGlobalProjectile.SplitProj(Main.projectile[p], (int)modifier, spread, 1);

            return false;
        }
    }
}
