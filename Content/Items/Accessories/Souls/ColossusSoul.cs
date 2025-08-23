using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Shield)]
    public class ColossusSoul : BaseSoul
    {

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.defense = 10;
            Item.shieldSlot = 4;
        }
        public static readonly Color ItemColor = new(252, 59, 0);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item, 50, 0.2f, 4);
        }
        public static void AddEffects(Player player, Item item, int maxHP, float damageResist, int lifeRegen)
        {
            Player Player = player;
            player.FargoSouls().ColossusSoul = true;
            Player.statLifeMax2 += maxHP;
            Player.endurance += damageResist;
            Player.lifeRegen += lifeRegen;

            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Stoned] = true;
            Player.buffImmune[BuffID.Weak] = true;
            Player.buffImmune[BuffID.BrokenArmor] = true;
            Player.buffImmune[BuffID.Bleeding] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Slow] = true;
            Player.buffImmune[BuffID.Confused] = true;
            Player.buffImmune[BuffID.Silenced] = true;
            Player.buffImmune[BuffID.Cursed] = true;
            Player.buffImmune[BuffID.Darkness] = true;
            Player.AddEffect<ShimmerImmunityEffect>(item);

            // molten shield
            Devilshield.ActiveEffects(Player, item);

            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.noFallDmg = true;
            //charm of myths
            Player.pStone = true;
            //shiny stone
            //Player.shinyStone = true;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Devilshield>()
            .AddIngredient(ItemID.AnkhShield)
            .AddIngredient(ItemID.WormScarf)
            .AddIngredient(ItemID.CharmofMyths)
            .AddIngredient(ItemID.ShinyStone)
            .AddTile<CrucibleCosmosSheet>()
            .Register();
        }
    }
    
    public class ShimmerImmunityEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ItemID.ShimmerCloak;
        
        public override void PostUpdateEquips(Player player)
        {
            player.buffImmune[BuffID.Shimmer] = true;
        }
    }
}