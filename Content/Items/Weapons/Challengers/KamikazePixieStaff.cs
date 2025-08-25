using System;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class KamikazePixieStaff : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/Challengers", Name);
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Summon;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<KamikazePixie>();
            Item.shootSpeed = 1f;
            Item.mana = 10;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity = velocity.RotatedBy(Math.PI / 2) * 10;
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback, default, velocity);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback, default, -velocity);
            return false;
        }

        //public override bool AltFunctionUse(Player player) => true;

        public override bool CanShoot(Player player) => player.altFunctionUse != 2;

        public override float UseSpeedMultiplier(Player player)
        {
            //if (player.altFunctionUse == 2) return 0.5f;

            return base.UseSpeedMultiplier(player);
        }

        public override bool? UseItem(Player player)
        {
            /*if (player.ItemTimeIsZero && player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == Item.shoot))
                    p.Kill();
            }*/

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<LifelightBag>(2).AddTile(TileID.Solidifier).DisableDecraft().Register();
        }
    }
}