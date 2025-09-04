using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SpiderEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override Color nameColor => new(109, 78, 69);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SpiderEffect>(Item);
            player.GetCritChance(DamageClass.Generic) += 7;
            if (player.FargoSouls().ForceEffect<SpiderEnchant>())
                player.GetCritChance(DamageClass.Generic) += 7;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SpiderMask)
            .AddIngredient(ItemID.SpiderBreastplate)
            .AddIngredient(ItemID.SpiderGreaves)
            .AddIngredient(ItemID.QueenSpiderStaff)
            .AddIngredient(ItemID.SpiderStaff)
            .AddIngredient(ItemID.WebSlinger)
            //web rope coil
            //rainbow string
            //fried egg
            //.AddIngredient(ItemID.SpiderEgg);

                .AddTile<EnchantedTreeSheet>()
            .Register();
        }
        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Summon;
            tooltipColor = null;
            scaling = null;
            return SpiderEnchantSpiderling.SpiderDamage(Main.LocalPlayer);
        }
    }

    public class SpiderEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LifeHeader>();
        public override int ToggleItemType => ModContent.ItemType<SpiderEnchant>();
        public override bool MutantsPresenceAffects => true;
        //public override bool MinionEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.SpiderCD > 0)
                modPlayer.SpiderCD--;
            //minion crits
            modPlayer.MinionCrits = true;
        }
    }
}
