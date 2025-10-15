using System;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class CrimsonEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(200, 54, 75);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<CrimsonEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimsonHelmet)
                .AddIngredient(ItemID.CrimsonScalemail)
                .AddIngredient(ItemID.CrimsonGreaves)
                .AddIngredient(ItemID.TheUndertaker)
                .AddIngredient(ItemID.TheMeatball)
                .AddIngredient(ItemID.CrimsonHeart)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class CrimsonEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<CrimsonEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.CrimsonRegenTime > (modPlayer.ForceEffect<CrimsonEnchant>() ? 420 * 2 : 420))
            { //if its force effect, end at 14 seconds instead of 7
                player.ClearBuff(ModContent.BuffType<CrimsonRegenBuff>());
            }
        }
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            if (!HasEffectEnchant(player))
                return;
            //if was already healing, stop the heal and do nothing
            if (player.HasBuff<CrimsonRegenBuff>())
            {
                player.ClearBuff(ModContent.BuffType<CrimsonRegenBuff>());
            }
            else
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                if (info.Damage < 10)
                    return; //ignore hits under 10 damage
                modPlayer.CrimsonRegenTime = 0; //reset timer
                const float HealPercentage = 50f / 100f; //% of damage given back
                float heal = (int)Math.Round(info.Damage * HealPercentage);
                const int Softcap = 200;
                if (heal > Softcap)
                    heal *= (Softcap * 2 + heal) / (3f * heal); // calculate post-softcap value
                modPlayer.CrimsonRegenAmount = (int)heal; //50% return heal, softcapped past 200

                player.AddBuff(ModContent.BuffType<CrimsonRegenBuff>(),
                    modPlayer.ForceEffect<CrimsonEnchant>() ? 900 : 430); //should never reach that time lol. buff gets removed in buff itself after its done. sets to actual time so that it shows in buff properly
            }
        }
    }
}
