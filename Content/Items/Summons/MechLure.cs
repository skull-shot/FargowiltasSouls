﻿using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.BanishedBaron;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasSouls.Content.Items.Summons
{
    public class MechLure : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Summons", Name);
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType<MechLureProjectile>();
            Item.shootSpeed = 10f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 2);
        }

        public override void AddRecipes()
        {
            CreateRecipe() //change
                .AddRecipeGroup("FargowiltasSouls:AnyMythrilBar", 4)
                .AddIngredient(ItemID.EnchantedNightcrawler, 3)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddIngredient(ItemID.ArmoredCavefish, 1)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .Register();
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.ZoneBeach && Player.wet)
                return !NPC.AnyNPCs(NPCType<BanishedBaron>()) && Player.ownedProjectileCounts[Item.shoot] <= 0; //not (x or y)
            return false;
        }

        public override bool? UseItem(Player player)
        {
            /*
            if (player.whoAmI == Main.myPlayer)
            {
                // If the player using the item is the client
                // (explicitely excluded serverside here)
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/BaronSummon"), player.Center);

                if (FargoSoulsUtil.HostCheck)
                {
                    // If the player is not in multiplayer, spawn directly
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<BanishedBaron>());
                }
                else
                {
                    // If the player is in multiplayer, request a spawn
                    // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, set in NPC code
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<BanishedBaron>());
                }
            }
            */
            return true;
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = Language.GetTextValue("Mods.FargowiltasSouls.Items.MechLure.TooltipTravelingMerchant");
            tooltips.Add(new TooltipLine(Mod, "TooltipTravelingMerchant",
                $"[i:Fargowiltas/TravellingMerchant] [c/AAAAAA:{text}]"));
        }
    }
}
