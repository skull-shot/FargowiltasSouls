using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shield)]
    public class SlimyShield : SoulsItem
    {
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
            Item.value = Item.sellPrice(0, 1);
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Slimed] = true;

            player.AddEffect<SlimeFallEffect>(Item);
            player.AddEffect<PlatformFallthroughEffect>(Item);

            if (player.AddEffect<SlimyShieldEffect>(Item))
                player.FargoSouls().SlimyShieldItem = Item;
        }
    }
    public class SlimeFallEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SlimyShield>();
        public override bool MutantsPresenceAffects => true;

        public override void PostUpdateEquips(Player player)
        {
            player.maxFallSpeed *= 1.5f;
        }
    }
    public class SlimyShieldEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SlimyShield>();
        public override bool ExtraAttackEffect => true;
    }
    public class PlatformFallthroughEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SlimyShield>();
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.holdDownCardinalTimer[0] > 0 && !modPlayer.LowGround && modPlayer.FallthroughCD <= 0 ||modPlayer.FallthroughCD > 8)
            {
                Tile thisTile = Framing.GetTileSafely(player.Bottom);
                Tile bottomTile = Framing.GetTileSafely(player.Bottom + Vector2.UnitY * 8);

                if (!Collision.SolidCollision(player.BottomLeft, player.width, 16))
                {
                    if (player.velocity.Y >= 0 && (IsPlatform(thisTile.TileType) || IsPlatform(bottomTile.TileType)))
                    {
                        player.position.Y += 2;
                        modPlayer.FallthroughCD = 10;
                    }
                    if (player.velocity.Y == 0)
                    {
                        player.position.Y += 16;
                    }

                }

                static bool IsPlatform(int tileType)
                {
                    return tileType == TileID.Platforms || tileType == TileID.PlanterBox;
                }
            }
        }
    }
}