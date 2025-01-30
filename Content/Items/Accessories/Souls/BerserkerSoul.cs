using FargowiltasSouls.Content.Items.Accessories.Essences;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Waist)]
    public class BerserkerSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 4;
        }
        public static readonly Color ItemColor = new(255, 111, 6);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.22f;
            player.GetCritChance(DamageClass.Melee) += 10;

            player.AddEffect<MeleeSpeedEffect>(Item);

            player.FargoSouls().MeleeSoul = true;

            //gauntlet
            player.kbGlove = true;
            player.autoReuseGlove = true;
            player.meleeScaleGlove = true;

            player.counterWeight = 556 + Main.rand.Next(6);
            player.yoyoGlove = true;
            player.yoyoString = true;

            //celestial shell
            player.wolfAcc = true;
            player.accMerman = true;

            if (hideVisual)
            {
                player.hideMerman = true;
                player.hideWolf = true;
            }

            player.lifeRegen += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient<BarbariansEssence>()
            .AddIngredient(ItemID.StingerNecklace)
            .AddIngredient(ItemID.YoyoBag)
            .AddIngredient(ItemID.FireGauntlet)
            .AddIngredient(ItemID.BerserkerGlove)
            .AddIngredient(ItemID.CelestialShell)

            .AddIngredient(ItemID.KOCannon)
            .AddIngredient(ItemID.IceSickle)
            .AddIngredient(ItemID.DripplerFlail)
            .AddIngredient(ItemID.ScourgeoftheCorruptor)
            .AddIngredient(ItemID.Kraken)
            .AddIngredient(ItemID.Flairon)
            .AddIngredient(ItemID.MonkStaffT3)
            .AddIngredient(ItemID.NorthPole)
            .AddIngredient(ItemID.Zenith)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();
        }
    }
    public class MeleeSpeedEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ModContent.ItemType<BerserkerSoul>();
        public override void PostUpdateEquips(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += .2f;
        }
    }
}
