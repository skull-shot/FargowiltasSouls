using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Projectiles.Accessories.BionomicCluster;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace FargowiltasSouls.Content.Items.Accessories
{
    //[AutoloadEquip(EquipType.Neck)]
    public class HallowedPendant : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 7);
        }
        public static void ActiveEffects(Player player, Item item)
        {
            player.longInvince = true;
            player.AddEffect<HallowedPendantEffect>(item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddIngredient(ItemID.StarVeil)
                .AddIngredient(ItemID.HoneyComb)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddIngredient(ItemID.BeeCloak)
                .AddIngredient(ItemID.CrossNecklace)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    public class HallowedPendantEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override int ToggleItemType => ModContent.ItemType<HallowedPendant>();
        public override void PostUpdateEquips(Player player)
        {
            // onhit dodge
        }
        public override void OnHitByEither(Player player, NPC npc, Projectile proj)
        {
            if (EffectItem(player).type == ModContent.ItemType<HallowedPendant>())
                PendantRays(player, 450, 500);
        }
        public static void PendantRays(Player player, int baseDamage, int maxDistance)
        {
            List<NPC> enemies = Main.npc.Where(n => n.Alive() && n.CanBeChasedBy() && n.DistanceSQ(player.Center) < maxDistance * maxDistance && Collision.CanHitLine(player.Center, 0, 0, n.Center, 0, 0)).OrderBy(n => n.Distance(player.Center)).ToList();
            for (int i = 0; i < 4; i++)
            {
                if (i >= enemies.Count)
                    break;
                NPC npc = enemies[i];
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 vel = (npc.Center - player.Center) * maxDistance * 0.01f / 660;
                    Projectile.NewProjectile(player.GetSource_EffectItem<HallowedPendantEffect>(), player.Center, vel, ModContent.ProjectileType<HallowRay>(), baseDamage, 1f, player.whoAmI);
                }
            }
        }
    }
}