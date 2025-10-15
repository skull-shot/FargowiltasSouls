using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Consumables
{
    public class MutantsPact : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Consumables", Name);
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 6));
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = true;
            Item.UseSound = SoundID.Item123;
            Item.value = Item.sellPrice(0, 15);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.FargoSouls().MutantsPactSlot;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.FargoSouls().MutantsPactSlot = true;

                SoundEngine.PlaySound(SoundID.Roar, player.Center);
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Thunder"), player.Center);
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}