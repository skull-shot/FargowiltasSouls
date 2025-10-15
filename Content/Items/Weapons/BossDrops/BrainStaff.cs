﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class BrainStaff : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/BossDrops", Name);
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<BrainMinion>();
            Item.shootSpeed = 10f;
            //Item.buffType = ModContent.BuffType<BrainMinion>();
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 2);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<Buffs.Minions.BrainMinionBuff>(), 2);
            Vector2 spawnPos = Main.MouseWorld;
            float usedminionslots = 0;
            var minions = Main.projectile.Where(x => x.minionSlots > 0 && x.owner == player.whoAmI && x.active);
            foreach (Projectile minion in minions)
                usedminionslots += minion.minionSlots;
            if (player.ownedProjectileCounts[type] == 0 && usedminionslots != player.maxMinions) //only spawn brain minion itself when the player doesnt have any, and if minion slots aren't maxxed out
            {
                player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback);
            }
            player.SpawnMinionOnCursor(source, player.whoAmI, ModContent.ProjectileType<CreeperMinion>(), Item.damage, knockback, default, Main.rand.NextVector2Circular(10, 10));
            return false;
        }
    }
}