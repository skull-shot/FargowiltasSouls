using FargowiltasSouls.Content.Projectiles.Weapons.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class DragonBreath : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.DD2_BetsyFlameBreath;
            Item.useAmmo = AmmoID.Gel;
            //Item.staff[Item.type] = true;
            Item.value = 50000;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<DragonBreathProj>();
            Item.shootSpeed = 35f;
            Item.noUseGraphic = false;
        }

        //make them hold it different
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, -6);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;

            return true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => !Main.rand.NextBool(3);
    }
}
