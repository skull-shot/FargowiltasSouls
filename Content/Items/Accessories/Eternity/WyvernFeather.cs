using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Projectiles.Accessories.BionomicCluster;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    [AutoloadEquip(EquipType.Face)]
    public class WyvernFeather : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Accessories/Eternity", Name);
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 4);
        }
        public static void PassiveEffects(Player player, Item item)
        {
            player.AddEffect<StabilizedGravity>(item);
        }
        public static void ActiveEffects(Player player, Item item)
        {
            PassiveEffects(player, item);
            player.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
            player.AddEffect<WyvernBalls>(item);
        }
        public override void UpdateInventory(Player player)
        {
            PassiveEffects(player, Item);
        }

        public override void UpdateVanity(Player player)
        {
            PassiveEffects(player, Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
        }
    }
    public class StabilizedGravity : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LithosphericHeader>();
        public override int ToggleItemType => ModContent.ItemType<WyvernFeather>();

        public override void PostUpdateMiscEffects(Player player)
        {
            player.gravity = Math.Max(player.gravity, Player.defaultGravity);

            if (!player.ignoreWater && Collision.WetCollision(player.position, player.width, player.height) && !player.shimmerWet && !player.trident && !player.merman)
            {
                player.ignoreWater = true; // allow full movement then restrict the horizontal
                float speedLoss = player.honeyWet ? 0.75f : 0.5f; // 25% speed in honey, 50% otherwise
                player.position.X -= speedLoss * player.velocity.X; // simulate slower horizontal movement
            }
        }
    }
    public class WyvernBalls : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LithosphericHeader>();
        public override int ToggleItemType => ModContent.ItemType<WyvernFeather>();
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.velocity.Y != 0 && ++modPlayer.WyvernBallsCD > 180)
            {
                modPlayer.WyvernBallsCD = 0;
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center,
                        Main.rand.NextFloat(6f, 12f) * Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi),
                        ModContent.ProjectileType<FlightBall>(), 0, 0f, player.whoAmI);
                }
            }
        }
    }
}