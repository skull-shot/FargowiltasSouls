using System;
using System.Linq;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Eternity
{
    public class SandsofTime : SoulsItem
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

            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.UseSound = SoundID.DD2_BetsyFlameBreath with { Pitch = -1f, Volume = 2f};
        }

        public static void PassiveEffects(Player player)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.FargoSouls().SandsofTime = true;
        }
        public static void ActiveEffects(Player player) => PassiveEffects(player);

        public override void UpdateInventory(Player player) => PassiveEffects(player);
        public override void UpdateVanity(Player player) => PassiveEffects(player);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.04f;
            ActiveEffects(player);
        }

        public static void Use(Player player, Item item)
        {
            if (player.itemTime == player.itemTimeMax / 2 && player.lastDeathPostion != Vector2.Zero && player.FargoSouls().SandsOfTimePosition != Vector2.Zero)
            {
                Vector2 tpPos = (item.type == ModContent.ItemType<MasochistSoul>() || item.type == ModContent.ItemType<EternitySoul>() || player.FargoSouls().MasochistSoul) ? player.lastDeathPostion : player.FargoSouls().SandsOfTimePosition;

                player.immune = true;
                player.immuneTime = 20;
                for (int index = 0; index < 70; ++index)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GemTopaz, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, new Color(), 1.5f);
                    Main.dust[d].velocity *= 4f;
                    Main.dust[d].noGravity = true;
                }

                player.grappling[0] = -1;
                player.grapCount = 0;
                for (int index = 0; index < Main.maxProjectiles; ++index)
                {
                    if (Main.projectile[index].active && Main.projectile[index].owner == player.whoAmI && Main.projectile[index].aiStyle == 7)
                        Main.projectile[index].Kill();
                }

                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 teleport = tpPos;
                    if (DungeonWalls.Contains(Framing.GetTileSafely(tpPos).WallType) && !NPC.downedBoss3)
                        teleport = new Vector2(Main.dungeonX*16+8, Main.dungeonY*16 - 16*3); //dungeon entrance

                    player.Teleport(teleport, 1);
                    player.velocity = Vector2.Zero;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, tpPos.X, tpPos.Y, 1);
                }

                for (int index = 0; index < 70; ++index)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GemTopaz, 0.0f, 0.0f, 150, new Color(), 1.5f);
                    Main.dust[d].velocity *= 4f;
                    Main.dust[d].noGravity = true;
                }
            }
        }
        private static readonly int[] DungeonWalls =
        [
            WallID.BlueDungeon, WallID.BlueDungeonSlab, WallID.BlueDungeonSlabUnsafe, WallID.BlueDungeonTile, WallID.BlueDungeonTileUnsafe, WallID.BlueDungeonUnsafe, WallID.GreenDungeon, WallID.GreenDungeonSlab, WallID.GreenDungeonSlabUnsafe,
            WallID.GreenDungeonTile, WallID.GreenDungeonTileUnsafe, WallID.GreenDungeonUnsafe, WallID.PinkDungeon, WallID.PinkDungeonSlab, WallID.PinkDungeonSlabUnsafe, WallID.PinkDungeonTile, WallID.PinkDungeonTileUnsafe, WallID.PinkDungeonUnsafe
        ];
        public override void UseItemFrame(Player player) => Use(player, Item);
        public override bool? UseItem(Player player) => true;
    }
}