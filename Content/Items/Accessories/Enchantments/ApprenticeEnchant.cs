using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ApprenticeEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(93, 134, 166);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().ApprenticeEnchantActive = true;
            player.AddEffect<ApprenticeSupport>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ApprenticeHat)
            .AddIngredient(ItemID.ApprenticeRobe)
            .AddIngredient(ItemID.ApprenticeTrousers)
            .AddIngredient(ItemID.DD2FlameburstTowerT2Popper)
            //magic missile
            //ice rod
            //golden shower
            .AddIngredient(ItemID.BookStaff)
            .AddIngredient(ItemID.ClingerStaff)

            .AddTile(TileID.CrystalBall)
            .Register();
        }

        
        public static MethodInfo ApprenticeShootMethod
        {
            get;
            set;
        }
        public override void Load()
        {
            ApprenticeShootMethod = typeof(Player).GetMethod("ItemCheck_Shoot", LumUtils.UniversalBindingFlags);
        }
        public static void ApprenticeShoot(Player player, int playerWhoAmI, Item item, int weaponDamage)
        {
            object[] args = new object[] { playerWhoAmI, item, weaponDamage };
            ApprenticeShootMethod.Invoke(player, args);
;
        }
        
    }
    public class ApprenticeSupport : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<ApprenticeEnchant>();
        public override bool ExtraAttackEffect => true;
        public static List<int> Blacklist = [];
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool forceEffect = modPlayer.ForceEffect<ApprenticeEnchant>();
            //update item cd
            if (modPlayer.ApprenticeItemCD > 0)
            {
                modPlayer.ApprenticeItemCD--;
            }

            if (player.controlUseItem)
            {

                if (player.controlUseItem)
                {
                    Item item = player.HeldItem;

                    //non weapons and weapons with no ammo begone
                    if (item.damage <= 0 || !player.HasAmmo(item) || (item.mana > 0 && player.statMana < item.mana)
                        || item.createTile != -1 || item.createWall != -1 || item.ammo != AmmoID.None || item.hammer != 0 || item.pick != 0 || item.axe != 0) return;

                    int startingSlot = 0;

                    //first we need to find what slot the current weapon is
                    for (int j = 0; j < 10; j++) //hotbar
                    {
                        Item item2 = player.inventory[j];

                        if (item2.type == item.type)
                        {
                            startingSlot = j;
                            break;
                        }
                    }

                    int weaponsUsed = 0;

                    //then go from there and find the next weapon to fire
                    for (int j = startingSlot; j < 10; j++) //hotbar
                    {
                        Item item2 = player.inventory[j];

                        if (item2 != null && item2.damage > 0 && item2.shoot > ProjectileID.None && item2.ammo <= 0 && item.type != item2.type && !item2.channel)
                        {
                            if (!player.HasAmmo(item2) || (item2.mana > 0 && player.statMana < item2.mana) || item2.sentry || ContentSamples.ProjectilesByType[item2.shoot].minion || Blacklist.Contains(item2.type))
                                continue;

                            weaponsUsed++;
                            if (weaponsUsed > 1)
                                break;

                            int itemCD = modPlayer.ApprenticeItemCD;

                            if (itemCD > 0)
                                continue;

                            if (!PlayerLoader.CanUseItem(player, item2) || !ItemLoader.CanUseItem(item2, player))
                                continue;

                            Vector2 pos = new(player.Center.X + Main.rand.Next(-50, 50), player.Center.Y + Main.rand.Next(-50, 50));
                            Vector2 velocity = Vector2.Normalize(Main.MouseWorld - pos);
                            
                            int projToShoot = item2.shoot;
                            float speed = item2.shootSpeed;
                            int damage = item2.damage;
                            float KnockBack = item2.knockBack;

                            //damage = (int)(damage * 0.75f);

                            //FargoSoulsGlobalProjectile.ApprenticeDamageCap = damage;
                            //ApprenticeEnchant.ApprenticeShoot(player, player.whoAmI, item2, damage);
                            //FargoSoulsGlobalProjectile.ApprenticeDamageCap = 0;

                            int divisor = 7;
                            if (modPlayer.DarkArtistEnchantActive && forceEffect)
                            {
                                divisor = 3;
                            }
                            else if (modPlayer.DarkArtistEnchantActive || forceEffect)
                            {
                                divisor = 5;
                            }

                            if (player.HasEffect<ShadowForceEffect>())
                                divisor = 10;

                            modPlayer.ApprenticeItemCD = item2.useAnimation * divisor;

                            
                            if (item2.useAmmo > 0)
                            {
                                player.PickAmmo(item2, out projToShoot, out speed, out damage, out KnockBack, out int usedAmmoItemId, ItemID.Sets.gunProj[item2.type]);
                            }
                            

                            //damage = (int)(damage * 0.75f);

                            if (item2.mana > 0)
                            {
                                if (player.CheckMana(item2.mana / 2, true, false))
                                {
                                    player.manaRegenDelay = 300;
                                }
                            }
                            if (item2.consumable)
                            {
                                item2.stack--;
                            }
                            
                            //modPlayer.ApprenticeItemCD = item2.useAnimation * 4;

                            if (projToShoot == ProjectileID.RainbowFront || projToShoot == ProjectileID.RainbowBack) // prevent fucked up op interaction
                            {
                                foreach (Projectile rainbow in Main.projectile.Where(p => (p.TypeAlive(ProjectileID.RainbowFront) || p.TypeAlive(ProjectileID.RainbowBack)) && p.owner == player.whoAmI))
                                    rainbow.Kill();
                            }
                            int p = Projectile.NewProjectile(player.GetSource_ItemUse(item), pos, Vector2.Normalize(velocity) * speed, projToShoot, damage, KnockBack, player.whoAmI);
                            Projectile proj = Main.projectile[p];

                            proj.FargoSouls().DamageCap = (int)MathHelper.Max(proj.FargoSouls().DamageCap, damage);
                            proj.noDropItem = true;

                            break;
                            /*
                            int shoot = item2.shoot;
                            if (shoot == 10) //purification powder
                            {
                                float speed;
                                int damage;
                                float kb;
                                int usedAmmo;
                                
                                ItemLoader.ModifyShootStats(item2, player, ref pos, ref velocity, ref shoot, ref damage, ref item2.knockBack);
                            }
                            */
                            //proj.usesLocalNPCImmunity = true;
                            //proj.localNPCHitCooldown = 5;

                        }
                    }
                }
            }
        }
    }
}
