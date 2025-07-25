using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class PureHeart : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, 6));
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.PureHeart = true;

            //darkened effect
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.RottingBuff>()] = true;
            player.moveSpeed += 0.1f;
            fargoPlayer.DarkenedHeartItem = Item;
            player.AddEffect<DarkenedHeartEaters>(Item);
            if (fargoPlayer.DarkenedHeartCD > 0)
                fargoPlayer.DarkenedHeartCD--;

            //gutted effect
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.BloodthirstyBuff>()] = true;
            player.statLifeMax2 += player.statLifeMax / 10;
            player.AddEffect<GuttedHeartEffect>(Item);
            player.AddEffect<GuttedHeartMinions>(Item);

            //gelic effect
            player.FargoSouls().GelicWingsItem = Item;
            //player.AddEffect<GelicWingJump>(Item);
            player.AddEffect<GelicWingSpikes>(Item);
            player.FargoSouls().WingTimeModifier += .3f;

            //pungent effect
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.AddEffect<PungentEyeballCursor>(Item);
            player.FargoSouls().PungentEyeball = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<DarkenedHeart>())
            .AddIngredient(ModContent.ItemType<GuttedHeart>())
            .AddIngredient(ModContent.ItemType<GelicWings>())
            .AddIngredient(ModContent.ItemType<PungentEyeball>())
            .AddIngredient(ItemID.PurificationPowder, 30)
            .AddIngredient(ItemID.GreenSolution, 50)
            .AddIngredient(ItemID.ChlorophyteBar, 5)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)

            .Register();
        }
    }
}