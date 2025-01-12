using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Patreon.Tiger;
using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Tiger
{
    public class TouhouStaff : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<CirnoMinion>();
            Item.shootSpeed = 10f;
            Item.buffType = ModContent.BuffType<TouhouHoardBuff>();
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 8);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<TouhouHoardBuff>(), 2);


            Vector2 spawnPos = Main.MouseWorld;
            float usedminionslots = 0;
            var minions = Main.projectile.Where(x => x.minionSlots > 0 && x.owner == player.whoAmI && x.active);
            foreach (Projectile minion in minions)
                usedminionslots += minion.minionSlots;

            if (usedminionslots < player.maxMinions)
            {
                List<int> minionList = new List<int>();
                minionList.Add(ModContent.ProjectileType<CirnoMinion>());
                minionList.Add(ModContent.ProjectileType<DaiyouseiMinion>());
                minionList.Add(ModContent.ProjectileType<MystiaMinion>());
                minionList.Add(ModContent.ProjectileType<RumiaMinion>());
                minionList.Add(ModContent.ProjectileType<WriggleMinion>());

                int minionType = -1;

                foreach (int checkMinion in minionList)
                {
                    if (!hasMinion(player, checkMinion))
                    {
                        minionType = checkMinion;
                        break;
                    }
                }

                if (minionType == -1)
                {
                    return false;
                }

                FargoSoulsUtil.NewSummonProjectile(source, spawnPos, Main.rand.NextVector2Circular(10, 10), minionType, Item.damage, knockback, player.whoAmI);
            }
            return false;
        }

        private bool hasMinion(Player player, int projType)
        {
            return player.ownedProjectileCounts[projType] >= 1;
        }

        public override void AddRecipes()
        {
            if (SoulConfig.Instance.PatreonTouhou)
            {

                CreateRecipe()
                .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)
                .AddIngredient(ItemID.FlinxFur, 10)
                .AddIngredient(ItemID.ZebraSwallowtailButterfly, 10)//recipe group tm
                .AddIngredient(ItemID.Firefly, 10)
                .AddIngredient(ItemID.Feather, 10)
                .AddTile(TileID.Anvils)
                .Register();
            }
        }
    }
}
