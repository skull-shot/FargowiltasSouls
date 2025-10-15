using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.Lifelight;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasSouls.Content.Items.Summons
{
    [LegacyName("FragilePixieLamp")]   
    public class CrystallineEffigy : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Summons", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 44;
            Item.useAnimation = 120;
            Item.useTime = 120;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.noUseGraphic = false;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyGoldBar", 4)
                .AddIngredient(ItemID.PixieDust, 2)
                .AddIngredient(ItemID.SoulofLight, 2)
                .AddIngredient(ItemID.CrystalShard, 3)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .Register();
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.ZoneHallow && Main.dayTime)
                return !NPC.AnyNPCs(NPCType<Lifelight>()); //not (x or y)
            return false;
        }
        public Vector2 OriginalLocation = Vector2.Zero;
        public override void UseItemFrame(Player player)
        {
            float shake = (120 - player.itemAnimation) / 20;
            player.itemLocation += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
        }
        public override void HoldItem(Player player)
        {
            if (player.itemAnimation == player.itemAnimationMax)
            {
                OriginalLocation = player.itemLocation;
            }
            Vector2 ItemCenter = player.itemLocation + new Vector2(Item.width / 2 * player.direction, -Item.height / 2);
            if (player.itemAnimation > 1)
            {
                SoundEngine.PlaySound(SoundID.Pixie, player.Center);
                Dust.NewDust(ItemCenter, 0, 0, DustID.Pixie, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), 100, new Color(), 0.75f);
            }
            if (player.itemAnimation == 1)
            {
                if (player.ZoneHallow && Main.dayTime)
                {
                    SoundEngine.PlaySound(SoundID.Shatter, player.Center);
                    //shatter effect
                    for (int i = 0; i < 50; i++)
                        Dust.NewDust(ItemCenter - Item.Size / 2, Item.width, Item.height, DustID.HallowedTorch, player.velocity.X, player.velocity.Y, 100, new Color(), 1f);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath3, player.Center);
                    //play fail sound
                }
            }
        }

        public override bool? UseItem(Player Player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(Player, NPCType<Lifelight>());
            return true;
        }
    }
}
