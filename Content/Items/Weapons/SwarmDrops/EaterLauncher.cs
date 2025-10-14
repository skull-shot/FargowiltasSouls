using Fargowiltas.Content.Items.Summons.SwarmSummons.Energizers;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Content.Projectiles.Weapons.SwarmDrops;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class EaterLauncher : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/SwarmDrops", Name);
        private int delay;
        private bool lastLMouse = false;
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 135;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            //Item.UseSound = SoundID.Item62 with { Volume = 0.4f };
            Item.useAmmo = AmmoID.Rocket;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RockeaterLauncherHeld>();
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public const int BaseDistance = 600;
        public const int IncreasedDistance = 1200;
        //public int Cooldown = 0;
        public const int CooldownTime = 24 * 60;
        public override bool AltFunctionUse(Player player) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (lastLMouse && !Main.mouseLeft && CanUseItem(player) && FargoSoulsUtil.ActuallyClickingInGameplay(player))
            {
                delay = (int)MathF.Ceiling(20f / player.FargoSouls().AttackSpeed);
            }
            if (delay-- < 0)
            {
                delay = 0;
            }
            lastLMouse = Main.mouseLeft;

            if (player.channel && player.ownedProjectileCounts[Item.shoot] < 1 && delay == 0)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, (int)(player.ActualClassDamage(DamageClass.Ranged) * Item.damage), Item.knockBack, player.whoAmI);
            }
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool() && player.channel && player.ownedProjectileCounts[Item.shoot] > 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<EaterLauncherJr>())
            .AddIngredient(ModContent.ItemType<EnergizerWorm>())
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.TileType<CrucibleCosmosSheet>())

            .Register();
        }
    }
}