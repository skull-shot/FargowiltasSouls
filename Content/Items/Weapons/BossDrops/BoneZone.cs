using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class BoneZone : SoulsItem
    {
        private int counter = 1;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Bone Zone");
            /* Tooltip.SetDefault("Uses bones for ammo" +
                "\n33% chance to not consume ammo" +
                "\n'The shattered remains of a defeated foe..'"); */

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "骸骨领域");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'被击败的敌人的残骸..'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        private static readonly int[] RiffVariants = [1, 2, 3, 4];
        private static readonly SoundStyle badtothebone = new SoundStyle("FargowiltasSouls/Assets/Sounds/Boneriff/boneriff") with { Variants = RiffVariants, Volume = 0.15f };
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 14;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = badtothebone;
            Item.value = 50000;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Bonez>();
            Item.shootSpeed = 5.5f;
            Item.useAmmo = ItemID.Bone;
            Item.scale = 0.9f;
        }

        // Manually reposition the Item when held out
        public override Vector2? HoldoutOffset() => new Vector2(-20, -2);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int shoot;

            if (counter > 2)
            {
                shoot = ProjectileID.ClothiersCurse;
                counter = 0;
            }
            else
                shoot = ModContent.ProjectileType<Bonez>();

            int i = Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, shoot, damage, knockback, player.whoAmI);

            if (i != Main.maxProjectiles && shoot == ProjectileID.ClothiersCurse)
            {
                Main.projectile[i].DamageType = DamageClass.Ranged;
                Main.projectile[i].localNPCHitCooldown = 10;
                Main.projectile[i].usesLocalNPCImmunity = true;
            }
            

            counter++;

            return false;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            //damage *= (15f / 24f); //current usetime / old usetime
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => !Main.rand.NextBool(3);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 35f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}