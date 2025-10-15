using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class OrichalcumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(235, 50, 145);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<OrichalcumEffect>(Item);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyOriHead")
                .AddIngredient(ItemID.OrichalcumBreastplate)
                .AddIngredient(ItemID.OrichalcumLeggings)
                .AddIngredient(ItemID.CursedFlames)
                .AddIngredient(ItemID.ShadowFlameBow)
                .AddIngredient(ItemID.Toxikarp)

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    public class OrichalcumEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<OrichalcumEnchant>();

        public override bool ExtraAttackEffect => true;

        public static float OriDotModifier(NPC npc, FargoSoulsPlayer modPlayer)
        {
            float multiplier = 1.75f;

            if (modPlayer.Player.ForceEffect<OrichalcumEffect>())
            {
                multiplier = 2.5f;
            }
            return multiplier;
        }

        public override void PostUpdateEquips(Player player)
        {
            if (!HasEffectEnchant(player))
                return;
            //player.onHitPetal = true;
        }

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            target.AddBuff(ModContent.BuffType<OriPoisonBuff>(), 60);
        }
    }
}
