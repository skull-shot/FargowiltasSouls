using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class BigBrainBuster : SoulsItem
    {
        public override string Texture => FargoAssets.GetAssetString("Content/Items/Weapons/SwarmDrops", Name);
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 1;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<BigBrainProj>();
            Item.shootSpeed = 10f;
            //Item.buffType = ModContent.BuffType<BigBrainMinion>();
            //Item.buffTime = 3600;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 10);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<BigBrainMinionBuff>(), 2);
            Vector2 spawnPos = player.Center - Main.MouseWorld;
            if (player.ownedProjectileCounts[type] == 0)
            {
                FargoSoulsUtil.NewSummonProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 0, spawnPos.ToRotation());
            }
            else
            {
                float usedslots = 0;
                int brain = -1;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.owner == player.whoAmI && proj.minionSlots > 0 && proj.active)
                    {
                        usedslots += proj.minionSlots;
                        if (proj.type == type)
                        {
                            brain = i;
                            if (usedslots < player.maxMinions && proj.minionSlots < BigBrainProj.MaxMinionSlots)
                                proj.minionSlots++;
                        }
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "BrainStaff")
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerBrain"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}