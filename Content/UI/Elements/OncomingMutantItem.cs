using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.UI.Elements
{
    // Exists to be displayed as an item icon in the Toggler UI when inflicted with Mutant's Presence.
    public class OncomingMutantItem : ModItem
    {
        public override string Texture => FargoAssets.GetAssetString("UI", "OncomingMutant");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override bool? UseItem(Player player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<MutantBoss>());
            return true;
        }
    }
}